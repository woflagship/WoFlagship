using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCore.Navigation;


namespace WoFlagship
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
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
            OrganizeTask task = new OrganizeTask(deck-1, ships);
            taskExecutor.EnqueueTask(task);

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
            if(int.TryParse(tb.Text, out no))
            {
                string name = gameContext.GameData.GetShipName(no);
                if (name == null)
                    return "未知";
                return name;
            }
            return "未知";
        }

        private void Btn_AIManual_Supply_Click(object sender, RoutedEventArgs e)
        {
            int supplyDeck = int.Parse(Txt_AIManual_Supply_Deck.Text);
            SupplyTask task = new SupplyTask(supplyDeck-1);
            taskExecutor.EnqueueTask(task);
        }

        private void Btn_AIManual_Map_Click(object sender, RoutedEventArgs e)
        {
            int area = int.Parse(Txt_AIManual_Map_Area.Text);
            int map = int.Parse(Txt_AIManual_Map_Map.Text);
            int deck = int.Parse(Txt_AIManual_Map_Deck.Text);
            MapTask task = new MapTask(deck-1, area * 10 + map);
            taskExecutor.EnqueueTask(task);
        }

        //进击
        private void Btn_AIManual_Battle_Next_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Next);
            taskExecutor.EnqueueTask(task);
        }

        //回港
        private void Btn_AIManual_Battle_Return_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Return);
            taskExecutor.EnqueueTask(task);
        }

        //撤退
        private void Btn_AIManual_Battle_Back_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Back);
            taskExecutor.EnqueueTask(task);
        }

        //夜战
        private void Btn_AIManual_Battle_Night_Click(object sender, RoutedEventArgs e)
        {
            BattleChoiceTask task = new BattleChoiceTask(BattleChoiceTask.BattleChoices.Night);
            taskExecutor.EnqueueTask(task);
        }
    }
}
