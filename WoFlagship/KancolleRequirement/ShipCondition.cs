using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
