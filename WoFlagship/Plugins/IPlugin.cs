using System.Windows.Controls;
using WoFlagship.KancolleCore;
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

        void OnMaterialUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);

        void OnQuestUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);

        void OnShipUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);

        void OnDeckUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);
    }
}
