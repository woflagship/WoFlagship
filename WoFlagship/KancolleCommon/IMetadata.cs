using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCommon
{
    interface IMetadata
    {
        int Version { get; set; }

        string UpdateTime { get; set; }
    }
}
