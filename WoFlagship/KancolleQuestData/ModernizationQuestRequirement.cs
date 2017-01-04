using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class ModernizationQuestRequirement : IQuestRequirement
    {

        public int? Times { get; set; }

        public int? ShipType { get; set; }

        public int[] Resources { get; set; }

        public ModernizationQuestItem[] Consumptions { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ModernizationQuestItem
    {
        public int? ShipId { get; set; }
        public int Amount { get; set; }
    }
}
