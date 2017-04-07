using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);//同时只允许1个执行线程
        private KancolleGameData gameData = null;
        private List<int> ShipsWaitForRepaired = new List<int>();

        private BehaviorTreeBuilder behaviorTree = null;
        private BehaviorTreeBuilder battleBTree = KancolleBehaviorFactory.SimpleBattle(0);

        private void InitBehaviorTree()
        {
            behaviorTree = new BehaviorTreeBuilder()
                .Sequence("Check")
                    .Condition("CheckGameData", async()=>await Task.Run(()=>KancolleGameData.Instance != null))
                    .Selector("SceneSelector")
                        .Sequence("UnknownScene")
                            .Condition("IsUnknowScene", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene?.SceneType == KancolleSceneTypes.Unknown))
                            .Do("Click", async ()=>
                            {
                                await KancolleTaskExecutor.Instance.DoTaskAsync(KancolleTask.BattleSkipTask);
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()                       
                        .Sequence("BattleScene")
                            .Condition("IsBattleScene", async () => await Task.Run(() => KancolleGameData.Instance.CurrentScene.IsBattleScene()))
                            .Do("DoBattle", async () =>
                            {
                                await battleBTree.BehaveAsync();
                                return BehaviorTreeStatus.Success;
                            })
                        .EndComposite()
                        .Selector("PortScene")
                            .Sequence("SimpleAI")
                                .Do("Repair", async () =>
                                {
                                    if(panel.AutoRepair)
                                        await RepairAsync();                                   
                                    if (panel.AutoBattle)
                                    {
                                        await Task.Delay(1000);
                                        if (!panel.AutoBattle) return BehaviorTreeStatus.Failure;
                                        var res = await OriganizeAsync();
                                        if (!res.IsSuccess)
                                            return BehaviorTreeStatus.Failure;
                                        if (!panel.AutoBattle) return BehaviorTreeStatus.Failure;
                                        await Task.Delay(1000);
                                        if (!panel.AutoBattle) return BehaviorTreeStatus.Failure;
                                        res = await KancolleTaskExecutor.Instance.DoTaskAsync(new SupplyTask(0));
                                        if (!res.IsSuccess)
                                            return BehaviorTreeStatus.Failure;
                                        if (!panel.AutoBattle) return BehaviorTreeStatus.Failure;
                                        await Task.Delay(1000);
                                        if (!panel.AutoBattle) return BehaviorTreeStatus.Failure;
                                        int battleMap = panel.BattleMap;
                                        if(battleMap > 0)
                                        {
                                            await KancolleTaskExecutor.Instance.DoTaskAsync(new MapTask(battleMap));
                                            if (!res.IsSuccess)
                                                return BehaviorTreeStatus.Failure;
                                        }
                                    }
                                    return BehaviorTreeStatus.Success;
                                })
                            .EndComposite()
                        .EndComposite()
                    .EndComposite()
                .EndComposite();
        }

        public SimpleAI()
        {
            InitBehaviorTree();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private async Task RepairAsync()
        {

            if (KancolleGameData.Instance != null)
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

        }

        private async Task<KancolleTaskResult> OriganizeAsync()
        {

                var gameData = KancolleGameData.Instance;
                //满血且等级高的优先
                var ships = (from s in gameData.OwnedShipDictionary.Values
                             where !s.BigBroken && !gameData.IsShipRepairing(s.No)
                             orderby (s.NowHP * 1.0 / s.MaxHP) descending, s.Condition descending, s.Level descending
                             select s).ToArray();
                var shipArray = new int[6];
                int i = 0,j = 0;
                HashSet<int> shipIdSet = new HashSet<int>();
                while (j < ships.Length && i<shipArray.Length)
                {
                    var s = ships[j++];
                    if (shipIdSet.Contains(s.ShipId))
                        continue;
                    shipArray[i++] = s.No;
                }

                for(;i<shipArray.Length; i++){
                    shipArray[i] = -1;
                }

                return await KancolleTaskExecutor.Instance.DoTaskAsync(new OrganizeTask(0, shipArray));
            
        }
            
    
        private async void Timer_Tick(object sender, EventArgs e)
        {
            panel.SetCurrentScene(KancolleGameData.Instance.CurrentScene);

            if (semaphoreSlim.CurrentCount == 0)
                return;
            await semaphoreSlim.WaitAsync();
            await behaviorTree.BehaveAsync();
            semaphoreSlim.Release();
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
    }
}
