using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
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
}
