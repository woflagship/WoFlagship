using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;


namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class ScrapequipmentQuestRequirement : IQuestRequirement
    {
        public QuestRequiredItem[] List { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
