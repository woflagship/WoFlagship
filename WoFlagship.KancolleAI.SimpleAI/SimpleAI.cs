using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleCore.Navigation;
using WoFlagship.Utils.BehaviorTree;

namespace WoFlagship.KancolleAI.SimpleAI
{
    class SimpleAI : IKancolleAI
    {
        private SimpleAIPanel panel = new SimpleAIPanel();
        private DispatcherTimer timer = new DispatcherTimer();
        private KancolleGameData gameData = null;
        private List<int> ShipsWaitForRepaired = new List<int>();
        private KancolleScene currentScene = null;

        public SimpleAI()
        {
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (gameData != null)
                {
                    if (panel.AutoRepair)
                    {
                        for (int i = 0; i < gameData.DockArray.Count; i++)
                        {
                            var dock = gameData.DockArray[i];
                            //当前为空闲，或者应该为空闲，即预计完成时间已经超过当前时间10miao
                            if (dock.State == 0 || (dock.State > 0 && dock.CompleteTime < DateTime.Now - TimeSpan.FromSeconds(10)))
                            {
                                int no = tryFindAShipToRepair();
                                if (no > 0)
                                {
                                    RepairTask task = new RepairTask(no, i, false);
                                    KancolleTaskExecutor.Get().DoTask(task);
                                }
                                break;
                            }
                        }
                    }
                }
            }));

            
        }

        private int tryFindAShipToRepair()
        {
           
            if(gameData != null)
            {
                var shipBorkens = (from s in gameData.OwnedShipDictionary.Values
                                  where s.NowHP < s.MaxHP && !gameData.IsShipRepairing(s) orderby s.DockTime ascending
                                  select s).ToArray();
                if(shipBorkens.Length > 0)
                {
                    if (DateTime.Now.TimeOfDay.Hours < 6 || DateTime.Now.TimeOfDay.Hours > 23)
                        return shipBorkens[shipBorkens.Length - 1].No;
                    else
                        return shipBorkens[0].No;
                }
            }
            return -1;
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
                return "常用AI";
            }
        }

        public string Name
        {
            get
            {
                return "常用AI";
            }
        }

       

        public int Version
        {
            get
            {
                return 1;
            }
        }

     

        public void OnGameDataUpdatedHandler(KancolleGameData gameData)
        {
            this.gameData = gameData;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                panel.UpdateGameData(gameData);
            }));
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            ShipsWaitForRepaired.Clear();
        }

        public void OnSceneUpdatedHandler(KancolleScene scene)
        {
            currentScene = scene;
        }
    }
}
