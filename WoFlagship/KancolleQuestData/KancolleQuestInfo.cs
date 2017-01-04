using System;
using WoFlagship.KancolleCore;

namespace WoFlagship.KancolleQuestData
{
    [Serializable]
    public class KancolleQuestInfoMetadata : IMetadata
    {
        public int Version { get; set; } = 1;
        public string UpdateTime { get; set; } = "20161229";

        public KancolleQuestInfoItem[] QuestInfos { get; set; }
    }

}
