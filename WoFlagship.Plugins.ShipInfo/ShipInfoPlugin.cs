using System;
using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.ViewModels;

namespace WoFlagship.KancollePlugin.ShipInfo
{
    class ShipInfoPlugin : IKancollePlugin
    {
        private ShipInfoPanel panel = new ShipInfoPanel();

        public string Description
        {
            get
            {
                return "舰娘信息";
            }
        }

        public string Name
        {
            get
            {
                return "舰娘信息";
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
            Application.Current.Dispatcher.Invoke(new Action(() => panel.UpdateShipList(gameData)));
        }

        public void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => panel.UpdateAllShipList(gameData)));
        }

        public void OnInit(GeneralViewModel generalViewModel)
        {
           
        }
    }
}
