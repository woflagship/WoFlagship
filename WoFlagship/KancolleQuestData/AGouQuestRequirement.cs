using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleAI;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class AGouQuestRequirement : IQuestRequirement
    {
        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
