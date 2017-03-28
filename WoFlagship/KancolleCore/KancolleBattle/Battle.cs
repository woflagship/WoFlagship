using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.Utils;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class Battle
    {
        private static readonly AttackTypes[] DayAttackTypeMap = new AttackTypes[]
       {
            AttackTypes.Normal,
            AttackTypes.Laser,
            AttackTypes.Double,
            AttackTypes.Primary_Secondary_CI,
            AttackTypes.Primary_Radar_CI,
            AttackTypes.Primary_AP_CI,
            AttackTypes.Primary_Primary_CI,
       };

        private static readonly AttackTypes[] NightAttackTypeMap = new AttackTypes[]
        {
            AttackTypes.Normal,
            AttackTypes.Double,
            AttackTypes.Primary_Torpedo_CI,
            AttackTypes.Torpedo_Torpedo_CI,
            AttackTypes.Primary_Secondary_CI,
            AttackTypes.Primary_Primary_CI,
        };

        private static readonly StageTypes[] SupportTypeMap = new StageTypes[]
        {
            StageTypes.Aerial,
            StageTypes.Shelling,
            StageTypes.Torpedo
        };

        public BattleTypes BattleType { get; set; }
        /// <summary>
        /// [body.api_maparea_id, body.api_mapinfo_no, body.api_no]
        /// </summary>
       // public int[] Map { get; private set; }
        public int FleetType { get; private set; }
        public int EnemyType { get; private set; }
        public Fleet MainFleet { get; private set; }
        public Fleet EscortFleet { get; private set; }
        public Fleet EnemyFleet { get; private set; }
        public Fleet EnemyEscort { get; private set; }
        public Fleet SupportFleet { get; private set; }
        public List<Stage> Stages { get; private set; } = new List<Stage>();


        /// <summary>
        /// 创建一次战斗的实例
        /// </summary>
        /// <param name="mainFleet">第一舰队</param>
        /// <param name="escortFleet">第二舰队，联合舰队时有效；单舰队时为null</param>
        /// <param name="fleetType">联合舰队类型,0-通常部队，1-空母机动部队，2-水上打击部队，3-输送护卫部队</param>
        /// <param name="supportFleet">支援舰队，无支援舰队为null</param>
        public Battle(Fleet mainFleet, Fleet escortFleet, int fleetType, Fleet supportFleet)
        {
            this.MainFleet = mainFleet;
            this.EscortFleet = escortFleet;
            this.FleetType = fleetType;
            this.SupportFleet = supportFleet;
        }

        public override string ToString()
        {
            string str = "";
            foreach (var stage in Stages)
            {
                str += stage.ToString() + "\n";
            }
            return str;
        }

        internal void Simulate(api_battle_data packet)
        {
            if (EnemyFleet == null)
            {
                EnemyFleet = new Fleet(new JArray(packet.api_ship_ke), packet.api_eSlot, packet.api_maxhps, packet.api_nowhps, packet.api_ship_lv, ShipOwner.Enemy, 0);
                if (packet.api_ship_ke_combined != null)
                    EnemyEscort = new Fleet(new JArray(packet.api_ship_ke_combined), packet.api_eSlot_combined, packet.api_maxhps_combined, packet.api_nowhps_combined, packet.api_ship_lv_combined, ShipOwner.Enemy, 6);
            }
            //以下两行在KancolleBattleManager中实现
            // HACK: Only enemy carrier task force now.
            //const enemyType = (path.includes('ec_') || path.includes('each_')) ? 1 : 0

            Stages.Clear();
            // Engagement
            Stages.Add(GetEngagementStage(packet));
            // Land base air attack
            if (packet.api_air_base_attack != null)
            {
                foreach (api_kouku kouku in packet.api_air_base_attack)
                {
                    Stages.Add(SimulateLandBase(EnemyFleet, EnemyEscort, kouku));
                }
            }
            //Aerial Combat
            Stages.AddIfNotNull(SimulateAerial(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_kouku));
            // Aerial Combat 2nd
            Stages.AddIfNotNull(SimulateAerial(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_kouku2));
            // Expedition Support Fire
            Stage stage_support = SimulateSupport(EnemyFleet, EnemyEscort, packet.api_support_info, packet.api_support_flag);
            Stages.AddIfNotNull(stage_support);

            //Normal Fleet
            if (FleetType == 0)
            {
                if (EnemyType == 0)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, null, EnemyFleet, null, packet.api_opening_atack, StageTypes.SOpening));
                    //Shelling(Main), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki1));
                    // Shelling (Main), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki2));
                    //Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, null, EnemyFleet, null, packet.api_raigeki));
                }
                if (EnemyType == 1)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_opening_atack, StageTypes.SOpening));
                    // Shelling (Escort)
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_hougeki1));
                    // Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_raigeki));
                    // Shelling (Any), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_hougeki2));
                    // Shelling (Any), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, EnemyEscort, packet.api_hougeki3));
                }
            }

            // Surface Task Force, 水上打撃部隊
            if (FleetType == 2)
            {
                if (EnemyType == 0)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(EscortFleet, null, EnemyFleet, null, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(EscortFleet, null, EnemyFleet, null, packet.api_opening_atack, StageTypes.SOpening));
                    // Shelling (Main), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki1, StageTypes.SMain));
                    // Shelling (Main), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki2, StageTypes.SMain));
                    // Shelling (Escort)
                    Stages.AddIfNotNull((Stage)SimulateShelling(EscortFleet, null, EnemyFleet, null, packet.api_hougeki3, StageTypes.SEscort));
                    // Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(EscortFleet, null, EnemyFleet, null, packet.api_raigeki));
                }
                if (EnemyType == 1)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_opening_atack, StageTypes.SOpening));
                    // Shelling (Main), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki1, StageTypes.SMain));
                    // Shelling (Main), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki2, StageTypes.SMain));
                    // Shelling (Escort)
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki3, StageTypes.SEscort));
                    // Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_raigeki));
                }
            }

            // Carrier Task Force, 空母機動部隊
            // Transport Escort, 輸送護衛部隊
            if (FleetType == 1 | FleetType == 3)
            {
                if (EnemyType == 0)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(EscortFleet, null, EnemyFleet, null, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(EscortFleet, null, EnemyFleet, null, packet.api_opening_atack, StageTypes.SOpening));
                    // Shelling (Escort)
                    Stages.AddIfNotNull((Stage)SimulateShelling(EscortFleet, null, EnemyFleet, null, packet.api_hougeki1, StageTypes.SEscort));
                    // Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(EscortFleet, null, EnemyFleet, null, packet.api_raigeki));
                    // Shelling (Main), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki2, StageTypes.SMain));
                    // Shelling (Main), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, null, EnemyFleet, null, packet.api_hougeki3, StageTypes.SMain));

                }
                if (EnemyType == 1)
                {
                    // Opening Anti-Sub
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_opening_taisen, StageTypes.SOpening));
                    //Opening Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_opening_atack, StageTypes.SOpening));
                    // Shelling (Main), 1st
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki1, StageTypes.SMain));
                    // Shelling (Escort)
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki2, StageTypes.SEscort));
                    // Closing Torpedo Salvo
                    Stages.AddIfNotNull((Stage)SimulateTorpedo(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_raigeki));
                    // Shelling (Main), 2nd
                    Stages.AddIfNotNull((Stage)SimulateShelling(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, packet.api_hougeki3, StageTypes.SMain));

                }
            }

            //省略了对api判断
            // TODO: We need better solution
            // HACK: Add Engagement Stage to sp_midnight battle.


            // Night Combat
            Stages.AddIfNotNull((Stage)SimulateNight(FleetType, MainFleet, EscortFleet, EnemyType, EnemyFleet, EnemyEscort, packet.api_hougeki, packet));
        }

        private Stage simulateAerial(api_kouku kouku)
        {
            return SimulateAerial(MainFleet, EscortFleet, EnemyFleet, EnemyEscort, kouku);
        }

        internal static Stage SimulateAerial(Fleet mainFleet, Fleet escortFleet, Fleet enemyFleet, Fleet enemyEscort, api_kouku kouku)
        {
            if (kouku == null)
                return null;
            Stage stage = new Stage();
            stage.StageType = StageTypes.Aerial;
            List<Attack> attacks = new List<Attack>();
            if (kouku.api_stage3 != null)
            {
                var st3 = kouku.api_stage3;
                attacks.AddRange(SimulateAerialAttack(enemyFleet, st3.api_edam, st3.api_ebak_flag, st3.api_erai_flag, st3.api_ecl_flag));
                attacks.AddRange(SimulateAerialAttack(mainFleet, st3.api_fdam, st3.api_fbak_flag, st3.api_frai_flag, st3.api_fcl_flag));
            }

            if (kouku.api_stage3_combined != null)
            {
                var st3 = kouku.api_stage3_combined;
                attacks.AddRange(SimulateAerialAttack(enemyEscort, st3.api_edam, st3.api_ebak_flag, st3.api_erai_flag, st3.api_ecl_flag));
                attacks.AddRange(SimulateAerialAttack(escortFleet, st3.api_fdam, st3.api_fbak_flag, st3.api_frai_flag, st3.api_fcl_flag));
            }
            stage.Attacks = attacks;
            return stage;
        }

        public static List<Attack> SimulateAerialAttack(Fleet fleet, double[] edam, int[] ebak_flag, int[] erai_flag, int[] ecl_flag)
        {
            List<Attack> attacks = new List<Attack>();

            if (fleet == null || edam == null)
                return attacks;
            for (int i = 1; i < edam.Length; i++)
            {
                int damage = (int)Math.Floor(edam[i]);
                if (damage < 0 || (ebak_flag[i] <= 0 && erai_flag[i] <= 0))
                    continue;
                var toShip = fleet[i - 1];
                var hit = (ecl_flag[i] == 1 ? HitTypes.Critical : (damage > 0 ? HitTypes.Hit : HitTypes.Miss));
                toShip.Damage(damage);
                attacks.Add(new Attack(AttackTypes.Normal, null, toShip,new int[] { damage }, new HitTypes[] { hit }));
            }

            return attacks;
        }

        internal static Stage SimulateTorpedo(Fleet mainFleet, Fleet escortFleet, Fleet enemyFleet, Fleet enemyEscort, api_raigeki raigeki, StageTypes subType = StageTypes.SUndefined)
        {
            if (raigeki == null)
                return null;
            List<Attack> attacks = new List<Attack>();
            if (raigeki.api_frai != null)
                attacks.AddRange(SimulateTorpedoAttack(mainFleet, escortFleet, enemyFleet, enemyEscort, raigeki.api_fydam, raigeki.api_frai, raigeki.api_fcl));
            if (raigeki.api_erai != null)
                attacks.AddRange(SimulateTorpedoAttack(enemyFleet, enemyEscort, mainFleet, escortFleet, raigeki.api_eydam, raigeki.api_erai, raigeki.api_ecl));

            Stage stage = new Stage
            {
                StageType = StageTypes.Torpedo,
                SubType = subType,
                Attacks = attacks,
            };
            return stage;
        }

        public static List<Attack> SimulateTorpedoAttack(Fleet mainFleet, Fleet escortFleet, Fleet enemyFleet, Fleet enemyEscort, double[] api_eydam, int[] api_erai, int[] api_ecl)
        {
            List<Attack> attacks = new List<Attack>();
            if (enemyFleet == null || api_eydam == null)
                return attacks;
            for (int i = 1; i < api_erai.Length; i++)
            {
                int t = api_erai[i];
                if (t <= 0) continue;
                Ship fromShip, toShip;
                if (i <= 6) fromShip = mainFleet[i - 1];
                else fromShip = escortFleet[i - 7];
                if (t <= 6) toShip = enemyFleet[t - 1];
                else toShip = enemyEscort[t - 7];

                int damage = (int)Math.Floor(api_eydam[i]);
                var hit = (api_ecl[i] == 2 ? HitTypes.Critical : (api_ecl[i] == 1 ? HitTypes.Hit : HitTypes.Miss));
                toShip.Damage(damage);
                attacks.Add(new Attack( AttackTypes.Normal, fromShip, toShip, new int[] { damage}, new HitTypes[] { hit}));
            }
            return attacks;
        }

        internal static Stage SimulateShelling(Fleet mainFleet, Fleet escortFleet, Fleet enemyFleet, Fleet enemyEscort, api_hougeki hougeki, StageTypes subType = StageTypes.SUndefined)
        {
            if (hougeki == null)
                return null;
            bool isNight = (subType == StageTypes.SNight);
            List<Attack> attacks = new List<Attack>();
            for (int i = 1; i < hougeki.api_at_list.Length; i++)
            {
                int at = hougeki.api_at_list[i];
                if (at == -1) continue;

                at = at - 1;//attacker
                int df = (hougeki.api_df_list[i] as JArray)[0].ToObject<int>() - 1;//defender
                bool fromEnemy;
                if (hougeki.api_at_eflag != null)
                {
                    fromEnemy = hougeki.api_at_eflag[i] == 1;
                }
                else
                {
                    fromEnemy = df < 6;
                    if (at >= 6) at -= 6;
                    if (df >= 6) df -= 6;
                }

                Ship fromShip, toShip;
                if (fromEnemy)
                {
                    fromShip = at < 6 ? enemyFleet[at] : enemyEscort[at - 6];
                    toShip = df < 6 ? mainFleet[df] : escortFleet[df - 6];
                }
                else
                {
                    fromShip = at < 6 ? mainFleet[at] : escortFleet[at - 6];
                    toShip = at < 6 ? enemyFleet[df] : enemyEscort[df - 6];
                }

                var attackType = isNight ? NightAttackTypeMap[hougeki.api_sp_list[i]] : DayAttackTypeMap[hougeki.api_at_type[i]];
                List<int> damage = new List<int>();
                int damageTotal = 0;
                foreach (double dmg_i in (hougeki.api_damage[i] as JArray).ToObject<double[]>())
                {
                    double dmg = dmg_i;
                    if (dmg < 0)
                        dmg = 0;
                    dmg = Math.Floor(dmg);
                    damage.Add((int)dmg);
                    damageTotal += (int)dmg;
                }

                List<HitTypes> hits = new List<HitTypes>();
                foreach (int cl in (hougeki.api_cl_list[i] as JArray).ToObject<int[]>())
                {
                    hits.Add(cl == 2 ? HitTypes.Critical : (cl == 1 ? HitTypes.Hit : HitTypes.Miss));
                }
                toShip.Damage(damageTotal);
                attacks.Add(new Attack(attackType, fromShip, toShip, damage.ToArray(), hits.ToArray()));
            }
            return new Stage()
            {
                StageType = StageTypes.Shelling,
                Attacks = attacks,
                SubType = subType
            };
        }

        internal static Stage SimulateNight(int fleetType, Fleet mainFleet, Fleet escortFleet, int enemyType, Fleet enemyFleet, Fleet enemyEscort, api_hougeki hougeki, api_battle_data packet)
        {
            if (hougeki == null)
                return null;
            Fleet _oursFleet = fleetType == 0 ? mainFleet : escortFleet;
            Fleet _enemyFleet = enemyType == 0 ? enemyFleet : enemyEscort;
            if (packet.api_active_deck != null)
            {
                if (packet.api_active_deck[0] == 1)
                {
                    _oursFleet = mainFleet;
                }
                if (packet.api_active_deck[0] == 2)
                {
                    _oursFleet = escortFleet;
                }
                if (packet.api_active_deck[1] == 1)
                {
                    _enemyFleet = enemyFleet;
                }
                if (packet.api_active_deck[1] == 2)
                {
                    _enemyFleet = enemyEscort;
                }
            }
            Stage stage = SimulateShelling(_oursFleet, null, _enemyFleet, null, hougeki, StageTypes.SNight);

            return stage;
        }

        internal static Stage SimulateSupport(Fleet enemyFleet, Fleet enemyEscort, dynamic support, Nullable<int> flag)
        {
            if (support == null || flag == null)
            {
                return null;
            }
            if (flag == 1)
            {
                var stage = SimulateAerial(null, null, enemyFleet, enemyEscort, support.api_support_airatack);
                return new Stage()
                {
                    StageType = StageTypes.Support,
                    Attacks = stage.Attacks,
                    SubType = SupportTypeMap[(int)flag]
                };

            }
            if (flag == 2 || flag == 3)
            {
                var hourai = support.api_support_hourai;
                List<Attack> attacks = new List<Attack>();
                for (int i = 1; i < hourai.api_damage.Length; i++)
                {
                    Ship toShip = null;
                    if (1 <= i && i <= 6)
                        toShip = enemyFleet[i - 1];
                    if (7 <= i && i <= 12)
                        toShip = enemyEscort[i - 7];
                    if (toShip == null)
                        continue;
                    int damage = (int)Math.Floor(hourai.api_damage[i]);
                    var cl = hourai.api_cl_list[i];
                    HitTypes hit = (cl == 2 ? HitTypes.Critical : (cl == 1 ? HitTypes.Hit : HitTypes.Miss));
                    // No showing Miss attack on support stage.
                    if (hit == HitTypes.Miss)
                        continue;
                    toShip.Damage(damage);
                    attacks.Add(new Attack( AttackTypes.Normal, null, toShip,new int[] { damage }, new HitTypes[] { hit} ));
                }
                return new Stage()
                {
                    StageType = StageTypes.Support,
                    SubType = SupportTypeMap[(int)flag],
                    Attacks = attacks,
                };
            }
            return null;
        }

        internal static Stage SimulateLandBase(Fleet enemyFleet, Fleet enemyEscort, api_kouku kouku)
        {
            var stage = SimulateAerial(null, null, enemyFleet, enemyEscort, kouku);
            stage.StageType = StageTypes.LandBase;
            return stage;
        }

        //未完成！！！！！！
        internal static Stage GetEngagementStage(api_battle_data packet)
        {
            return new Stage()
            {
                StageType = StageTypes.Engagement
            };
        }
    }
}
