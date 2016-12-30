using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCommon;
using WoFlagship.ViewModels;

namespace WoFlagship.Plugins.ShipInfo
{
    class ShipInfoPlugin : IPlugin
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

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            
        }

        public void OnDeckUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            
        }

        public void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => panel.UpdateAllShipList(gameData)));
        }

        public void OnInit(GeneralViewModel generalViewModel)
        {
           
        }

        public void OnMaterialUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            
        }

        public void OnQuestUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
           
        }

        public void OnShipUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            Application.Current.Dispatcher.Invoke(new Action(()=>panel.UpdateShipList(gameData)));
        }
    }
}
