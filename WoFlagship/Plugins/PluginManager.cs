using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using WoFlagship.Logger;

namespace WoFlagship.Plugins
{
    class PluginManager
    {
        public event Action<List<IPlugin>> OnPluginsLoaded;

        public const string PluginFolder = "Plugins";

        public List<IPlugin> Plugins { get; private set; } = new List<IPlugin>();

        public void LoadPlugins()
        {
            LogFactory.SystemLogger.Info("开始载入插件");
            Plugins.Clear();
            DirectoryInfo pluginRootFolder = new DirectoryInfo(PluginFolder);
            if (pluginRootFolder.Exists)
            {
                foreach (var pluginFolder in pluginRootFolder.GetDirectories())
                {
                    var pluginDlls = pluginFolder.GetFiles("*.dll");
                    foreach (var dll in pluginDlls)
                    {
                        try
                        {
                            Assembly assembly = Assembly.LoadFrom(dll.FullName);
                            var types = from t in assembly.GetTypes()
                                        where t.IsClass && t.Namespace != null && t.Namespace.StartsWith("WoFlagship.Plugins")
                                        select t;

                            foreach (var type in types)
                            {
                                if (type.GetInterface("IPlugin") != null)
                                {
                                    var plugin = assembly.CreateInstance(type.FullName) as IPlugin;
                                    if (plugin.Name == null || plugin.Name == "")
                                    {
                                        LogFactory.SystemLogger.Error($"插件文件'{dll.Name}'中的类型{type.FullName}'载入失败，插件名'Name'不能为空");
                                        continue;
                                    }
                                    var samePlugin = Plugins.Find(p => p.Name == plugin.Name);
                                    if (samePlugin != null)
                                    {
                                        IPlugin p;
                                        if (samePlugin.Version > plugin.Version)
                                            p = samePlugin;
                                        else
                                            p = plugin;
                                        LogFactory.SystemLogger.Error($"存在多个插件'{plugin.Name}'，将只保留版本最高的一个[Version={p.Version}]");
                                        Plugins.Remove(samePlugin);
                                        Plugins.Add(p);
                                    }
                                    else
                                    {
                                        Plugins.Add(plugin);
                                        LogFactory.SystemLogger.Info($"插件'{plugin.Name}'载入成功");
                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            MessageBox.Show("PluginManager\n" + ex.Message);
#endif
                            LogFactory.SystemLogger.Error($"插件文件'{dll.Name}'载入失败", ex);
                        }
                    }
                }
            }
            LogFactory.SystemLogger.Info($"插件载入完毕，共载入{Plugins.Count}个插件");
            OnPluginsLoaded?.Invoke (Plugins);
        }

    }

}
