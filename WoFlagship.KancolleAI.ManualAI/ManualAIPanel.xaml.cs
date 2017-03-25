using System;
using System.Collections.Generic;
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
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleAI.ManualAI
{
    /// <summary>
    /// ManualAIPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ManualAIPanel : UserControl
    {
        public event Action<KancolleTask> OnTaskGenerated;
        

        public ManualAIPanel()
        {
            InitializeComponent();
        }

        private KancolleGameData gameData = null;

        public void UpdateGameContext(KancolleGameData gameData)
        {
            this.gameData = gameData;
        }

        //编成
        private void Btn_AIManual_Organize_Click(object sender, RoutedEventArgs e)
        {
            int deck = int.Parse(Txt_AIManual_Organize_Deck.Text);
            int[] ships = new int[]
            {
                int.Parse(Txt_AIManual_Organize_Ship1.Text),
                int.Parse(Txt_AIManual_Organize_Ship2.Text),
                int.Parse(Txt_AIManual_Organize_Ship3.Text),
                int.Parse(Txt_AIManual_Organize_Ship4.Text),
                int.Parse(Txt_AIManual_Organize_Ship5.Text),
                int.Parse(Txt_AIManual_Organize_Ship6.Text),
            };
            OrganizeTask task = new OrganizeTask(deck - 1, ships);
            OnTaskGenerated?.Invoke(task);

        }

        //编成中的id变了之后，显示舰娘名
        private void Txt_AIManual_Organize_Ship_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Txt_AIManual_Organize_Ship1_name != null)
            {
                Txt_AIManual_Organize_Ship1_name.Text = getShipName(Txt_AIManual_Organize_Ship1);
                Txt_AIManual_Organize_Ship2_name.Text = getShipName(Txt_AIManual_Organize_Ship2);
                Txt_AIManual_Organize_Ship3_name.Text = getShipName(Txt_AIManual_Organize_Ship3);
                Txt_AIManual_Organize_Ship4_name.Text = getShipName(Txt_AIManual_Organize_Ship4);
                Txt_AIManual_Organize_Ship5_name.Text = getShipName(Txt_AIManual_Organize_Ship5);
                Txt_AIManual_Organize_Ship6_name.Text = getShipName(Txt_AIManual_Organize_Ship6);
            }
        }

        private string getShipName(TextBox tb)
        {
            int no;
            if (gameData != null && int.TryParse(tb.Text, out no))
            {
                string name = gameData.GetShipName(no);
                if (name == null)
                    return "未知";
                return name;
            }
            return "未知";
        }

        private void Btn_AIManual_Supply_Click(object sender, RoutedEventArgs e)
        {
            int supplyDeck = int.Parse(Txt_AIManual_Supply_Deck.Text);
            SupplyTask task = new SupplyTask(supplyDeck - 1);
            OnTaskGenerated?.Invoke(task);
        }

        private void Btn_AIManual_Map_Click(object sender, RoutedEventArgs e)
        {
            int area = int.Parse(Txt_AIManual_Map_Area.Text);
            int map = int.Parse(Txt_AIManual_Map_Map.Text);
            int deck = int.Parse(Txt_AIManual_Map_Deck.Text);
            MapTask task = new MapTask(deck - 1, area * 10 + map);
            OnTaskGenerated?.Invoke(task);
        }

        //阵型选择
        private void Btn_AIManual_Battle_Formation_Click(object sender, RoutedEventArgs e)
        {
            BattleFormationTask task = new BattleFormationTask(Cbx_AIManual_Battle_Formation.SelectedIndex + 1);
            OnTaskGenerated?.Invoke(task);
        }

        //进击
        private void Btn_AIManual_Battle_Next_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Next);
            OnTaskGenerated?.Invoke(task);
        }

        //回港
        private void Btn_AIManual_Battle_Return_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Return);
            OnTaskGenerated?.Invoke(task);
        }

        //撤退
        private void Btn_AIManual_Battle_Back_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Back);
            OnTaskGenerated?.Invoke(task);
        }

        //夜战
        private void Btn_AIManual_Battle_Night_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Night);
            OnTaskGenerated?.Invoke(task);
        }

        //跳过过场
        private void Btn_AIManual_Battle_Skip_Click(object sender, RoutedEventArgs e)
        {
            BattleSkipTask task = new BattleSkipTask();
            OnTaskGenerated?.Invoke(task);
        }

        //改装
        private void Btn_AIManual_Remodel_Click(object sender, RoutedEventArgs e)
        {
            int deck = int.Parse(Txt_AIManual_Remodel_Deck.Text);
            int position = int.Parse(Txt_AIManual_Remodel_Position.Text);
            int[] itemNos = new int[]
            {
                int.Parse(Txt_AIManual_Remodel_Item1.Text),
                int.Parse(Txt_AIManual_Remodel_Item2.Text),
                int.Parse(Txt_AIManual_Remodel_Item3.Text),
                int.Parse(Txt_AIManual_Remodel_Item4.Text),
            };
            RemodelTask task = new RemodelTask(deck - 1, position - 1, itemNos);
            OnTaskGenerated?.Invoke(task);
        }

        private void Txt_AIManual_Remodel_Item_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Txt_AIManual_Remodel_Item1_name != null)
            {
                Txt_AIManual_Remodel_Item1_name.Text = getItemName(Txt_AIManual_Remodel_Item1);
                Txt_AIManual_Remodel_Item2_name.Text = getItemName(Txt_AIManual_Remodel_Item2);
                Txt_AIManual_Remodel_Item3_name.Text = getItemName(Txt_AIManual_Remodel_Item3);
                Txt_AIManual_Remodel_Item4_name.Text = getItemName(Txt_AIManual_Remodel_Item4);
            }
        }

        //入渠
        private void Btn_AIManual_Repair_Click(object sender, RoutedEventArgs e)
        {
            int shipNo = int.Parse(Txt_AIManual_Repair_ShipNo.Text);
            int dock = int.Parse(Txt_AIManual_Repair_Dock.Text);
            RepairTask task = new RepairTask(shipNo, dock - 1, (bool)Chk_AIManual_Repair_UseFastRepair.IsChecked);
            OnTaskGenerated?.Invoke(task);
        }


        private void Txt_AIManual_Repair_ShipNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Txt_AIManual_Repair_Ship_name != null)
            {
                Txt_AIManual_Repair_Ship_name.Text = getShipName(Txt_AIManual_Repair_ShipNo);
            }
        }


        private string getItemName(TextBox tb)
        {
            int no;
            if (gameData!= null && int.TryParse(tb.Text, out no))
            {
                string name = gameData.GetSlotItemName(no);
                if (name == null)
                    return "未知";
                return name;
            }
            return "未知";
        }
    }
}
