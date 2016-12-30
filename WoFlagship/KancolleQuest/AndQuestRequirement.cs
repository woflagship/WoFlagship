using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleAI;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleQuest
{
    [Serializable]
    public class AndQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 需要完成的出击
        /// </summary>
        public IQuestRequirement[] List { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
