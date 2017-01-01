using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleQuest;
using WoFlagship.KancolleRequirement;
using WoFlagship.ViewModels;

namespace WoFlagship.KancolleMap
{
    [Serializable]
    public class MapInfoItem
    {
        public string Map { get; set; }
        public int MapId { get; set; }

        public Spot[] Spots { get; set; }

        public MapRoute[] Routes { get; set; }

    }

    [Serializable]
    public class MapRoute
    {
        public string From { get; set; }
        public string To { get; set; }
        public ShipCondition[] Conditions { get; set; }
    }

    [Serializable]
    public class Spot
    {
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>
    /// 路线分歧条件
    /// </summary>
    [Serializable]
    public class Condition : ViewModelBase
    {
        private ObservableCollection<RequiredShipType> _shipTypes = new ObservableCollection<RequiredShipType>();
        public ObservableCollection<RequiredShipType> ShipTypes { get { return _shipTypes; } set { _shipTypes = value; OnPropertyChanged(); } }

        private double _possibility = 1;
        public double Possibility { get { return _possibility; } set { _possibility = value; OnPropertyChanged(); } }
    }

    [Serializable]
    public class RequiredShipType : ViewModelBase
    {
        private int _typeId;
        public int TypeId { get { return _typeId; } set { _typeId = value; OnPropertyChanged(); } }

        private int _amount;
        public int Amount { get { return _amount; } set { _amount = value; OnPropertyChanged(); } }

        private int _rangeFrom;
        public int RangeFrom { get { return _rangeFrom; } set { _rangeFrom = value; OnPropertyChanged(); } }

        private int _rangeTo;
        public int RangeTo { get { return _rangeTo; } set { _rangeTo = value; OnPropertyChanged(); } }
    }
}
