using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

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
