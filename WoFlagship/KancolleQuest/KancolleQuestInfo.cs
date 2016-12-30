using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleQuest
{
    [Serializable]
    public class KancolleQuestInfo
    {
        public int Version { get; set; } = 1;
        public string UpdateTime { get; set; } = "20161229";

        public QuestInfoViewModel[] QuestInfos { get; set; }
    }

}
