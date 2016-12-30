using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.ViewModels
{
    class SettingViewModel : ViewModelBase
    {
        private WebSettingViewModel _webSetting;
        public WebSettingViewModel WebSetting { get { return _webSetting; } set { _webSetting = value; OnPropertyChanged(); } }
    }

    public enum ProxyTypes
    {
        NoProxy,
        Http
    }


    public class WebSettingViewModel : ViewModelBase
    {
        private string _proxyHost;
        public string ProxyHost { get { return _proxyHost; } set { _proxyHost = value; OnPropertyChanged(); } }

        private int _proxyPort;
        public int ProxyPort { get { return _proxyPort; } set { _proxyPort = value; OnPropertyChanged(); } }

        private ProxyTypes _proxyType;
        public ProxyTypes ProxyType { get { return _proxyType; } set { _proxyType = value; OnPropertyChanged(); } }
    }
}
