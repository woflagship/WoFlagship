using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WoFlagship.Logger;

namespace WoFlagship
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    MessageBox.Show("未处理的异常！可能由于当前使用的插件或者AI导致！\n" + exception.Message);
                    LogFactory.SystemLogger.Error("未处理异常", exception);
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLogger.Fatal("不可恢复的未处理异常", ex);
                MessageBox.Show("应用程序发生不可恢复的异常，将要退出！");
            }
        }
    }
}
