using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCommon
{
    public interface IKancolleGameDataReceiver
    {
        void OnGameDataUpdatedHandler(KancolleGameData gameData);
    }
}
