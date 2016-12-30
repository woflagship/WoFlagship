using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public interface IBehaviorNode
    {
        string Name { get; }

        BehaviorTreeStatus Behave(DateTime duration);
    }
}
