using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
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
