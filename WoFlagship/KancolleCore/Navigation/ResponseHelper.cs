using System;

namespace WoFlagship.KancolleCore.Navigation
{
    class ResponseHelper
    {
        public ResponseHelper(RequestInfo requestInfo, string response, string api)
        {
            RequestInfo = requestInfo;
            Response = response;
            API = api;
            Time = DateTime.Now;
        }
        public RequestInfo RequestInfo { get; set; }
        public string Response { get; set; }
        public string API { get; set; }
        public DateTime Time { get;  }
    }
}
