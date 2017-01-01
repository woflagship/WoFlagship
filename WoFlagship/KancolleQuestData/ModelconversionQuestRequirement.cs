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
    public class ModelconversionQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 装备
        /// </summary>
        public QuestRequiredItem[] Equipment { get; set; }

        /// <summary>
        /// 废弃的物品
        /// </summary>
        public QuestRequiredItem[] Scraps { get; set; }

        /// <summary>
        /// 秘书舰
        /// </summary>
        public int[] Secretary { get; set; }

        public bool? Fullyskilled { get; set; }

        public bool? Maxmodified { get; set; }

        public bool? UseSkilledCrew { get; set; }

        public QuestRequiredItem[] Consumptions { get; set; }

        public int[] Resources { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
