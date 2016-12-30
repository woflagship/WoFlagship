using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    /// <summary>
    /// 重复运行Child TargetCount次
    /// </summary>
    public class Counter : DecoratorNode
    {
        public int CurrentCount { get; protected set; } = 0;

        /// <summary>
        /// 可以更改TargetCount，但是CurrentCount不会清零，如需清零，需调用Reset
        /// </summary>
        public int TargetCount { get; set; }

        public Counter(string name, IBehaviorNode child,  int targetCount):base(name, child)
        {
            TargetCount = targetCount;
            CurrentCount = 0;
        }

        /// <summary>
        ///  重复运行Child TargetCount次
        /// </summary>
        /// <param name="duration"></param>
        /// <returns>未到达次数，返回Running,否则返回Success</returns>
        public override BehaviorTreeStatus Behave(DateTime duration)
        {
            Behave(duration);
            if (CurrentCount < TargetCount-1)
            {
                
                return BehaviorTreeStatus.Running;
            }
            else
            {
                return BehaviorTreeStatus.Success;
            }
        }

        public void Reset()
        {
            CurrentCount = 0;
        }
    }
}
