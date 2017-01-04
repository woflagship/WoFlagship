using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class ExpeditionQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 远征任务和次数
        /// </summary>
        public ExpeditionObject[] Objects { get; set; }

        /// <summary>
        /// 奖励资源？
        /// </summary>
        public int[] Resources { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ExpeditionObject
    {
        /// <summary>
        /// 远征次数
        /// </summary>
        public int? Times { get; set; }

        /// <summary>
        /// 远征id
        /// </summary>
        public int[] Id { get; set; }
    }
}
