using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    /// <summary>
    /// 攻击类型
    /// </summary>
    public enum AttackTypes
    {
        /// <summary>
        /// 通常攻击
        /// </summary>
        Normal,

        /// <summary>
        /// 未知
        /// </summary>
        Laser,

        /// <summary>
        /// 连击
        /// </summary>
        Double,

        /// <summary>
        /// 主副CI
        /// </summary>
        Primary_Secondary_CI,//主砲/副砲

        /// <summary>
        /// 主电碳CI
        /// </summary>
        Primary_Radar_CI,//主砲/電探

        /// <summary>
        /// 主彻甲CI
        /// </summary>
        Primary_AP_CI,//主砲/徹甲

        /// <summary>
        /// 主主CI
        /// </summary>
        Primary_Primary_CI,//主砲/主砲

        /// <summary>
        /// 主雷CI
        /// </summary>
        Primary_Torpedo_CI,//主砲/魚雷

        /// <summary>
        /// 鱼雷CI
        /// </summary>
        Torpedo_Torpedo_CI//魚雷/魚雷
    }
}
