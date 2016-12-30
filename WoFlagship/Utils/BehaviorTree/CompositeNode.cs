using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public abstract class CompositeNode : IBehaviorNode, ICollection<IBehaviorNode>
    {
        public string Name { get; protected set; }

        public CompositeNode(string name)
        {
            Name = name;
        }

        protected List<IBehaviorNode> children = new List<IBehaviorNode>();

        public int Count
        {
            get
            {
                return children.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(IBehaviorNode item)
        {
            children.Add(item);
        }

        public void Clear()
        {
            children.Clear();
        }

        public bool Contains(IBehaviorNode item)
        {
            return children.Contains(item);
        }

        public void CopyTo(IBehaviorNode[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IBehaviorNode> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        public bool Remove(IBehaviorNode item)
        {
            return children.Remove(item);
        }

        public abstract BehaviorTreeStatus Behave(DateTime duration);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }
    }
}
