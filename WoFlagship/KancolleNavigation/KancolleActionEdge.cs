using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleNavigation
{
    public class KancolleActionEdge : IEdge<SceneTypes>
    {
        private SceneTypes _source;
        private SceneTypes _target;
        public KancolleAction Action { get; set; }

        public SceneTypes Source
        {
            get
            {
                return _source;
            }
        }

        public SceneTypes Target
        {
            get
            {
                return _target;
            }
        }

        public KancolleActionEdge(SceneTypes from, SceneTypes to, KancolleAction action)
        {
            this._source = from;
            this._target = to;
            this.Action = action;
        }
    }
}
