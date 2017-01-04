namespace WoFlagship.KancolleRequirement
{
    public class ShipCondition
    {
        /// <summary>
        /// 约束项
        /// </summary>
        public ShipConstraint[] Constraints { get; set; }

        /// <summary>
        /// 到达的概率
        /// </summary>
        public double Possibility { get; set; }
    }
}
