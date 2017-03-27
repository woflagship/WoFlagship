using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WoFlagship.Logger
{
    public class ConsoleLogger
    {
        private readonly static ILog _consoleLogger = LogManager.GetLogger("ConsoleLogger");

        private TextBlock textBlock = null;

        internal ConsoleLogger(TextBlock tb)
        {
            Debug.Assert(tb != null);
            textBlock = tb;
        }

        public void WriteLine(string message, Brush foreground)
        {
            Write(message + "\n", foreground);
        }

        public void WriteLine(string message)
        {
            Write(message + "\n");
        }

        public void Write(string message)
        {
            WriteToTextBlock(message, Brushes.Black);
        }

        public void Write(string message, Brush foreground)
        {
            WriteToTextBlock(message, Brushes.Black);
        }

        private void WriteToTextBlock(string message, Brush foreground)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Run line = new Run(message);
                line.Foreground = foreground;
                textBlock.Inlines.Add(line);
            }
            ));
        }

        private void ClearTextBlock()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                textBlock.Inlines.Clear();
            }
           ));
        }
    }
}
