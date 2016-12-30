using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCommon;
using WoFlagship.ViewModels;

namespace WoFlagship.Plugins
{
    public interface IPlugin : IKancolleAPIReceiver
    {
        int Version { get; }

        string Name { get; }

        string Description { get; }

        UserControl PluginPanel { get; }

        bool NewWindow { get; }

        void OnInit(GeneralViewModel generalViewModel);

        void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData);

        void OnMaterialUpdated(GeneralViewModel generalViewModel, KancolleCommon.KancolleGameData gameData);

        void OnQuestUpdated(GeneralViewModel generalViewModel, KancolleCommon.KancolleGameData gameData);

        void OnShipUpdated(GeneralViewModel generalViewModel, KancolleCommon.KancolleGameData gameData);

        void OnDeckUpdated(GeneralViewModel generalViewModel, KancolleCommon.KancolleGameData gameData);
    }
}
