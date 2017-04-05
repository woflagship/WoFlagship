using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public class SelectorNode : CompositeNode
    {
        public SelectorNode(string name) : base(name) { }

        /// <summary>
        /// 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        /// 如遇到一个Child Node执行后返回True，那停止迭代，本Node向自己的Parent Node也返回True；
        /// 否则所有Child Node都返回False，那本Node向自己的Parent Node返回False。
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public override async Task<BehaviorTreeStatus> BehaveAsync()
        {
            foreach(var child in children)
            {
                var childStatus = await child.BehaveAsync();
                if (childStatus != BehaviorTreeStatus.Failure)
                    return childStatus;
            }

            return BehaviorTreeStatus.Failure;
        }
    }
}
