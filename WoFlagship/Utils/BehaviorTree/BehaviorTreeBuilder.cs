using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public class BehaviorTreeBuilder
    {
        private IBehaviorNode curNode = null;

        private Stack<CompositeNode> compositeNodeStack = new Stack<CompositeNode>();

        /// <summary>
        /// 创建一个ActionNode
        /// </summary>
        /// <param name="name">节点名</param>
        /// <param name="actionFunc">行为</param>
        /// <returns>创建的行为树</returns>
        public BehaviorTreeBuilder Do(string name, Func<DateTime, BehaviorTreeStatus> actionFunc)
        {
            if(compositeNodeStack.Count <=0)
            {
                throw new ApplicationException("ActionNode不能作为行为树的根节点");
            }

            var actionNode = new ActionNode(name, actionFunc);
            compositeNodeStack.Peek().Add(actionNode);
            return this;
        }

        public BehaviorTreeBuilder Invertor(string name)
        {
            var invertor = new Invertor(name, null);
            if (compositeNodeStack.Count > 0)
            {
                compositeNodeStack.Peek().Add(invertor);
            }
            compositeNodeStack.Push(invertor);
            return this;
        }
        /// <summary>
        /// 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        /// 如遇到一个Child Node执行后返回False，那停止迭代， 本Node向自己的Parent Node也返回False；
        /// 否则所有Child Node都返回True，那本Node向自己的Parent Node返回True。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Sequence(string name)
        {
            var sequence = new SequenceNode(name);
            if (compositeNodeStack.Count > 0)
            {
                compositeNodeStack.Peek().Add(sequence);
            }
            compositeNodeStack.Push(sequence);
            return this;
        }

        /// <summary>
        /// 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node：
        /// 如遇到一个Child Node执行后返回True，那停止迭代，本Node向自己的Parent Node也返回True；
        /// 否则所有Child Node都返回False，那本Node向自己的Parent Node返回False。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Selector(string name)
        {
            var selector = new SelectorNode(name);
            if (compositeNodeStack.Count > 0)
            {
                compositeNodeStack.Peek().Add(selector);
            }
            compositeNodeStack.Push(selector);
            return this;
        }

        public BehaviorTreeBuilder Condition(string name, Func<DateTime, bool> condition)
        {
            if (compositeNodeStack.Count <= 0)
            {
                throw new ApplicationException("ConditionNode不能作为行为树的根节点");
            }
            var conditionNode = new ConditionNode(name, condition);
            compositeNodeStack.Peek().Add(conditionNode);
            return this;
        }


        /// <summary>
        /// 一个序列的结束（例如Selector，Sequence，Invertor等）
        /// </summary>
        /// <param name="compositeNode">返回当前序列的节点</param>
        /// <returns></returns>
        public BehaviorTreeBuilder EndComposite(out IBehaviorNode compositeNode)
        {
            curNode = compositeNodeStack.Pop();
            compositeNode = curNode;
            return this;
        }

        /// <summary>
        /// 一个序列的结束（例如Selector，Sequence，Invertor等）
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeBuilder EndComposite()
        {
            curNode = compositeNodeStack.Pop();
            return this;
        }
    }
}
