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
