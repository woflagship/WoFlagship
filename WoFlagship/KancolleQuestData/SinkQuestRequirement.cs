using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore.Navigation;
using WoFlagship.KancolleCore;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public enum EnemySinkType
    {
        Unknown,

        /// <summary>
        /// 敵潜水艦
        /// </summary>
        SS,

        /// <summary>
        /// 敵空母
        /// </summary>
        CV,

        /// <summary>
        /// 敵補給艦
        /// </summary>
        Supply,
    }

    [Serializable]
    public class SinkQuestRequirement : IQuestRequirement
    {
        public EnemySinkType ShipType { get; set; }

        public int Amount { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
