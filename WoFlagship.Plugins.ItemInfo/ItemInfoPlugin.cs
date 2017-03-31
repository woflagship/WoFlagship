using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.ViewModels;

namespace WoFlagship.KancollePlugin.ItemInfo
{
    public class ItemInfoPlugin : IKancollePlugin
    {
        private ItemInfoPanel panel = new ItemInfoPanel();

        public string Description
        {
            get
            {
                return "装备信息插件";
            }
        }

        public string Name
        {
            get
            {
                return "装备信息插件";
            }
        }

        public bool NewWindow
        {
            get
            {
                return true;
            }
        }

        public UserControl PluginPanel
        {
            get
            {
                return panel;
            }
        }

        public int Version
        {
            get
            {
                return 1;
            }
        }

        public void OnGameDataUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => panel.UpdateItemList(gameData)));
        }

        public void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => panel.UpdateItemList(gameData)));
        }

        public void OnInit(GeneralViewModel generalViewModel)
        {
           
        }
    }
}
