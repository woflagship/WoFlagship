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
using WoFlagship.KancolleCommon;
using WoFlagship.Utils;
using WoFlagship.ViewModels;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;

namespace WoFlagship.Plugins.ShipInfo
{
    /// <summary>
    /// ShipInfoPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ShipInfoPanel : UserControl
    {
        private ObservableCollection<ShipViewModel> ShipCollection { get; set; } = new ObservableCollection<ShipViewModel>();
        private ObservableCollection<ShipViewModel> AllShipCollection { get; set; } = new ObservableCollection<ShipViewModel>();

        private SortDescriptionCollection ShipSort = new SortDescriptionCollection();
        private SortDescriptionCollection AllShipSort = new SortDescriptionCollection();

        private KancolleGameData gameData;

        public ShipInfoPanel()
        {
            InitializeComponent();
            
        }

        public void UpdateAllShipList(KancolleGameData gameData)
        {
            this.gameData = gameData;
            AllShipCollection.Clear();
            foreach (var ship in gameData.ShipDic.Values)
            {
                var model = new ShipViewModel()
                {
                    Name = ship.api_name,
                    Id = ship.api_id,
                   
                    TypeId = ship.api_stype,
                    Type = ship.api_stype > KancolleAPIs.ShipTypeText.Length ? ship.api_stype + "" : KancolleAPIs.ShipTypeText[ship.api_stype - 1],
                };

                AllShipCollection.Add(model);
            }
        }

        public void UpdateShipList(KancolleGameData gameData)
        {
            this.gameData = gameData;
            ShipCollection.Clear();
            foreach(var ship in gameData.OwnedShipDic.Values)
            {
                var mst_ship = gameData.ShipDic[ship.api_ship_id];
                var model = new ShipViewModel()
                {
                    Name = mst_ship.api_name,
                    Level = ship.api_lv.ToString(),
                    Id = ship.api_id,
                    ShipId = ship.api_ship_id,
                    TypeId = mst_ship.api_stype,
                    Type = mst_ship.api_stype >= KancolleAPIs.ShipTypeText.Length ? mst_ship.api_stype + "" : KancolleAPIs.ShipTypeText[mst_ship.api_stype - 1],
                    Condition = ship.api_cond,
                    Karyoku = ship.api_karyoku[0],
                    Raisou = ship.api_raisou[0],
                    Taiku = ship.api_taiku[0],
                    Soukou = ship.api_soukou[0],
                    Kaihi = ship.api_kaihi[0],
                    Taisen = ship.api_taisen[0],
                    Sakuteki = ship.api_sakuteki[0],
                    Lucky = ship.api_lucky[0],
                    SlotIds = ship.api_slot
                };

                model.Slots = "";
                for(int i=0; i<model.SlotIds.Length; i++)
                {
                    if(model.SlotIds[i] != -1)
                    {
                        api_slot_item_item slotItem;
                        if(gameData.OwnedSlotDic.TryGetValue(model.SlotIds[i], out slotItem))
                        {
                            model.Slots += gameData.SlotDic[slotItem.api_slotitem_id].api_name + " ";
                        }
                    }
                }

                ShipCollection.Add(model);
            }
        }

        private void Dg_Ships_Loaded(object sender, RoutedEventArgs e)
        {
            Dg_Ships.ItemsSource = ShipCollection;
            Dg_Ships.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
        }

        private void Dg_All_Ships_Loaded(object sender, RoutedEventArgs e)
        {
            Dg_All_Ships.ItemsSource = AllShipCollection;
            Dg_All_Ships.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
        }

        private void Btn_SaveOwned_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json文件(*.json)|*.json";
            if(sfd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(sw, ShipCollection.ToArray());
                }
            }
        }
    }

    public class ShipViewModel : ViewModelBase
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _level;
        public string Level { get { return _level; } set { _level = value; OnPropertyChanged(); } }

        private int _id;
        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private int _shipId;
        public int ShipId { get { return _shipId; } set { _shipId = value; OnPropertyChanged(); } }

        private int _typeId;
        public int TypeId { get { return _typeId; } set { _typeId = value; OnPropertyChanged(); } }

        private string _type;
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged(); } }

        //疲劳值
        private int _condition;
        public int Condition { get { return _condition; } set { _condition = value; OnPropertyChanged(); } }

        //火力
        private int _karyoku;
        public int Karyoku { get { return _karyoku; } set { _karyoku = value; OnPropertyChanged(); } }

        //雷装
        private int _raisou;
        public int Raisou { get { return _raisou; } set { _raisou = value; OnPropertyChanged(); } }

        //对空
        private int _taiku;
        public int Taiku { get { return _taiku; } set { _taiku = value; OnPropertyChanged(); } }

        //装甲
        private int _soukou;
        public int Soukou { get { return _soukou; } set { _soukou = value; OnPropertyChanged(); } }

        //回避
        private int _kaihi;
        public int Kaihi { get { return _kaihi; } set { _kaihi = value; OnPropertyChanged(); } }

        //对潜
        private int _taisen;
        public int Taisen { get { return _taisen; } set { _taisen = value; OnPropertyChanged(); } }

        //索敌
        private int _sakuteki;
        public int Sakuteki { get { return _sakuteki; } set { _sakuteki = value; OnPropertyChanged(); } }

        //运
        private int _lucky;
        public int Lucky { get { return _lucky; } set { _lucky = value; OnPropertyChanged(); } }

        private int[] _slotIds;
        public int[] SlotIds { get { return _slotIds; } set { _slotIds = value; OnPropertyChanged(); } }

        public string _slots;
        public string Slots { get { return _slots; } set { _slots = value; OnPropertyChanged(); } }
    }
}
