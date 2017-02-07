﻿using Priority_Queue;
using System;
using System.Linq;
using System.Threading;
using CefSharp.Wpf;
using System.Windows;
using System.Collections;
using System.Collections.Generic;

namespace WoFlagship.KancolleCore.Navigation
{
    class KancolleTaskExecutor : IKancolleAPIReceiver
    {
        public event Action<KancolleTaskExecutor, TaskResult> OnTaskFinished;

        /// <summary>
        /// 等待新Scenes响应的超时，单位为毫秒
        /// ActionTimeout为了容忍网络延迟，或者在切换过程中出现新的场景（一般这种情况需要修改navigator）
        /// </summary>
        public double ActionTimeout { get; set; } = 10000;

        private Thread aiThread = null;
        private SimplePriorityQueue<KancolleTask> taskQueue = new SimplePriorityQueue<KancolleTask>();
        private bool bExit = false;

        private ResponseHelper lastResponse = null;
        private ResponseHelper currentResponse = null;
        private DateTime actionTimeStamp;
        private Object actionTimeStampLock = new object();

        private KancolleActionExecutor actionExector;

        private INavigator navigator = new SimpleNavigator();

        private Func<KancolleScene> GetCurrentScene;
        private Func<KancolleGameData> GetGameData;


        protected KancolleScene CurrentScene
        {
            get
            {
                
                return Application.Current.Dispatcher.Invoke(GetCurrentScene);
            }
        }

        public KancolleGameData GameData
        {
            get { return Application.Current.Dispatcher.Invoke(GetGameData); }
        }

        private ChromiumWebBrowser _webBrowser;
        protected ChromiumWebBrowser WebBrowser
        {
            get
            {
                return _webBrowser;
            }
        }

        public string Name
        {
            get { return "KancolleManualAI"; }
        }

        public string Description
        {
            get { return "手动控制AI"; }
        }

        public KancolleTaskExecutor(ChromiumWebBrowser webBrowser,Func<KancolleScene> GetCurrentScene, Func<KancolleGameData> GetGameData)
        {
            this.GetCurrentScene = GetCurrentScene;
            this.GetGameData = GetGameData;
            _webBrowser = webBrowser;
            actionExector = new KancolleActionExecutor(WebBrowser);
        }

        public void EnqueueTask(KancolleTask task)
        {
            taskQueue.Enqueue(task, (int)task.Priority);
        }

        public int TaskRemaining
        {
            get { return taskQueue.Count; }
        }

        public void Start()
        {
            if (aiThread == null)
            {
                aiThread = new Thread(new ThreadStart(AiThreadHandler));
                aiThread.IsBackground = true;
                aiThread.Start();
            }
            else
            {
                throw new ThreadStateException("TaskExecutor线程已经启动，无法重复启动！");
            }
        }

        public void Stop()
        {
            if (aiThread == null)
            {
                throw new ThreadStateException("TaskExecutor线程未Start！");
            }
            else
            {
                bExit = true;
                aiThread.Abort();
            }
        }

        private void AiThreadHandler()
        {

            while (!bExit)
            {
                //当前没有任务
                if (taskQueue.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                var currentTask = taskQueue.Dequeue();
                bool success = false;
                if (currentTask is OrganizeTask)
                {
                    success = Organize(currentTask as OrganizeTask); 
                }
                else if (currentTask is SupplyTask)
                {
                    success = Supply(currentTask as SupplyTask);
                }
                else if (currentTask is QuestTask)
                {
                    success = Quest(currentTask as QuestTask);
                }
                else if (currentTask is MissionTask)
                {
                    success = Mission(currentTask as MissionTask);
                }
                else if(currentTask is MapTask)
                {
                    success = Map(currentTask as MapTask);
                }
                else if(currentTask is BattleTask)
                {
                    success = Battle(currentTask as BattleTask);
                }
                else if(currentTask is RemodelTask)
                {
                    success = Remodel(currentTask as RemodelTask);
                }

                TaskResult result = new TaskResult()
                {
                    Success = success,
                    FinishedTask = currentTask,
                };

                OnTaskFinished?.Invoke(this, result);
                Thread.Sleep(1000);

            }
            MessageBox.Show("thread finish");
        }

        private bool Organize(OrganizeTask task)
        {
            int deck = task.OrganizedDeck;
            if (deck < 0 || deck > 3)
                throw new ArgumentOutOfRangeException("编成OrganizedDeck只能是0-3");
            
            int[] ships = task.Ships;
            if (ships.Length != 6)
                throw new ArgumentOutOfRangeException("编成Ships个数必须为6，空的船位用-1表示");

            if (deck == 0 && ships[0] == -1)
                throw new ArgumentException("第一舰队的旗舰不能为空");

            bool foundEmpty = false;
            for(int i=0; i<ships.Length; i++)
            {
                if (ships[i] < 0)
                    foundEmpty = true;
                else if (foundEmpty)
                    throw new ArgumentException("编成Ships参数错误，不可以存在中间的-1，如果出现空船位，往后的所有船位都必须空");
            }

            bool result;
            //先到编成界面
            result = ReachScene(KancolleSceneTypes.Organize);
            if (!result)
                return false;
            //切换到正确的舰队
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Decks[deck]));
            Thread.Sleep(1000);

