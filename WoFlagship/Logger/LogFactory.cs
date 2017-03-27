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

        //以下Logger用于系统内部使用
        internal static ILog SystemLogger { get { return _systemLogger; } }
        internal static ILog AILogger { get { return _aiLogger; } }

        //控制台输出，用于给用户最常用的输出
        public static ConsoleLogger ConsoleLogger { get; internal set; }

    }
}
