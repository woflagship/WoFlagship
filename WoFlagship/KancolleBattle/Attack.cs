using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleBattle
{
    public enum AttackTypes
    {
        Normal,
        Laser,
        Double,
        Primary_Secondary_CI,//主砲/副砲
        Primary_Radar_CI,//主砲/電探
        Primary_AP_CI,//主砲/徹甲
        Primary_Primary_CI,//主砲/主砲
        Primary_Torpedo_CI,//主砲/魚雷
        Torpedo_Torpedo_CI//魚雷/魚雷


    }

    public enum HitTypes
    {
        Miss,
        Hit,
        Critical
    }


    public class Attack
    {
        public AttackTypes AttackType { get; set; }
        public Ship FromShip { get; set; }
        public Ship ToShip { get; set; }
        public int[] Damages { get; set; }
        public HitTypes[] Hits { get; set; }
        public int FromHP { get; set; }
        public int ToHp { get; set; }
        public int ItemUse { get; set; }

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
