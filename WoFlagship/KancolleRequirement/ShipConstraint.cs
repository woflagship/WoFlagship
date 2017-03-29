using System;

namespace WoFlagship.KancolleRequirement
{
    public class ShipConstraint
    {
        /// <summary>
        /// 表示约束的linq的where语句
        /// 不要再加额外的where关键字
        /// 注意变量名为s
        /// 例如 s.Id == 3 且 s.TypeId == -2
        /// </summary>
        public string ConstraintLinq { get; set; }

        /// <summary>
        /// 满足条件的船的个数
        /// Amount和Selection只有一个参数有效
        /// Amount>0,则以Amount为准，否则以Selection为准
        /// Selection元素包含-1，则该selection也无效，此时表示船的数量无要求
        /// </summary>
        public int Amount { get; set; } = -1;


        /// <summary>
        /// 满足条件的船的范围
        /// Amount和Selection只有一个参数有效
        /// Amount>0,则以Amount为准，否则以Selection为准
        /// Selection元素包含-1，则该selection也无效，此时表示船的数量无要求
        /// </summary>
        public Tuple<int, int> Selection { get; set; } = new Tuple<int, int>(-1,-1);

      
    }

}
