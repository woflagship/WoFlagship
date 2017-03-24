namespace WoFlagship.ViewModels
{
    public class GeneralViewModel : ViewModelBase
    {
        public GeneralViewModel()
        {
            for (int i = 0; i < QuestList.Length; i++)
            {
                QuestList[i] = new QuestViewModel();
                QuestList[i].Reset();
            }

            for (int i = 0; i < Decks.Length; i++)
            {
                Decks[i] = new DeckViewModel();
                Decks[i].Name = (i + 1) + "";
            }

            for(int i=0; i<Docks.Length; i++)
            {
                Docks[i] = new DockViewModel();
                Docks[i].ShipName = "未解锁";
            }

        }

        private string _level;
        public string Level { get { return _level; } set { _level = value; OnPropertyChanged(); } }

        private string _rank;
        public string Rank { get { return _rank; } set { _rank = value; OnPropertyChanged(); } }

        private string _nickName;
        public string NickName { get { return _nickName; } set { _nickName = value; OnPropertyChanged(); } }

        private int _shipCount;
        public int ShipCount { get { return _shipCount; } set { _shipCount = value; OnPropertyChanged(); } }

        private int _itemCount;
        public int ItemCount { get { return _itemCount; } set { _itemCount = value; OnPropertyChanged(); } }

        private int _maxshipCount;
        public int MaxShipCount { get { return _maxshipCount; } set { _maxshipCount = value; OnPropertyChanged(); } }

        private int _maxitemCount;
        public int MaxItemCount { get { return _maxitemCount; } set { _maxitemCount = value; OnPropertyChanged(); } }


        private int _ran;
        public int Ran { get { return _ran; } set { _ran = value; OnPropertyChanged(); } }

        private int _dan;
        public int Dan { get { return _dan; } set { _dan = value; OnPropertyChanged(); } }

        private int _gang;
        public int Gang { get { return _gang; } set { _gang = value; OnPropertyChanged(); } }

        private int _lv;
        public int Lv { get { return _lv; } set { _lv = value; OnPropertyChanged(); } }

        private int _gaixiu;
        public int Gaixiu { get { return _gaixiu; } set { _gaixiu = value; OnPropertyChanged(); } }

        private int _kaifa;
        public int Kaifa { get { return _kaifa; } set { _kaifa = value; OnPropertyChanged(); } }

        private int _xiufu;
        public int Xiufu { get { return _xiufu; } set { _xiufu = value; OnPropertyChanged(); } }

        private int _jianzao;
        public int Jianzao { get { return _jianzao; } set { _jianzao = value; OnPropertyChanged(); } }

        private QuestViewModel[] _questList = new QuestViewModel[6];
        public QuestViewModel[] QuestList { get { return _questList; } set { _questList = value; OnPropertyChanged(); } }

        private DeckViewModel[] _decks = new DeckViewModel[4];
        public DeckViewModel[] Decks { get { return _decks; } set { _decks = value; OnPropertyChanged(); } }

        private DockViewModel[] _docks = new DockViewModel[4];
        public DockViewModel[] Docks { get{ return _docks; } set{_docks=value; OnPropertyChanged(); } }
    }

    public class QuestViewModel : ViewModelBase
    {
        private string _name = "未设置";
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _detail = "未设置";
        public string Detail { get { return _detail; } set { _detail = value; OnPropertyChanged(); } }

        private int _id = int.MaxValue;
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private int _state;
        public int State { get { return _state; } set { _state = value; OnPropertyChanged(); } }

        private int _progressFlag;
        public int ProgressFlag
        {
            get { return _progressFlag; }
            set
            {
                _progressFlag = value;
                if (State > 1)
                {
                    if (_progressFlag == 3)
                        _progress = "完成";
                    else
                    {
                        if (_progressFlag == 0)
                            _progress = "进行中";
                        else
                            _progress = (30 * _progressFlag + 20) + "%";
                    }
                }
                else
                {
                    _progress = "";
                }
                OnPropertyChanged();
                OnPropertyChanged("Progress");
            }
        }

        private string _progress;
        public string Progress { get { return _progress; }}

        public void Reset()
        {
            Name = "未设置";
            Detail = "未设置";
            State = 1;      
            Id = int.MaxValue;
        }

    }

    public class DeckViewModel : ViewModelBase
    {
        public DeckViewModel()
        {
            for(int i=0; i<Ships.Length; i++)
            {
                Ships[i] = new DeckShipViewModel();
                Ships[i].Reset();
            }
        }

        private string _id;
        public string Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private string _name = "";
        public string Name { get { return _name; } set { _name = value;OnPropertyChanged(); } }

        private DeckShipViewModel[] _ships = new DeckShipViewModel[6];
        public DeckShipViewModel[] Ships { get { return _ships; } set { _ships = value; OnPropertyChanged(); } }
    }

    public class DeckShipViewModel : ViewModelBase
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private int _id;
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private string[] _slots;
        public string[] Slots
        {
            get { return _slots; }
            set
            {
                _slots = value; OnPropertyChanged();
                SlotsStr = "";
                if (_slots != null)
                {   
                    foreach(var s in _slots)
                    {
                        SlotsStr += s + "; "; 
                    }
                }
                OnPropertyChanged("SlotsStr");
            }
        }

        public string SlotsStr { get; private set; }

        public void Reset()
        {
            Name = "空";
            Slots = null;
        }

        public DeckShipViewModel Clone()
        {
            DeckShipViewModel ship = new DeckShipViewModel();
            ship.Name = this.Name;
            ship.Id = Id;
            ship.Slots = Slots;
            return ship;
        }
    }

    public class DockViewModel : ViewModelBase
    {
        private string _ShipName;
        public string ShipName { get { return _ShipName; } set { _ShipName = value; OnPropertyChanged(); } }

        private string _CompleteTime;
        public string CompleteTime { get { return _CompleteTime; } set { _CompleteTime = value; OnPropertyChanged(); } }

        private string _RemainingTime;
        public string RemainingTime { get { return _RemainingTime; } set { _RemainingTime = value; OnPropertyChanged(); } }

        public void Lock()
        {
            ShipName = "未解锁";
            CompleteTime = "";
            RemainingTime = "";
        }

        public void Empty()
        {
            ShipName = "空闲";
            CompleteTime = "";
            RemainingTime = "";
        }
    }


}
