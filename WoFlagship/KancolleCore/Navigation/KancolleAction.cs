using System.Windows;

namespace WoFlagship.KancolleCore.Navigation
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
