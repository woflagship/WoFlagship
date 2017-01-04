using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class EquipexchangeQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 装备要求
        /// </summary>
        public QuestRequiredItem[] Equipments { get; set; }

        /// <summary>
        /// 废弃要求
        /// </summary>
        public QuestRequiredItem[] Scraps { get; set; }

        /// <summary>
        /// 消耗
        /// </summary>
        public QuestRequiredItem[] Consumptions { get; set; }

        public int[] Resources { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }

   
}
