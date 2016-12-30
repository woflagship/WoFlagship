using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WoFlagship.Plugins;

namespace WoFlagship
{
    /// <summary>
    /// PluginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PluginWindow : Window
    {
        public IPlugin Plugin { get; private set;}
        public PluginWindow(IPlugin plugin)
        {
            if (plugin == null)
                this.Close();
            Plugin = plugin;
           
            InitializeComponent();

            this.Closing += PluginWindow_Closing;
        }

        private void PluginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if(Plugin.PluginPanel != null)
                grid.Children.Add(Plugin.PluginPanel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Plugin.Name;
        }
    }
}
