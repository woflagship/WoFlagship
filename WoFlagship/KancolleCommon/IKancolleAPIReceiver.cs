using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCommon
{
    public interface IKancolleAPIReceiver
    {
        void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api);
    }
}
