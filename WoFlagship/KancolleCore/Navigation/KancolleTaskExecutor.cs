using Priority_Queue;
using System;
using System.Linq;
using System.Threading;
using CefSharp.Wpf;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using static WoFlagship.KancolleCore.Navigation.KancolleTaskResultErrors;
using System.Diagnostics;
using WoFlagship.Utils;
using System.Collections.Specialized;
using System.Threading.Tasks;
using WoFlagship.KancolleCore.KancolleBattle;

namespace WoFlagship.KancolleCore.Navigation
{
    /// <summary>
    /// 任务执行器，执行各类KancolleTask
    /// </summary>
    public class KancolleTaskExecutor : IKancolleAPIReceiver
    {
        internal event Action<KancolleTaskExecutor, KancolleTaskResult> OnTaskFinished_Internal;
        internal event Action<KancolleTaskExecutor, NotifyCollectionChangedEventArgs> OnTasksChanged_Internal;
        internal event Action<KancolleTaskExecutor> OnTaskStart_Internal;


        /// <summary>
        /// 等待新Scenes响应的超时，单位为毫秒
        /// ActionTimeout为了容忍网络延迟，或者在切换过程中出现新的场景（一般这种情况需要修改navigator）
        /// </summary>
        public double ActionTimeout { get; private set; } = 10000;

        /// <summary>
        /// 战斗之间的转场间隔，比如等待罗盘娘，等待新舰娘等时间间隔
        /// </summary>
        public double BattleSkipTimeout { get; private set; } = 10000;

        /// <summary>
        /// 正在运行的任务，没有运行的任务则为null
        /// </summary>
        public KancolleTask RunningTask { get; private set; }

        /// <summary>
        /// 当前剩余任务数，不包含RunningTask
        /// 不再对外可获取
        /// </summary>
        private int TaskRemaining
        {
            get { return taskQueue.Count; }
        }

        private Thread aiThread = null;
        private SimplePriorityQueue<KancolleTask> taskQueue = new SimplePriorityQueue<KancolleTask>();
        private bool bExit = false;

        private ResponseHelper lastResponse = null;
        private ResponseHelper currentResponse = null;
        private DateTime actionTimeStamp;
        private Object actionTimeStampLock = new object();

        private KancolleActionExecutor actionExecutor;

        private INavigator navigator = new SimpleNavigator();

        private Func<KancolleScene> GetCurrentScene;

        private Random random = new Random();

        /// <summary>
        /// 最后一次任务的结果
        /// </summary>
        public KancolleTaskResult LastResult
        {
            get; private set;
        } = null;

        private Battle currentBattle = null;

        private static KancolleTaskExecutor s_instance = null;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        /// <returns></returns>
        public static KancolleTaskExecutor Instance { get { return s_instance; } }

        protected KancolleScene CurrentScene
        {
            get
            {
                
                return Application.Current.Dispatcher.Invoke(GetCurrentScene);
            }
        }


        internal KancolleTaskExecutor(KancolleActionExecutor actionExecutor, Func<KancolleScene> GetCurrentScene)
        {
            this.GetCurrentScene = GetCurrentScene;
            this.actionExecutor = actionExecutor;
            //单例类，只允许一个实例
            Debug.Assert(s_instance == null);
            s_instance = this;
        }

        /// <summary>
        /// 在优先级队列中插入一个新的任务，该任务会在合适的时机被执行
        /// 该函数将不再对外可用
        /// </summary>
        /// <param name="task"></param>
        private void EnqueueTask(KancolleTask task)
        {
            taskQueue.Enqueue(task, (int)task.Priority);
            OnTasksChanged_Internal?.InvokeAll(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, task));
        }

