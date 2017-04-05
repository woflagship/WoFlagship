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

        private Func<Task<bool>> condition;

        public ConditionNode(string name, Func<Task<bool>> condition)
        {
            Name = name;
            this.condition = condition;
        }

        public async Task<BehaviorTreeStatus> BehaveAsync()
        {
            if (await condition())
                return BehaviorTreeStatus.Success;
            else
                return BehaviorTreeStatus.Failure;
        }
    }
}
