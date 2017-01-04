using System;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public enum RequirementTypes
    {
        /// <summary>
        /// 单个船
        /// </summary>
        SingleShip,
        /// <summary>
        /// 船的类型，例如驱逐，航战等等
        /// </summary>
        ShipType,

    }

    [Serializable]
    public class ShipRequirement
    {
        public RequirementTypes RequirementType { get; set; }

        /// <summary>
        /// 是否为旗舰
        /// 空代表无要求
        /// </summary>
        public bool Flagship { get; set; }

        /// <summary>
        /// 船id候选项
        /// </summary>
        public int[] Ship { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int[] Amount { get; set; }

        /// <summary>
        /// 从候选项中挑选的个数
        /// 个数范围为Select[0]到Select[1]
        /// </summary>
        public int[] Select { get; set; }

    }
}
