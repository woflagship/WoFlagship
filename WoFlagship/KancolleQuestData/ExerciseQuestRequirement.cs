using System;
using System.Collections.Generic;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class ExerciseQuestRequirement : IQuestRequirement
    {

        /// <summary>
        /// 需要完成的次数
        /// null也表示1次
        /// </summary>
        public int? Times { get; set; }

        /// <summary>
        /// 是否需要胜利
        /// null表示无要求
        /// </summary>
        public bool? Victory { get; set; }

        /// <summary>
        /// 是否为日常任务
        /// </summary>
        public bool? Daily { get; set; }

        public List<KancolleTask> GetTasks(KancolleGameData gameData)
        {
            throw new NotImplementedException();
        }
    }
}
