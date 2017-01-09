using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.ViewModels;

namespace WoFlagship.Plugins
{
    public interface IPlugin
    {
        int Version { get; }

        string Name { get; }

        string Description { get; }

        UserControl PluginPanel { get; }

        bool NewWindow { get; }

        void OnInit(GeneralViewModel generalViewModel);

        void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData);

        void OnGameDataUpdated(GeneralViewModel generalViewModel, KancolleCore.KancolleGameData gameData);

    }
}
