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
    public class FleetQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 要求的舰队，从1开始
        /// null表示无要求
        /// </summary>
        public int? FleetId { get; set; }

        /// <summary>
        /// 编成要求
        /// </summary>
        public ShipRequirement[] Groups { get; set; }

        /// <summary>
        /// 不可以的编成
        /// </summary>
        public int? Disallowed { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
