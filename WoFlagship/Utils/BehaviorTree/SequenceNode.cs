using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.Utils.BehaviorTree;

namespace WoFlagship.Utils.BehaviorTree
{
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(string name) : base(name) { }

        /// <summary>
        /// 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        /// 如遇到一个Child Node执行后返回False，那停止迭代， 本Node向自己的Parent Node也返回False；
        /// 否则所有Child Node都返回True，那本Node向自己的Parent Node返回True。
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public override async Task<BehaviorTreeStatus> BehaveAsync()
        {
            foreach(var child in children)
            {
                var childStaus = await child.BehaveAsync();
                if (childStaus != BehaviorTreeStatus.Success)
                    return childStaus;
            }
            return BehaviorTreeStatus.Success;
        }
    }
}