        /// <summary>
        /// 清空所有的任务
        /// </summary>
        public void ClearTaskList()
        {
            taskQueue.Clear();
            OnTasksChanged_Internal?.InvokeAll(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal IEnumerator<KancolleTask> GetEnumerator()
        {
            return taskQueue.GetEnumerator();
        }

        /// <summary>
        /// 执行task,改函数进程阻塞，等到该任务被执行并返回结果。
        /// </summary>
        /// <param name="task"></param>
        /// <returns>执行结果</returns>
        public KancolleTaskResult DoTask(KancolleTask task)
        {
            if(TaskRemaining > 0 || RunningTask != null)
            {
                var result = new KancolleTaskResult(task, KancolleTaskResultType.Fail, "当前正在执行其他任务！", ExistRunningTask);
                //OnTaskFinished_Internal(this, result);
                return result;
            }
            LastResult = null;
            EnqueueTask(task);
            while (LastResult == null)
                Thread.Sleep(500);
            return LastResult;
        }

        /// <summary>
        /// 执行task,并返回结果。
        /// </summary>
        /// <param name="task"></param>
        /// <returns>执行结果</returns>
        public async Task<KancolleTaskResult> DoTaskAsync(KancolleTask task)
        {
            return await Task.Run<KancolleTaskResult>(()=>
                {
                    return DoTask(task);
                });
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
                RunningTask = currentTask;
                OnTasksChanged_Internal?.InvokeAll(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, currentResponse));
                OnTaskStart_Internal?.InvokeAll(this);
                KancolleTaskResult result = null;
                if (currentTask is OrganizeTask)
                {
                    result = Organize(currentTask as OrganizeTask); 
                }
                else if (currentTask is SupplyTask)
                {
                    result = Supply(currentTask as SupplyTask);
                }
                else if (currentTask is QuestTask)
                {
                    result = Quest(currentTask as QuestTask);
                }
                else if (currentTask is MissionTask)
                {
                    result = Mission(currentTask as MissionTask);
                }
                else if(currentTask is MapTask)
                {
                    result = Map(currentTask as MapTask);
                }
                else if(currentTask is BattleTask)
                {
                    result = Battle(currentTask as BattleTask);
                }
                else if(currentTask is RemodelTask)
                {
                    result = Remodel(currentTask as RemodelTask);
                }
                else if(currentTask is RepairTask)
                {
                    result = Repair(currentTask as RepairTask);
                }
                else if(currentTask is RefreshDataTask)
                {
                    result = RefreshData(currentTask as RefreshDataTask);
                }

                if(result == null)
                {
                    result = new KancolleTaskResult(currentTask, KancolleTaskResultType.Fail, $"未能处理当前类型任务【{currentTask.GetType().Name}】", UnknownTaskType);
                }
                RunningTask = null;
                LastResult = result;
                OnTaskFinished_Internal?.Invoke(this, result);
                Thread.Sleep(1000);

            }
            MessageBox.Show("thread finish");
        }

        private KancolleTaskResult Organize(OrganizeTask task)
        {
            int deck = task.OrganizedDeck;
            if (deck < 0 || deck > 3)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"编成OrganizedDeck非法，只能是0-3，当前值设为【{deck}】", IncorrectDeckIndex);
            
