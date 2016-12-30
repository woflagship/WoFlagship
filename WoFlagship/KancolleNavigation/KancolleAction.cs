using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WoFlagship.KancolleNavigation
{
    public enum ActionTypes
    {
        Click,
        Move
    }

    

    public class KancolleAction
    {
        public ActionTypes ActionType { get; set; }
        public Point ActionPosition { get; set; }


        public KancolleAction(ActionTypes actionType, Point actionPosition)
        {
            ActionType = actionType;
            ActionPosition = actionPosition;
        }

        public KancolleAction(Point actionPosition)
        {
            ActionType = ActionTypes.Click;
            ActionPosition = actionPosition;
        }
    }
}
