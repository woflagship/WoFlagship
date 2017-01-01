using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleQuest
{
    [Serializable]
    public class KancolleQuestInfo : IMetadata
    {
        public int Version { get; set; } = 1;
        public string UpdateTime { get; set; } = "20161229";

        public QuestInfoItem[] QuestInfos { get; set; }
    }

}
