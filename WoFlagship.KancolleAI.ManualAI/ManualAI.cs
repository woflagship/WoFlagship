using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;
using TestClassLibaray;
using System.Windows;

namespace WoFlagship.KancolleAI.ManualAI
{
    class ManualAI : IKancolleAI
    {
        private ManualAIPanel panel = new ManualAIPanel();

        public ManualAI()
        {
            //用于测试插件加载的依赖项
            Class1 c1 = new Class1();
            c1.i = 0;
            panel.OnTaskGenerated += Panel_OnTaskGenerated;
        }

        public UserControl AIPanel
        {
            get
            {
                return panel;
            }
        }

        public string Description
        {
            get
            {
                return "手动控制Task";
            }
        }

        public string Name
        {
            get
            {
                return "手动控制";
            }
        }

      

        public int Version
        {
            get
            {
                return 0;
            }
        }


        public void OnGameDataUpdated(KancolleGameData gameData)
        {
            panel.UpdateGameContext(gameData);
        }

        public void Start()
        {
            
        }

        private async void Panel_OnTaskGenerated(KancolleTask obj)
        {
            await KancolleTaskExecutor.Instance.DoTaskAsync(obj);
        }

        public void Stop()
        {
            
        }

    }
}
