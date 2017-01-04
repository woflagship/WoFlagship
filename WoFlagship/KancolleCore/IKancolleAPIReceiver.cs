namespace WoFlagship.KancolleCore
{
    public interface IKancolleAPIReceiver
    {
        void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api);
    }
}
