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

namespace WoFlagship.KancolleAI.SimpleAI
{
    /// <summary>
    /// SimpleAIPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleAIPanel : UserControl
    {
        public bool AutoRepair
        {
            get { return Dispatcher.Invoke<bool>(()=>(bool)Chk_AutoRepair.IsChecked); }
        }

        public bool AutoBattle
        {
            get { return Dispatcher.Invoke(() => (bool)Chk_AutoBattle.IsChecked); }
        }

        public int BattleMap
        {
            get
            {
                return Dispatcher.Invoke(() =>
                {
                    int i1, i2;
                    if (int.TryParse(Txt_BattleMap1.Text, out i1) && int.TryParse(Txt_BattleMap2.Text, out i2))
                    {
                        return i1 * 10 + i2;
                    }
                    else
                        return -1;
                });
            }
        }

        public void SetCurrentScene(KancolleScene currentScene)
        {
            Dispatcher.Invoke(() =>
            {
                Txt_CurrentScene.Text = currentScene.SceneType + " " + currentScene.SceneState;
            });
        }


        public void UpdateGameData(KancolleGameData gameData)
        {
            string dockStr = "";
            foreach(var dock in gameData.DockArray)
            {
                if (dock.State < 0)
                    dockStr += "【未解锁】";
                else if (dock.State == 0)
                    dockStr += "【空】 ";
                else
                    dockStr += $"【{gameData.GetShipName(dock.ShipId)}】 ";
              
            }
            Txt_CurrentRepair.Text = dockStr;
        }

        public SimpleAIPanel()
        {
            InitializeComponent();

        }
    }
}
