using System;
using WoFlagship.KancolleCore;

namespace WoFlagship.KancolleMap
{
    [Serializable]
    public class KancolleMapInfo : IMetadata
    {
        public string UpdateTime { get; set; }

        public int Version { get; set; }

        public MapInfoItem[] MapInfos { get; set; }
    }
}
