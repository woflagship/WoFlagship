using log4net;
using System.IO;

namespace WoFlagship.Logger
{
    class LogFactory
    {
        static LogFactory()
        {
            FileInfo configFile = new FileInfo("Log4Net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
        }

        private readonly static ILog _systemLogger = LogManager.GetLogger("SystemLogger");
        private readonly static ILog _aiLogger = LogManager.GetLogger("AILogger");

        public static ILog SystemLogger { get { return _systemLogger; } }
        public static ILog AILogger { get { return _aiLogger; } }

    }
}
