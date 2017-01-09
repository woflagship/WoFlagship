using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WoFlagship.KancolleCore;
using WoFlagship.ViewModels;

namespace WoFlagship.Plugins.ItemInfo
{
    /// <summary>
    /// ItemInfoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ItemInfoPanel : UserControl
    {
        private ObservableCollection<ItemInfoPanelViewModel> ItemCollection = new ObservableCollection<ItemInfoPanelViewModel>();
        private KancolleGameData gameData;

        public ItemInfoPanel()
        {
            InitializeComponent();
        }

        private void Dg_Items_Loaded(object sender, RoutedEventArgs e)
        {
            Dg_Items.ItemsSource = ItemCollection;
        }

        private void Dg_All_Items_Loaded(object sender, RoutedEventArgs e)
        {

        }

       public void UpdateItemList(KancolleGameData gameData)
        {
            this.gameData = gameData;
            ItemCollection.Clear();

            //itemId - shipIds
            //一个装备名对应装备该装备的舰娘
            //没有被装备则不添加
            Dictionary<int, List<int>> belongDic = new Dictionary<int, List<int>>();
            foreach(var ownedShip in gameData.OwnedShipDictionary.Values)
            {
                if(ownedShip.Slot != null && ownedShip.Slot.Length>0)
                {
                    foreach (var itemNo in ownedShip.Slot)
                    {
                        if (itemNo > 0)
                        {
                            var itemId = gameData.OwnedSlotDictionary[itemNo].SlotItemId;
                            if (!belongDic.ContainsKey(itemId))
                                belongDic.Add(itemId, new List<int>());
                            belongDic[itemId].Add(ownedShip.ShipId);
                        }
                    }
                       
                }
            }

            //itemid-model
            Dictionary<int, ItemInfoPanelViewModel> itemDic = new Dictionary<int, ItemInfoPanelViewModel>();
            foreach(var item in gameData.OwnedSlotDictionary.Values)
            {            
                if(itemDic.ContainsKey(item.SlotItemId))
                {
                    itemDic[item.SlotItemId].TotalCount++;
                }
                else
                {
                    var itemData = gameData.SlotDictionary[item.SlotItemId];
                    var model = new ItemInfoPanelViewModel()
                    {
                        SlotItemId = item.SlotItemId,
                        Name = itemData.Name,
                        TotalCount = 1
                    };
                    if (belongDic.ContainsKey(item.SlotItemId))
                    {
                        var belongShips = belongDic[item.SlotItemId];
                        var groupedShips = from s in belongShips
                                           group s by s into g
                                           select new { shipId = g.Key, belongCount = g.Count() };
                        foreach (var gship in groupedShips)
                        {
                            if (gship.belongCount == 1)
                                model.BelongShips.Add(gameData.ShipDataDictionary[gship.shipId].Name);
                            else
                                model.BelongShips.Add(gameData.ShipDataDictionary[gship.shipId].Name + "*" + gship.belongCount);

                        }

                    }
                    ItemCollection.Add(model);
                    itemDic.Add(model.SlotItemId, model);
                }
                
            }
        }
    }

    class ItemInfoPanelViewModel : ViewModelBase
    {

        private int _slotItemId;
        /// <summary>
        /// 对应到装备数据库的id
        /// </summary>
        public int SlotItemId { get { return _slotItemId; } set { _slotItemId = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private int _totalCount;
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get { return _totalCount; } set { _totalCount = value; OnPropertyChanged(); } }

        private int _remainCount;
        /// <summary>
        /// 剩余数量，即为装备数量
        /// </summary>
        public int RemainCount { get { return _remainCount; } set { _remainCount = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _belongShips = new ObservableCollection<string>();
        /// <summary>
        /// 所属舰娘
        /// </summary>
        public ObservableCollection<string> BelongShips { get { return _belongShips; } set { _belongShips = value;  OnPropertyChanged(); } }

    }

}
