using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using WoFlagship.KancolleAI;
using WoFlagship.KancolleNavigation;
using WoFlagship.Logger;
using WoFlagship.Plugins;
using WoFlagship.ToolWindows;
using WoFlagship.ViewModels;


namespace WoFlagship
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
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
            OrganizeTask task = new OrganizeTask(deck, ships);
            taskExecutor.EnqueueTask(task);

        }

        private void Btn_AIManual_Supply_Click(object sender, RoutedEventArgs e)
        {
            int supplyDeck = int.Parse(Txt_AIManual_Supply_Deck.Text);
            SupplyTask task = new SupplyTask(0);
            taskExecutor.EnqueueTask(task);
        }

        private void Btn_AIManual_Map_Click(object sender, RoutedEventArgs e)
        {
            int area = int.Parse(Txt_AIManual_Map_Area.Text);
            int map = int.Parse(Txt_AIManual_Map_Map.Text);
            int deck = int.Parse(Txt_AIManual_Map_Deck.Text);
            MapTask task = new MapTask(deck, area * 10 + map);
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
