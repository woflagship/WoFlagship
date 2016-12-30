using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.Utils.BehaviorTree
{
    public abstract class DecoratorNode : CompositeNode
    {
        public IBehaviorNode Child
        {
            get
            {
                if (children.Count > 0)
                    return children[0];
                else
                    return null;
            }

            set
            {
                children.Clear();
                if (value != null)
                    children.Add(value);
            }
        }

        public DecoratorNode(string name, IBehaviorNode child) : base(name)
        {
            Name = name;
            Child = child;
        }

      
    }
}
