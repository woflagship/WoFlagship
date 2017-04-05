using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    /// <summary>
    /// Child状态为Success，反转为Failure
    /// Child状态为Failure，反转为Success
    /// Child状态为Running，仍然为Running
    /// </summary>
    public class Invertor : DecoratorNode
    {
        public Invertor(string name, IBehaviorNode child) : base(name, child) { }

        public override async Task<BehaviorTreeStatus> BehaveAsync()
        {
            var childStatus = await Child.BehaveAsync();
            if (childStatus == BehaviorTreeStatus.Failure)
                return BehaviorTreeStatus.Success;
            else if (childStatus == BehaviorTreeStatus.Success)
                return BehaviorTreeStatus.Failure;
            else return BehaviorTreeStatus.Running;
        }
    }
}
