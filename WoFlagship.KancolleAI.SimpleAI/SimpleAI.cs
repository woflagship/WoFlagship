using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private BehaviorTreeBuilder behaviorTree = null;

        private void InitBehaviorTree()
        {
            behaviorTree = new BehaviorTreeBuilder()
                .Selector("SceneSelector")
                    .Condition("UnknowScene", async () => await Task.Run(() => currentScene?.SceneType == KancolleSceneTypes.Unknown))
                    .Sequence("BattleScene")
                        .Condition("IsBattleScene", async () => await Task.Run(() => currentScene != null && currentScene.IsBattleScene()))
                    .EndComposite()
                    .Selector("PortScene")
                        .Sequence("Repair")
                            .Condition("AutoRepair", async () => await Task.Run(() => panel.AutoRepair))
                            .Do("Repair", async () => await RepairAsync())
                        .EndComposite()
                    .EndComposite();
        }

        public SimpleAI()
        {
            InitBehaviorTree();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private async Task<BehaviorTreeStatus> RepairAsync()
        {
            var res = await Application.Current.Dispatcher.Invoke((async ()=>
            {
                BehaviorTreeStatus result = BehaviorTreeStatus.Failure;
                if (gameData != null)
                {
                    var repairNos = findAShipToRepair();
                    if (repairNos != null)
                    {
                        int repairIndex = 0;
                        for (int i = 0; i < gameData.DockArray.Count && repairIndex < repairNos.Length; i++)
                        {
                            if (repairIndex >= repairNos.Length)
                                break;
                            var dock = gameData.DockArray[i];
                            //当前为空闲
                            if (dock.State == 0)
                            {
                                var taskResult = await KancolleTaskExecutor.Instance.DoTaskAsync(new RepairTask(repairNos[repairIndex++], i, false));
                                if (taskResult.IsSuccess)
                                    result = BehaviorTreeStatus.Success;
                            }
                            else if (dock.State > 0 && dock.CompleteTime < DateTime.Now - TimeSpan.FromSeconds(10))
                            {
                                //本应该为空闲（给了10秒的容错），但是还没有刷新数据导致state仍然不为0，则刷新
                                await KancolleTaskExecutor.Instance.DoTaskAsync(KancolleTask.RefreshDataTask);
                                result = BehaviorTreeStatus.Running;
                                break;
                            }
                        }
                    }
                }
                return result;
            }));
            return res;
        }
            

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await behaviorTree.BehaveAsync();
            /*
            await Application.Current.Dispatcher.InvokeAsync(new Action(async () =>
            {
                if (gameData != null)
                {
                    
                    //没有别的任务才可以自动维修

                        var repairNos = findAShipToRepair();
                        if (repairNos != null)
                        {
                            int repairIndex = 0;
                            for (int i = 0; i < gameData.DockArray.Count && repairIndex<repairNos.Length; i++)
                            {
                                if (repairIndex >= repairNos.Length)
                                    break;
                                var dock = gameData.DockArray[i];
                                //当前为空闲
                                if (dock.State == 0)
                                {
                                    await KancolleTaskExecutor.Instance.DoTaskAsync(new RepairTask(repairNos[repairIndex++], i, false));
                                }
                                else if (dock.State > 0 && dock.CompleteTime < DateTime.Now - TimeSpan.FromSeconds(10))
                                {
                                    //本应该为空闲（给了10秒的容错），但是还没有刷新数据导致state仍然不为0，则刷新
                                    await KancolleTaskExecutor.Instance.DoTaskAsync(KancolleTask.RefreshDataTask);
                                    break;
                                }
                            }
                        }
                    
                }
            }));*/

            
        }

        private int[] findAShipToRepair()
        {
           
            if(gameData != null)
            {
                if (DateTime.Now.TimeOfDay.Hours < 6 || DateTime.Now.TimeOfDay.Hours > 23)
                    return (from s in gameData.OwnedShipDictionary.Values
                            where s.NowHP < s.MaxHP && !gameData.IsShipRepairing(s)
                            orderby s.DockTime descending
                            select s.No).ToArray();
                else
                    return (from s in gameData.OwnedShipDictionary.Values
                            where s.NowHP < s.MaxHP && !gameData.IsShipRepairing(s)
                            orderby s.DockTime ascending
                            select s.No).ToArray();
               
            }
            return null;
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

     

        public void OnGameDataUpdated(KancolleGameData gameData)
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
