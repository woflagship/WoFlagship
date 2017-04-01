using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using WoFlagship.KancolleMap;
using WoFlagship.KancolleRequirement;

namespace WoFlagship.ViewModels
{
    class MapEditorViewModel : ViewModelBase
    {
        private ObservableCollection<MapInfoItemViewModel> _mapInfos = new ObservableCollection<MapInfoItemViewModel>();
        public ObservableCollection<MapInfoItemViewModel> MapInfos { get { return _mapInfos; } set { _mapInfos = value; OnPropertyChanged(); } }

        private MapInfoItemViewModel _selectedMap;
        public MapInfoItemViewModel SelectedMap { get { return _selectedMap; } set { _selectedMap = value; OnPropertyChanged(); } }

        public static List<string> LinqItems = new List<string>();

        static MapEditorViewModel()
        {
            PropertyInfo[] properties = typeof(KancolleCore.KancolleShip).GetProperties(BindingFlags.Instance|BindingFlags.Public);
            foreach(var pi in properties)
            {
                LinqItems.Add("s." + pi.Name);
            }
        }

    }

    class MapInfoItemViewModel : ViewModelBase
    {
        private MapInfoItem infoItem;

        public string Map { get { return infoItem.Map; } set { infoItem.Map = value; OnPropertyChanged(); } }

        private ObservableCollection<MapRouteViewModel> _routes = new ObservableCollection<MapRouteViewModel>();
        public ObservableCollection<MapRouteViewModel> Routes { get { return _routes; } set { _routes = value; OnPropertyChanged(); } }

        private MapRouteViewModel _selectedRoute;
        public MapRouteViewModel SelectedRoute { get { return _selectedRoute; } set { _selectedRoute = value; OnPropertyChanged(); } }

        public MapInfoItemViewModel(MapInfoItem infoItem)
        {
            this.infoItem = infoItem;
            if(infoItem.Routes != null)
            {
                Routes.Clear();
                foreach(var r in infoItem.Routes)
                {
                    Routes.Add(new MapRouteViewModel(r));
                }
            }
        }
    }

    class MapRouteViewModel : ViewModelBase
    {
        private MapRoute mapRoute;

        private string _routeName;
        public string RouteName { get { return _routeName; } }

        private ObservableCollection<ShipConditionViewModel> _conditions = new ObservableCollection<ShipConditionViewModel>();
        public ObservableCollection<ShipConditionViewModel> Conditions { get { return _conditions; } set { _conditions = value; OnPropertyChanged(); } }

        private ShipConditionViewModel _selectedCondition;
        public ShipConditionViewModel SelectedCondition { get { return _selectedCondition; } set { _selectedCondition = value; OnPropertyChanged(); } }


        public MapRouteViewModel(MapRoute mapRoute)
        {
            this.mapRoute = mapRoute;
            _routeName = mapRoute.From + " -> " + mapRoute.To;
            Conditions.Clear();
            if (mapRoute.Conditions != null)
            {
                foreach(var c in mapRoute.Conditions)
                {
                    Conditions.Add(new ShipConditionViewModel(c));
                }
            }
            Conditions.CollectionChanged += Conditions_CollectionChanged;
        }

        private void Conditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            List<ShipCondition> conditions = new List<ShipCondition>();
            foreach(var c in Conditions)
            {
                conditions.Add(c.ShipCondition);
            }
            mapRoute.Conditions = conditions.ToArray();
        }
    }

    class ShipConditionViewModel : ViewModelBase
    {
        public ShipCondition ShipCondition { get; private set; }

        private ObservableCollection<ShipConstraintViewModel> _constraints = new ObservableCollection<ShipConstraintViewModel>();
        public ObservableCollection<ShipConstraintViewModel> Constraints { get { return _constraints; } set { _constraints = value; OnPropertyChanged(); } }

        private ShipConstraintViewModel _selectedConstraint;
        public ShipConstraintViewModel SelectedConstraint { get { return _selectedConstraint; } set { _selectedConstraint = value; OnPropertyChanged(); } }


        public double Possibility { get { return ShipCondition.Possibility; } set { ShipCondition.Possibility = value; OnPropertyChanged(); } }

        public ShipConditionViewModel()
        {
            ShipCondition = new ShipCondition();
            Constraints.CollectionChanged += Constraints_CollectionChanged;
        }

        public ShipConditionViewModel(ShipCondition shipCondition)
        {
            ShipCondition = shipCondition;
            Constraints.Clear();
            if(shipCondition.Constraints != null)
            {
                foreach(var c in shipCondition.Constraints)
                {
                    Constraints.Add(new ShipConstraintViewModel(c));
                }
            }
            Constraints.CollectionChanged += Constraints_CollectionChanged;
        }

        private void Constraints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            List<ShipConstraint> constraints = new List<ShipConstraint>();
            foreach (var c in Constraints)
            {
                constraints.Add(c.ShipConstraint);
            }
            ShipCondition.Constraints = constraints.ToArray();
           
        }
        
    }

    class ShipConstraintViewModel : ViewModelBase
    {
        public ShipConstraint ShipConstraint { get; private set; }

        public string ConstraintLinq { get { return ShipConstraint.ConstraintLinq; } set { ShipConstraint.ConstraintLinq = value; OnPropertyChanged(); } }
        public int Amount { get { return ShipConstraint.Amount; } set { ShipConstraint.Amount = value; OnPropertyChanged(); } }
        public int From { get { return ShipConstraint.Selection.Item1; } set { ShipConstraint.Selection = new Tuple<int, int>(value, ShipConstraint.Selection.Item2); OnPropertyChanged(); } }
        public int To { get { return ShipConstraint.Selection.Item2; } set { ShipConstraint.Selection = new Tuple<int, int>(ShipConstraint.Selection.Item1, value); OnPropertyChanged(); } }

        public IEnumerable<string> LinqItems { get { return MapEditorViewModel.LinqItems; } }

        public ShipConstraintViewModel()
        {
            ShipConstraint = new ShipConstraint();
        }

        public ShipConstraintViewModel(ShipConstraint shipConstraint)
        {
            ShipConstraint = shipConstraint;
            if (ShipConstraint.Selection == null)
                ShipConstraint.Selection = new Tuple<int, int>(-1, -1);
        }


       
    }

}