            int[] ships = task.Ships;
            if (ships.Length != 6)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"编成舰娘个数必须为6，当前舰娘个数位【{ships.Length}】", IllegalShipCount);

            if (deck == 0 && ships[0] == -1)
               return new KancolleTaskResult(task, KancolleTaskResultType.Fail, "第一舰队的旗舰不能为空", EmptyFirstShip);

            bool foundEmpty = false;
            for(int i=0; i<ships.Length; i++)
            {
                if (ships[i] < 0)
                    foundEmpty = true;
                else if (foundEmpty)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, "编成参数错误，不可以存在中间的空位，如果中间出现空船位，往后的所有船位都必须位空", EmptyInterveningShip);
            }

            KancolleTaskResult tresult;
            bool result;
            //先到编成界面
            tresult = ReachScene(KancolleSceneTypes.Organize);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);
            //切换到正确的舰队
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Decks[deck]));
            Thread.Sleep(1000);

            //先移除除旗舰外所有的位置
            //这么做会降低效率，但是能解决很多编成时的问题           
            if (KancolleGameData.Instance.OwnedShipPlaceArray[deck, 1] != -1)//至少有2个船
            {
                result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Organize_RemoveAllExceptFirst));
                if (!result)//超时
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
                Thread.Sleep(1000);
            }


            var sortedShip = (from s in KancolleGameData.Instance.OwnedShipDictionary.Values
                             orderby s.No descending
                             select s).ToArray();//按照降序排列，和new的顺序相同

            //依次变更
            for (int i = 0; i < ships.Length; i++)
            {
                int currentShipId = KancolleGameData.Instance.OwnedShipPlaceArray[deck, i];
                if (ships[i] == currentShipId)
                    continue;

                if (ships[i] == -1)
                {
                    for (int j = i; j < ships.Length; j++)//删除后面的
                    {
                        if (KancolleGameData.Instance.OwnedShipPlaceArray[deck, i] == -1)//始终判断第i个
                            break;
                        //点击变更按钮
                        actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_ChangeButtons[i]));//始终移除第i个位置的，因为删了之后后面会补上来
                        Thread.Sleep(1000);
                        LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Remove));//移除
                        Thread.Sleep(1000);
                    }
                    break;
                }


                //点击变更按钮
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_ChangeButtons[i]));
                Thread.Sleep(1000);
                //将排序方式改为new
                DateTime start = DateTime.Now;
                while (CurrentScene.SceneState != KancolleSceneStates.Organize_SortByNew && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
                {
                    //点击变更排序
                    actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_SortType));
                    Thread.Sleep(1000);
                }
                if (CurrentScene.SceneState != KancolleSceneStates.Organize_SortByNew)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景状态【{CurrentScene.SceneState}】与预期场景状态【{KancolleSceneStates.Organize_SortByNew}】不符", UnexceptedState);

                int index = indexOfShips(sortedShip, ships[i]);
                if (index == -1)//没找到
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未在已有舰娘中找到舰娘序号【{ships[i]}】", UnfoundShipNo);


                int maxPage = sortedShip.Length / 10;//最大的页数
                int page = index / 10;//所在的页数，每页最多可以放10个
                Tuple<int, int> pageTurn = getPageTurn(maxPage, page);
                int page5 = pageTurn.Item1;//每5页翻一次
                int pageIn5 = pageTurn.Item2;//5页翻完后还有几页
                int item = index % 10;//每页第几个
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_FirstPage));//转到第1页
                Thread.Sleep(1000);
                for (int p = 0; p < page5; p++)//先5页5页的翻
                {
                    actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Next5Page));
                    Thread.Sleep(1000);
                }

                
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_Pages[pageIn5]));
                Thread.Sleep(1000);

                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Organize_Changes_ShipList[item]));
                Thread.Sleep(500);
                result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Organize_Change_Decide));
                if (!result)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
                Thread.Sleep(1000);
            }
            return new KancolleTaskResult(task, KancolleTaskResultType.Success, "编成完成", Success);
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

        private KancolleTaskResult Supply(SupplyTask task)
        {
            KancolleTaskResult tresult;
            bool result;
            //先转到补给界面
            tresult = ReachScene(KancolleSceneTypes.Supply);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);

            //转到正确的补给舰队
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Supply_Decks[task.SupplyDeck]));
            Thread.Sleep(1000);

            //悬停补给全部的按钮
            actionExecutor.Execute(new KancolleAction(ActionTypes.Move, KancolleWidgetPositions.Supply_SupplyDeck));
            Thread.Sleep(1000);

            //点击补给全部的按钮
            result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Supply_SupplyDeck));
            
            if (!result)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);

            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"补给舰队【{task.SupplyDeck}】成功", Success);
        }

        private KancolleTaskResult Quest(QuestTask task)
        {
            KancolleTaskResult tresult;
            //bool result;
            //先转到任务界面
            tresult = ReachScene(KancolleSceneTypes.Quest);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);

            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"任务选择成功", Success);
        }

        private KancolleTaskResult Mission(MissionTask task)
        {
            KancolleTaskResult tresult;
            //bool result;
            //先转到远征界面
            tresult = ReachScene(KancolleSceneTypes.Mission);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);


            //MissionId以1开头的
            int page = (task.MissionId - 1) / 8;
            int item = (task.MissionId - 1) % 8;
            //先找到页数
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Maps[page]));
            Thread.Sleep(1000);
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Mission_MissionItems[item]));
            Thread.Sleep(500);
            if (CurrentScene != KancolleSceneTypes.Mission_Decide)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Mission_Decide}】不符", UnexceptedScene);
            //决定按钮
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Decide));
            //鼠标移开
            actionExecutor.Execute(new KancolleAction(ActionTypes.Move, KancolleWidgetPositions.Port));
            Thread.Sleep(500);
            if (CurrentScene != KancolleSceneTypes.Mission_Decide)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Mission_Start}】不符", UnexceptedScene);
            //选择舰队
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Start_Decks[task.MissionFleet]));
            
            Thread.Sleep(500);
            /*if (CurrentScene != Scenes.Mission_Start_True)
                return false;*/
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Mission_Start));
            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"开始远征【{task.MissionId}】", Success);
        }

        //出击
        private KancolleTaskResult Map(MapTask task)
        {
            KancolleTaskResult tresult;
            bool result;
            //先到出击地图界面
            tresult = ReachScene(KancolleSceneTypes.Map);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);

            KancolleMapInfoData mapinfo;
            if (!KancolleGameData.Instance.MapInfoDictionary.TryGetValue(task.MapId, out mapinfo))
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能找到地图id【{task.MapId}】", UnfoundMapId);
            int areaIndex = mapinfo.MapAreaId - 1;
            int mapIndex = mapinfo.MapNo - 1;

            //先转到海域
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Map_Areas[areaIndex]));
            Thread.Sleep(1000);

            //选择地图
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Map_Maps[mapIndex]));
            Thread.Sleep(1000);

            //点击决定
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Map_Decide));
            Thread.Sleep(1000);

            //选择舰队
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Map_SelectDeck[task.Fleet]));
            Thread.Sleep(1000);

            //点击出击
            result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Map_Start));
            if (!result)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"舰队【{task.Fleet}】出击【{task.MapId}】", Success);
        }

        private KancolleTaskResult Battle(BattleTask task)
        {
           
            bool result;
            Battle battle = null;
            string taskContent = "作战行动";
            if(task is BattleChoiceTask)
            {
                var t = task as BattleChoiceTask;
                taskContent = $"作战行动【{t.BattleChoice.ToString()}】";
                if(t.BattleChoice == BattleChoiceTask.BattleChoices.Back || t.BattleChoice == BattleChoiceTask.BattleChoices.Night)
                {
                    //夜战选择
                    result = WaitForScene(KancolleSceneTypes.Battle_NightChoice, ActionTimeout);
                    if (!result)
                        return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Battle_NightChoice}】不符", UnexceptedScene);
                    //左为返回，右为夜战
                    if (t.BattleChoice == BattleChoiceTask.BattleChoices.Back)
                        actionExecutor.Execute(KancolleWidgetPositions.Battle_LeftChoice);
                    else
                    {//夜战会返回battle结果
                        battle = WaitForBattle(new KancolleAction(KancolleWidgetPositions.Battle_RightChoice));
                        if(battle == null)
                            return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得Battle数据", NoBattleInfo);
                    }
                }
                else if(t.BattleChoice == BattleChoiceTask.BattleChoices.Next || t.BattleChoice == BattleChoiceTask.BattleChoices.Return)
                {
                    result = WaitForScene(KancolleSceneTypes.Battle_NextChoice, ActionTimeout);
                    if (!result)
                        return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Battle_NextChoice}】不符", UnexceptedScene);
                    //左为进击，右为回港
                    if (t.BattleChoice == BattleChoiceTask.BattleChoices.Next)
                        actionExecutor.Execute(KancolleWidgetPositions.Battle_LeftChoice);
                    else
                        actionExecutor.Execute(KancolleWidgetPositions.Battle_RightChoice);
                }
            }
            else if(task is BattleFormationTask)
            {
                var t = task as BattleFormationTask;
                if(t.Formation<=0 || t.Formation>5)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"阵型参数错误，应为1-5，实际为【{t.Formation}】", IllegalFormation);
                taskContent = $"阵型选择【{t.Formation}】";
                result = WaitForScene(KancolleSceneTypes.Battle_Formation, ActionTimeout);
                if (!result)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Battle_Formation}】不符", UnexceptedScene);
                //阵型会返回battle结果
                battle = WaitForBattle(new KancolleAction(KancolleWidgetPositions.Battle_Formation[t.Formation - 1]));
                if (battle == null)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得Battle数据", NoBattleInfo);
            }
            else if(task is BattleSkipTask)
            {
                //var t = task as BattleSkipTask;
                taskContent = "跳过战斗过场";
                DateTime start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
                {
                    Thread.Sleep(500);
                    //一直点击屏幕空白处，直到场景跳转为已知，且不为罗盘娘界面
                    if (CurrentScene != KancolleSceneTypes.Unknown && CurrentScene != KancolleSceneTypes.Battle_Compass)
                        break;
                    actionExecutor.Execute(GetBattleRandomSafePoint());            
                }
  
            }

            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"{taskContent}完成", Success, battle);
        }

        private Point GetBattleRandomSafePoint()
        {
            double randX = random.NextDouble();
            double randY = random.NextDouble();
            var rect = KancolleWidgetPositions.Battle_SafeRect;
            return new Point(rect.X + rect.Width * randX, rect.Y + rect.Height * randY);
        }

        //改装
        private KancolleTaskResult Remodel(RemodelTask task)
        {
            bool result;
            KancolleTaskResult tresult;
            //需要改装的舰娘no
            var shipNo = KancolleGameData.Instance.OwnedShipPlaceArray[task.TargetDeck, task.TargetPosition];
            if (shipNo == -1)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前位置【{task.TargetDeck + "-" + task.TargetPosition}】没有舰娘", NoShipAtPosition);

            //先到改装界面
            tresult = ReachScene(KancolleSceneTypes.Remodel);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);

            //选择正确的舰队
            actionExecutor.Execute(KancolleWidgetPositions.Remodel_Decks[task.TargetDeck]);
            Thread.Sleep(1000);

            //选择正确的舰娘
            actionExecutor.Execute(KancolleWidgetPositions.Remodel_Ships[task.TargetPosition]);
            Thread.Sleep(1000);

            //先移除自己的所有装备，不过已经在正确位置的装备就不动了
            int correctSlotNum = 0;
            int slotNum = KancolleGameData.Instance.GetShip(shipNo).SlotNum;
            for(; correctSlotNum < slotNum; correctSlotNum++)
            {
                int itemNo = task.SlotItemNos[correctSlotNum];
                if (itemNo < 0)
                    break;
                if (KancolleGameData.Instance.OwnedShipDictionary[shipNo].Slot[correctSlotNum] != itemNo)
                    break;
            }
            for (int i = correctSlotNum; i < slotNum; i++)
            {
                if (KancolleGameData.Instance.OwnedShipDictionary[shipNo].Slot[correctSlotNum] < 0)//当前位置没有装备
                    break;
                //移除装备，因为装备移除后，下面的会补上来，所以始终是correntSlotNum的位置
                result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Remodel_RemoveItem[correctSlotNum]));
                if (!result)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
                Thread.Sleep(2000);
            }


            //对每一个装备
            for (int i=0; i<task.SlotItemNos.Length; i++)
            {
                int itemNo = task.SlotItemNos[i];
                if (itemNo < 0)
                    continue;
                if (KancolleGameData.Instance.OwnedShipDictionary[shipNo].Slot[i] == itemNo)
                    continue;
                IEnumerable<int> items;
                bool isEquipedSlot = false;
                if (KancolleGameData.Instance.EquipedSlotDictionary.ContainsKey(itemNo))//该装备已经被别的舰娘装备
                {
                    //不能在本船
                    items = from es in KancolleGameData.Instance.EquipedSlotDictionary
                            where es.Value!=shipNo && KancolleGameData.Instance.CanShipEquipItem(KancolleGameData.Instance.OwnedShipDictionary[shipNo].ShipId, KancolleGameData.Instance.OwnedSlotDictionary[es.Key].SlotItemId)
                            select es.Key;
                    isEquipedSlot = true;
                }
                else
                {
                    items = from s in KancolleGameData.Instance.UnEquipedSlotArray
                            where KancolleGameData.Instance.CanShipEquipItem(KancolleGameData.Instance.OwnedShipDictionary[shipNo].ShipId, KancolleGameData.Instance.OwnedSlotDictionary[s].SlotItemId)
                            select s;
                }
                //以slotItemId为第一关键字，no为第二关键字
                var sortedItem = (from s in items
                                 orderby KancolleGameData.Instance.OwnedSlotDictionary[s].SlotItemId, KancolleGameData.Instance.OwnedSlotDictionary[s].No
                                  select KancolleGameData.Instance.OwnedSlotDictionary[s]).ToArray();
              
                //找到装备位置
                int index = indexOfItems(sortedItem, itemNo);
                if (index < 0)
                    return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未找到装备【{itemNo}】", UnfoundItemNo);
                //选择装备槽
                actionExecutor.Execute(KancolleWidgetPositions.Remodel_Items[i]);
                Thread.Sleep(1000);

                if(isEquipedSlot)//选择已装备列表
                {
                    if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Other)
                    {
                        actionExecutor.Execute(KancolleWidgetPositions.Remodel_ChangeItemMode);
                        Thread.Sleep(1000);
                        if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Other)
                            return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景状态【{CurrentScene.SceneState}】与预期场景状态【{KancolleSceneStates.Remodel_ItemList_Other}】不符", UnexceptedState);
                    }
                }
                else
                {
                    if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Normal)
                    {
                        actionExecutor.Execute(KancolleWidgetPositions.Remodel_ChangeItemMode);
                        Thread.Sleep(1000);
                        if (CurrentScene.SceneState != KancolleSceneStates.Remodel_ItemList_Normal)
                            return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景状态【{CurrentScene.SceneState}】与预期场景状态【{KancolleSceneStates.Remodel_ItemList_Normal}】不符", UnexceptedState);
                    }
                }

                int maxPage = sortedItem.Length / 10;//最大的页数
                int page = index / 10;//所在的页数，每页最多可以放10个
                Tuple<int, int> pageTurn = getPageTurn(maxPage, page);
                int page5 = pageTurn.Item1;//每5页翻一次
                int pageIn5 = pageTurn.Item2;//5页翻完后还有几页
                int item = index % 10;//每页第几个
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_FirstPage));//转到第1页
                Thread.Sleep(1000);
                for (int p = 0; p < page5; p++)//先5页5页的翻
                {
                    actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_Next5Page));
                    Thread.Sleep(1000);
                }


                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_Pages[pageIn5]));
                Thread.Sleep(1000);

                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Remodel_Changes_ItemList[item]));
                Thread.Sleep(500);
                if (isEquipedSlot)
                {
                    result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Remodel_Change_Decide));
                    if (!result)
                        return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
                    Thread.Sleep(1000);
                    //如果是已装备的，则还有一个确认列表
                    if (CurrentScene.SceneType !=  KancolleSceneTypes.Remodel_ItemList_Other_Decide)
                        return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】与预期场景【{KancolleSceneTypes.Remodel_ItemList_Other_Decide}】不符", UnexceptedScene);
                    actionExecutor.Execute(KancolleWidgetPositions.Remodel_Change_Other_Decide);
                }
                else
                {
                    result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Remodel_Change_Decide));
                    if (!result)
                        return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
                }
               
                Thread.Sleep(2000);
            }

            return new KancolleTaskResult(task, KancolleTaskResultType.Success, "改装成功", Success);
        }

        //入渠
        private KancolleTaskResult Repair(RepairTask task)
        {
            KancolleTaskResult tresult;
            //先到入渠界面
            tresult = ReachScene(KancolleSceneTypes.RepairMain);
            if (!tresult.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, tresult.Message, tresult.ErrorCode);

            Thread.Sleep(500);
            actionExecutor.Execute(KancolleWidgetPositions.Repair_Docks[task.Dock]);

            var sortedShip = (from s in KancolleGameData.Instance.OwnedShipDictionary.Values
                              orderby s.No descending
                              select s).ToArray();//按照降序排列，和new的顺序相同
            Thread.Sleep(1000);
            //将排序方式改为new
            DateTime start = DateTime.Now;
            while (CurrentScene.SceneState != KancolleSceneStates.Repair_SortByNew && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
            {
                //点击变更排序
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_ShipList_SortMode));
                Thread.Sleep(1000);
            }

            if (CurrentScene.SceneState != KancolleSceneStates.Repair_SortByNew)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景状态【{CurrentScene.SceneState}】与预期场景状态【{KancolleSceneStates.Repair_SortByNew}】不符", UnexceptedState);

            int index = indexOfShips(sortedShip, task.ShipNo);
            if (index == -1)//没找到
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未在已有舰娘中找到舰娘序号【{task.ShipNo}】", UnfoundShipNo);

            int maxPage = sortedShip.Length / 10;//最大的页数
            int page = index / 10;//所在的页数，每页最多可以放10个
            Tuple<int, int> pageTurn = getPageTurn(maxPage, page);
            int page5 = pageTurn.Item1;//每5页翻一次
            int pageIn5 = pageTurn.Item2;//5页翻完后还有几页
            int item = index % 10;//每页第几个
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_Ships_FirstPage));//转到第1页

            Thread.Sleep(1000);
            for (int p = 0; p < page5; p++)//先5页5页的翻
            {
                actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_Ships_Next5Page));
                Thread.Sleep(1000);
            }


            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_Ships_Pages[pageIn5]));
            Thread.Sleep(1000);

            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_ShipList[item]));
            Thread.Sleep(500);
            if(CurrentScene.SceneState != KancolleSceneStates.Repair_Start_True)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"当前场景状态【{CurrentScene.SceneState}】与预期场景状态【{KancolleSceneStates.Repair_Start_True}】不符", UnexceptedState);

            if (task.UseFastRepair)
                actionExecutor.Execute(KancolleWidgetPositions.Repair_Start_UseFastRepair);
            Thread.Sleep(500);
            actionExecutor.Execute(new KancolleAction(KancolleWidgetPositions.Repair_Start));
            Thread.Sleep(1000);
            bool result = LockNowAndWaitForResponse(new KancolleAction(KancolleWidgetPositions.Repair_Start_Decide));
            if(!result)
                return new KancolleTaskResult(task, KancolleTaskResultType.Fail, $"未能获得服务端响应", NoResponse);
            Thread.Sleep(500);
            return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"入渠舰娘【{task.ShipNo}】于【{task.Dock}】成功", Success);
        }

        private KancolleTaskResult RefreshData(RefreshDataTask task)
        {
            KancolleTaskResult result;
            if(CurrentScene == KancolleSceneTypes.Port)
            {
                result = ReachScene(KancolleSceneTypes.Organize);
                if (result.IsSuccess)
                {
                    Thread.Sleep(1000);
                    result = ReachScene(KancolleSceneTypes.Port);
                }
            }
            else
            {
                result = ReachScene(KancolleSceneTypes.Port);
            }

            if (result.IsSuccess)
                return new KancolleTaskResult(task, KancolleTaskResultType.Success, $"刷新成功", Success);
            else
                return result;
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
        private KancolleTaskResult ReachScene(KancolleSceneTypes toScene)
        {
            var edges = navigator.Navigate(CurrentScene.SceneType, toScene);
            if (edges == null)
                return new KancolleTaskResult(null, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】无法到达场景【{toScene.ToString()}】，无通向目的地的路径", NoNavigationPath);//无法到达界面
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
                            return new KancolleTaskResult(null, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】无法到达场景【{toScene.ToString()}】，无通向目的地的路径", NoNavigationPath);//无法到达界面
                        isFinished = false;//中途有变，所以重新切换界面
                        break;

                    }
                    Thread.Sleep(1000);
                }
            }
            if (CurrentScene.SceneType == toScene)
            {
                return new KancolleTaskResult(null, KancolleTaskResultType.Success, $"成功到达场景【{toScene}】", Success);
            }
            else
                return new KancolleTaskResult(null, KancolleTaskResultType.Fail, $"当前场景【{CurrentScene.SceneType}】无法到达场景【{toScene.ToString()}】，实际结果与导航预期不符", UnexceptedScene);//无法到达界面
        }

        /// <summary>
        /// 线程阻塞，一直等到新的场景出现
        /// 如果新的场景为edge.Target，返回true，否则为false
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="timeout"></param>
        /// <returns>如果新的场景为scene，返回true，否则为false</returns>
        private bool DoActionAndWaitForScene(KancolleActionEdge edge, double timeout)
        {
            if (!actionExecutor.Execute(edge))//未能执行成功
                return false;
            if (edge.Target == KancolleSceneTypes.Quest)
            {
                Thread.Sleep(2000);//等待明石动画
                if (!actionExecutor.Execute(edge))//点掉明石
                    return false;
            }
            return WaitForScene(edge.Target, timeout);
        }

        /// <summary>
        /// 线程阻塞，一直等到有response响应
        /// </summary>
        /// <returns></returns>
        private bool LockNowAndWaitForResponse(KancolleAction action)
        {
            lock (actionTimeStampLock)
            {
                currentResponse = null;
                actionTimeStamp = DateTime.Now;
            }
            actionExecutor.Execute(action);
            DateTime start = DateTime.Now;
            while ((currentResponse == null || currentResponse.Time <= actionTimeStamp) && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
            {
                Thread.Sleep(500);
            }

            if (currentResponse != null && currentResponse.Time > actionTimeStamp)
                return true;
            return false;
        }

        private Battle WaitForBattle(KancolleAction action)
        {
            currentBattle = null;
            actionExecutor.Execute(action);
            DateTime start = DateTime.Now;
            while (currentBattle == null  && (DateTime.Now - start).TotalMilliseconds <= ActionTimeout)
            {
                Thread.Sleep(500);
            }
            return currentBattle;
        }

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            lock (actionTimeStampLock)
            {
                lastResponse = currentResponse;
                currentResponse = new ResponseHelper(requestInfo, response, api);
            }
        }

        public void OnBattleHappenedHandler(Battle obj)
        {
            currentBattle = obj;
        }
       
    }
}
