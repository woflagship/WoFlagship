using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class Attack
    {
        public AttackTypes AttackType { get; set; }
        public Ship FromShip { get; set; }
        public Ship ToShip { get; set; }
        public ReadOnlyCollection<int> Damages { get; set; }
        public ReadOnlyCollection<HitTypes> Hits { get; set; }

        /// <summary>
        /// 创建一次攻击实例
        /// 注意，toShip所受到的伤害damages作用需要自行计算，该Attack实例不会计算toShip.Damage(damges)
        /// </summary>
        /// <param name="attackType"></param>
        /// <param name="fromShip"></param>
        /// <param name="toShip"></param>
        /// <param name="damages"></param>
        /// <param name="hits"></param>
        public Attack(AttackTypes attackType, Ship fromShip, Ship toShip, int[] damages, HitTypes[] hits)
        {
            AttackType = attackType;
            FromShip = fromShip;
            ToShip = toShip;
            Damages = new ReadOnlyCollection<int>(damages);
            Hits = new ReadOnlyCollection<HitTypes>(hits);
        }

        public override string ToString()
        {
            int ds = 0;
            if (Damages != null)
            {
                foreach (var d in Damages)
                    ds += d;
            }
            string str = string.Format("Attack:{0}\t从[{1}] 到 [{2}] 伤害[{3}]", AttackType.ToString(), FromShip.Position, ToShip.Position, ds);
            return str;
        }
    }
}
