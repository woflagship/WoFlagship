using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
