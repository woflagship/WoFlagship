using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleCore
{
    public interface IKancolleSceneRevceiver
    {
        void OnSceneUpdatedHandler(KancolleScene scene);
    }
}