            //先移除除旗舰外所有的位置
            /*
            if (GameData.OwnedShipPlaceArray[deck, 1] != -1)//至少有2个船
            {
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_RemoveAllExceptFirst));
                result = LockNowAndWaitForResponse();
                if (!result)//超时
                    return false;
                Thread.Sleep(1000);
            }*/


            var sortedShip = (from s in GameData.OwnedShipDictionary.Values
                             orderby s.No descending
                             select s).ToArray();//按照降序排列，和new的顺序相同

            //依次变更
            for (int i = 0; i < ships.Length; i++)
            {
                int currentShipId = GameData.OwnedShipPlaceArray[deck, i];
                if (ships[i] == currentShipId)
                    continue;

                if (ships[i] == -1)
                {
                    for (int j = i; j < ships.Length; j++)//删除后面的
                    {
                        if (GameData.OwnedShipPlaceArray[deck, i] == -1)//始终判断第i个
                            break;
                        //点击变更按钮
                        actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_ChangeButtons[i]));//始终移除第i个位置的，因为删了之后后面会补上来
                        Thread.Sleep(1000);
                        actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Remove));//移除
                        LockNowAndWaitForResponse();
                        Thread.Sleep(1000);
                    }
                    break;
                }


                //点击变更按钮
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_ChangeButtons[i]));
                Thread.Sleep(1000);
                //将排序方式改为new
                DateTime start = DateTime.Now;
                while (CurrentScene.SceneState != KancolleSceneStates.Organize_SortByNew && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
                {
                    //点击变更排序
                    actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_SortType));
                    Thread.Sleep(1000);
                }
                if (CurrentScene.SceneState != KancolleSceneStates.Organize_SortByNew)
                    return false;

                int index = indexOfShips(sortedShip, ships[i]);
                if (index == -1)//没找到
                    return false;

           
                int maxPage = sortedShip.Length / 10;//最大的页数
                int page = index / 10;//所在的页数，每页最多可以放10个
                Tuple<int, int> pageTurn = getPageTurn(maxPage, page);
                int page5 = pageTurn.Item1;//每5页翻一次
                int pageIn5 = pageTurn.Item2;//5页翻完后还有几页
                int item = index % 10;//每页第几个
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_FirstPage));//转到第1页
                Thread.Sleep(1000);
                for (int p = 0; p < page5; p++)//先5页5页的翻
                {
                    actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Next5Page));
                    Thread.Sleep(1000);
                }

                
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Pages[pageIn5]));
                Thread.Sleep(1000);

                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_ShipList[item]));
                Thread.Sleep(500);
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Change_Decide));
                result = LockNowAndWaitForResponse();
                if (!result)
                    return false;
                Thread.Sleep(1000);
            }

            return true;
        }

        private int indexOfShips(KancolleShip[] shipArray, int noToFind)
        {
            for(int i=0; i<shipArray.Length; i++)
            {
                if (shipArray[i].No == noToFind)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 当最大页数为maxPage时，翻到第page页所需的操作数；所有页数都以0开始计算！
        /// </summary>
        /// <param name="maxPage"></param>
        /// <param name="page"></param>
        /// <param name="pageStep"></param>
        /// <returns>item1:每pageStep页翻的次数；item2：翻完pageStep页后剩余的位置</returns>
        Tuple<int, int> getPageTurn(int maxPage, int page, int pageStep = 5)
        {
            //以pageStep为5为例
            //列表始终包含5页，例如0-4,3-7,8-12
            //5页切换规则为[0,4]->[3,7]->[8,12]->...->[max-4,max](max为最大页数)
            int page5 = 0;//翻5页的次数
            int pageIn5 =0;//翻玩5页剩余的位置
            if (page < pageStep)
            {
                page5 = 0;
                pageIn5 = page;
            }
            else if(page > maxPage- pageStep)
            {
                page5 = maxPage / pageStep + 1;
                pageIn5 = pageStep - 1 - (maxPage - page);
            }
            else
            {
                page5 = (page + 2) / pageStep;
                pageIn5 = (page + 2) % pageStep;
            }


            Tuple<int, int> turn = new Tuple<int, int>(page5, pageIn5);
            return turn;
        }

        private bool Supply(SupplyTask task)
        {
            bool result;
            //先转到补给界面
            result = ReachScene(KancolleSceneTypes.Supply);
            if (!result)
                return false;

            //转到正确的补给舰队
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Supply_Decks[task.SupplyDeck]));
            Thread.Sleep(1000);

            //悬停补给全部的按钮
            actionExector.Execute(new KancolleAction(ActionTypes.Move, KancolleWidgetPositions.Supply_SupplyDeck));
            Thread.Sleep(1000);
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Supply_SupplyDeck));

            //点击补给全部的按钮
            result = LockNowAndWaitForResponse();
            
            if (!result)
                return false;

            return true;
        }

        private bool Quest(QuestTask task)
        {
            bool result;
            //先转到任务界面
            result = ReachScene(KancolleSceneTypes.Quest);
            if(!result)
                return false;
            return true;
        }

        private bool Mission(MissionTask task)
        {
            bool result;
            //先转到远征界面
            result = ReachScene(KancolleSceneTypes.Mission);
            if (!result)
                return false;

            //MissionId以1开头的
            int page = (task.MissionId - 1) / 8;
            int item = (task.MissionId - 1) % 8;
            //先找到页数
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Maps[page]));
            Thread.Sleep(1000);
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Mission_MissionItems[item]));
            Thread.Sleep(500);
            if (CurrentScene != KancolleSceneTypes.Mission_Decide)
                return false;
            //决定按钮
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Decide));
            //鼠标移开
            actionExector.Execute(new KancolleAction(ActionTypes.Move, KancolleWidgetPositions.Port));
            Thread.Sleep(500);
            if (CurrentScene != KancolleSceneTypes.Mission_Start)
                return false;
            //选择舰队
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Start_Decks[task.MissionFleet]));
            
            Thread.Sleep(500);
            /*if (CurrentScene != Scenes.Mission_Start_True)
                return false;*/
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Start));
            return true;
        }

        //出击
        private bool Map(MapTask task)
        {
            bool result;
            //先到出击地图界面
            result = ReachScene(KancolleSceneTypes.Map);
            if(!result)
                return false;

            KancolleMapInfoData mapinfo;
            if (!GameData.MapInfoDictionary.TryGetValue(task.MapId, out mapinfo))
                return false;
            int areaIndex = mapinfo.MapAreaId - 1;
            int mapIndex = mapinfo.MapAreaId - 1;

            //先转到海域
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Map_Areas[areaIndex]));
            Thread.Sleep(1000);

            //选择地图
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Map_Maps[mapIndex]));
            Thread.Sleep(1000);

            //点击决定
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Map_Decide));
            Thread.Sleep(1000);

            //选择舰队
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Map_SelectDeck[task.Fleet]));
            Thread.Sleep(1000);

            //点击出击
            actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Map_Start));
            result = LockNowAndWaitForResponse();
            if (!result)
                return false;
            return true;
        }

        private bool Battle(BattleTask task)
        {
            bool result;
            if(task is BattleChoiceTask)
            {
                var t = task as BattleChoiceTask;
                if(t.BattleChoice == BattleChoiceTask.BattleChoices.Back || t.BattleChoice == BattleChoiceTask.BattleChoices.Night)
                {
                    //夜战选择
                    result = WaitForScene(KancolleSceneTypes.Battle_NightChoice, ActionTimeout);
                    if (!result)
                        return false;
                    //左为返回，右为夜战
                    if (t.BattleChoice == BattleChoiceTask.BattleChoices.Back)
                        return actionExector.Execute(KancolleWidgetPositions.Battle_LeftChoice);
                    else
                        return actionExector.Execute(KancolleWidgetPositions.Battle_RightChoice);
                }
                else if(t.BattleChoice == BattleChoiceTask.BattleChoices.Next || t.BattleChoice == BattleChoiceTask.BattleChoices.Return)
                {
                    result = WaitForScene(KancolleSceneTypes.Battle_NextChoice, ActionTimeout);
                    if (!result)
                        return false;
                    //左为进击，右为回港
                    if (t.BattleChoice == BattleChoiceTask.BattleChoices.Next)
                        return actionExector.Execute(KancolleWidgetPositions.Battle_LeftChoice);
                    else
                        return actionExector.Execute(KancolleWidgetPositions.Battle_RightChoice);
                }
            }
            else if(task is BattleFormationTask)
            {
                var t = task as BattleFormationTask;
                result = WaitForScene(KancolleSceneTypes.Battle_Formation, ActionTimeout);
                if (!result)
                    return result;
                actionExector.Execute(KancolleWidgetPositions.Battle_Formation[t.Formation-1]);
            }

            return true;
        }

        //改装
        private bool Remodel(RemodelTask task)
        {
            bool result;

            //需要改装的舰娘no
            var shipNo = GameData.OwnedShipPlaceArray[task.TargetDeck, task.TargetPosition];
            if (shipNo == -1)
                return false;

            //先到改装界面
            result = ReachScene(KancolleSceneTypes.Remodel);
            if (!result)
                return false;

            //选择正确的舰队
            actionExector.Execute(KancolleWidgetPositions.Remodel_Decks[task.TargetDeck]);
            Thread.Sleep(1000);

            //选择正确的舰娘
            actionExector.Execute(KancolleWidgetPositions.Remodel_Ships[task.TargetPosition]);
            Thread.Sleep(1000);


            //对每一个装备
            for(int i=0; i<task.SlotItemNos.Length; i++)
            {
                int itemNo = task.SlotItemNos[i];
                if (itemNo < 0)
                    continue;
                if (GameData.OwnedShipDictionary[shipNo].Slot[i] == itemNo)
                    continue;
                IEnumerable<int> items;
                bool isEquipedSlot = false;
                if (GameData.EquipedSlotDictionary.ContainsKey(itemNo))//该装备已经被别的舰娘装备
                {
                    //不能在本船
                    items = from es in GameData.EquipedSlotDictionary
                            where es.Value!=shipNo && GameData.CanShipEquipItem(GameData.OwnedShipDictionary[shipNo].ShipId, GameData.OwnedSlotDictionary[es.Key].SlotItemId)
                            select es.Key;
                    isEquipedSlot = true;
                }
                else
                {
                    items = from s in GameData.UnEquipedSlotArray
                            where GameData.CanShipEquipItem(GameData.OwnedShipDictionary[shipNo].ShipId, GameData.OwnedSlotDictionary[s].SlotItemId)
                            select s;
                }
                //以slotItemId为第一关键字，no为第二关键字
                var sortedItem = (from s in items
                                 orderby GameData.OwnedSlotDictionary[s].SlotItemId, GameData.OwnedSlotDictionary[s].No
                                  select GameData.OwnedSlotDictionary[s]).ToArray();
              
                //找到装备位置
                int index = indexOfItems(sortedItem, itemNo);
                if (index < 0)
                    return false;
                //选择装备槽
                actionExector.Execute(KancolleWidgetPositions.Remodel_Items[i]);
                Thread.Sleep(1000);

                if(isEquipedSlot)//选择已装备列表
                {
                    if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Other)
                    {
                        actionExector.Execute(KancolleWidgetPositions.Remodel_ChangeItemMode);
                        Thread.Sleep(1000);
                        if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Other)
                            return false;
                    }
                }
                else
                {
                    if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Normal)
                    {
                        actionExector.Execute(KancolleWidgetPositions.Remodel_ChangeItemMode);
                        Thread.Sleep(1000);
                        if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Normal)
                            return false;
                    }
                }

                int maxPage = sortedItem.Length / 10;//最大的页数
                int page = index / 10;//所在的页数，每页最多可以放10个
                Tuple<int, int> pageTurn = getPageTurn(maxPage, page);
                int page5 = pageTurn.Item1;//每5页翻一次
                int pageIn5 = pageTurn.Item2;//5页翻完后还有几页
                int item = index % 10;//每页第几个
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_FirstPage));//转到第1页
                Thread.Sleep(1000);
                for (int p = 0; p < page5; p++)//先5页5页的翻
                {
                    actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_Next5Page));
                    Thread.Sleep(1000);
                }


                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_Pages[pageIn5]));
                Thread.Sleep(1000);

                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_ItemList[item]));
                Thread.Sleep(500);
                actionExector.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Change_Decide));
                Thread.Sleep(1000);
                if (isEquipedSlot)
                {
                    //如果是已装备的，则还有一个确认列表
                    if (CurrentScene.SceneType !=  KancolleSceneTypes.Remodel_ItemList_Other_Decide)
                        return false;
                    actionExector.Execute(KancolleWidgetPositions.Remodel_Change_Other_Decide);
                    Thread.Sleep(500);
                }
                result = LockNowAndWaitForResponse();
                if (!result)
                    return false;
                Thread.Sleep(1000);
            }

            return false;
        }

        private int indexOfItems(KancolleSlotItem[] items, int itemNo)
        {
            for(int i=0; i<items.Length; i++)
            {
                if (itemNo == items[i].No)
                    return i;
            }
            return -1;
        }

        private bool WaitForScene(KancolleSceneTypes scene, double timeout)
        {
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds <= timeout)
            {
                if (CurrentScene == scene)
                    break;
                Thread.Sleep(500);
            }
            return CurrentScene == scene;
        }

        /// <summary>
        /// 线程阻塞，到达场景toScene
        /// </summary>
        /// <param name="toScene"></param>
        /// <returns></returns>
        private bool ReachScene(KancolleSceneTypes toScene)
        {
            //先转到补给界面
            var edges = navigator.Navigate(CurrentScene.SceneType, toScene);
            if (edges == null)
                return false;//无法到达界面
            bool isFinished = false;
            while (!isFinished && edges.Count > 0)
            {
                isFinished = true;
                foreach (var edge in edges)
                {
                    //等待结果
                    if (!DoActionAndWaitForScene(edge, ActionTimeout))
                    {
                        edges = navigator.Navigate(CurrentScene.SceneType, toScene);
                        if (edges == null)
                            return false;//无法到达界面
                        isFinished = false;//中途有变，所以重新切换界面
                        break;

                    }
                    Thread.Sleep(1000);
                }
            }


            return CurrentScene.SceneType == toScene;
        }

        /// <summary>
        /// 线程阻塞，一直等到新的场景出现
        /// 如果新的场景为edge.Target，返回true，否则为false
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>如果新的场景为scene，返回true，否则为false</returns>
        private bool DoActionAndWaitForScene(KancolleActionEdge edge, double timeout)
        {
            if (!actionExector.Execute(edge))//未能执行成功
                return false;
            if (edge.Target == KancolleSceneTypes.Quest)
            {
                Thread.Sleep(2000);//等待明石动画
                if (!actionExector.Execute(edge))//点掉明石
                    return false;
            }
            return WaitForScene(edge.Target, timeout);
        }

        /// <summary>
        /// 线程阻塞，一直等到有response响应
        /// </summary>
        /// <returns></returns>
        private bool LockNowAndWaitForResponse()
        {
            lock (actionTimeStampLock)
            {
                actionTimeStamp = DateTime.Now;
            }
            DateTime start = DateTime.Now;
            while ((currentResponse == null || currentResponse.Time <= actionTimeStamp) && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
            {
                Thread.Sleep(500);
            }

            if (currentResponse != null && currentResponse.Time > actionTimeStamp)
                return true;
            return false;
        }

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            lock (actionTimeStampLock)
            {
                lastResponse = currentResponse;
                currentResponse = new ResponseHelper(requestInfo, response, api);
            }
        }

       
    }

    public class TaskResult
    {
        public bool Success { get; set; }

        public KancolleTask FinishedTask { get; set; }

    }
}
