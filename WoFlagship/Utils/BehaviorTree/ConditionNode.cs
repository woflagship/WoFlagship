using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WoFlagship.Utils.BehaviorTree
{
    public class ConditionNode : IBehaviorNode
    {
        public string Name { get; protected set; }

        private Func<DateTime, bool> condition;

        public ConditionNode(string name, Func<DateTime, bool> condition)
        {
            Name = name;
            this.condition = condition;
        }

        public BehaviorTreeStatus Behave(DateTime duration)
        {
            if (condition(duration))
                return BehaviorTreeStatus.Success;
            else
                return BehaviorTreeStatus.Failure;
        }
    }
}
