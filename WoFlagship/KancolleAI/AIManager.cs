using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using WoFlagship.Logger;
using WoFlagship.Utils;

namespace WoFlagship.KancolleAI
{
    class AIManager
    {
        public event Action<List<IKancolleAI>> OnAILoaded;

        public const string AIFolder = "AIs";

        public List<IKancolleAI> AIs { get; private set; } = new List<IKancolleAI>();

        public void LoadAIs()
        {
            LogFactory.SystemLogger.Info("开始载入AI");
            AIs.Clear();
            DirectoryInfo aiRootFoler = new DirectoryInfo(AIFolder);
            if (aiRootFoler.Exists)
            {
                foreach (var aiFolder in aiRootFoler.GetDirectories())
                {
                    var aiDlls = aiFolder.GetFiles("*.dll");
                    foreach (var dll in aiDlls)
                    {
                        try
                        {
                            Assembly assembly = Assembly.LoadFile(dll.FullName);
                            var types = from t in assembly.GetTypes()
                                        where t.IsClass && t.Namespace.StartsWith("WoFlagship.KancolleAI")
                                        select t;
                            try
                            {
                                foreach (var type in types)
                                {
                                    if (type.GetInterface("IKancolleAI") != null)
                                    {
                                        var ai = assembly.CreateInstance(type.FullName) as IKancolleAI;
                                        if (ai.Name == null || ai.Name == "")
                                        {
                                            LogFactory.SystemLogger.Error($"AI文件'{dll.Name}'中的类型{type.FullName}'载入失败，AI名'Name'不能为空");
                                            continue;
                                        }
                                        var sameAI = AIs.Find(p => p.Name == ai.Name);
                                        if (sameAI != null)
                                        {
                                            IKancolleAI p;
                                            if (sameAI.Version > ai.Version)
                                                p = sameAI;
                                            else
                                                p = ai;
                                            LogFactory.SystemLogger.Error($"存在多个AI'{ai.Name}'，将只保留版本最高的一个[Version={p.Version}]");
                                            AIs.Remove(sameAI);
                                            AIs.Add(p);
                                        }
                                        else
                                        {
                                            AIs.Add(ai);
                                            LogFactory.SystemLogger.Info($"AI'{ai.Name}'载入成功");
                                        }
                                    }
                                }

                            }
                            catch (NullReferenceException) { }//当types为空时，就会报这个异常，这说明该文件下没有IPlugin类

                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            MessageBox.Show("PluginManager\n" + ex.Message);
#endif
                            LogFactory.SystemLogger.Error($"AI文件'{dll.Name}'载入失败", ex);
                        }
                    }
                }
            }
            LogFactory.SystemLogger.Info($"AI载入完毕，共载入{AIs.Count}个AI");
            OnAILoaded?.InvokeAll(AIs);
        }
    }
}
