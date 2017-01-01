using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoFlagship.KancolleAI;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleQuestData
{
    public interface IQuestRequirement
    {
       
        /// <summary>
        /// 完成一次该任务所需的Tasks
        /// 如果返回null，表示无法实现该任务
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        List<KancolleTask> GetTasks(KancolleGameData gameData);
    }

    [Serializable]
    public class QuestRequiredItem
    {
        public int SlotId { get; set; }

        /// <summary>
        /// 装备名
        /// </summary>
        public string SlotName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { get; set; }
    }

   
}
