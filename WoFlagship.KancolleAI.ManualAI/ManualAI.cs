using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.KancolleAI.ManualAI
{
    class ManualAI : IKancolleAI
    {
        private ManualAIPanel panel = new ManualAIPanel();

        public ManualAI()
        {
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

        event Action<KancolleTask> OnTaskGenerated;
        event Action<KancolleTask> IKancolleAI.OnTaskGenerated
        {
            add
            {
                OnTaskGenerated += value;
            }

            remove
            {
                OnTaskGenerated -= value;
            }
        }

        public void OnGameDataUpdatedHandler(KancolleGameData gameData)
        {
            panel.UpdateGameContext(gameData);
        }

        public void Start()
        {
            
        }

        private void Panel_OnTaskGenerated(KancolleTask obj)
        {
            OnTaskGenerated?.Invoke(obj);
        }

        public void Stop()
        {
            if (OnTaskGenerated != null)
            {
                foreach(var listener in OnTaskGenerated.GetInvocationList())
                {
                    OnTaskGenerated -= listener as Action<KancolleTask>;
                }
            }
        }
    }
}
