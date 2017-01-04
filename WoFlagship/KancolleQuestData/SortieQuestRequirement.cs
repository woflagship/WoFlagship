using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class SortieQuestRequirement : IQuestRequirement
    {
        /// <summary>
        /// 可出击海域地图id
        /// </summary>
        public int[] Maps { get; set; }

        public string MapStr { get; set; }

        /// <summary>
        /// 是否需要boss战
        /// </summary>
        public bool Boss { get; set; }

        /// <summary>
        /// 结果要求
        /// null表示无要求
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 需要完成的次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 要求的舰队，从1开始
        /// 0表示无要求
        /// </summary>
        public int FleetId { get; set; }

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
            return null;
        }

        private MapTask GetMapTask()
        {
            return null;
        }

        private OrganizeTask GetOrganizeTask()
        {
            if(Groups == null)//表示
            {

            }
            return null;
        }
    }
}
