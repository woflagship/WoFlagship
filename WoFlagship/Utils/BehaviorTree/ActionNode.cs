using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public class ActionNode : IBehaviorNode
    {

        private Func<DateTime, BehaviorTreeStatus> actionFunc;
        public string Name { get; protected set; }

        public ActionNode(string name, Func<DateTime, BehaviorTreeStatus> actionFunc)
        {
            Name = name;
            this.actionFunc = actionFunc;
        }

        public BehaviorTreeStatus Behave(DateTime duration)
        {
            return actionFunc(duration);
        }
    }
}
