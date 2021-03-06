﻿using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleAI;
using WoFlagship.Logger;
using WoFlagship.KancollePlugin;
using WoFlagship.ToolWindows;
using WoFlagship.ViewModels;
using WoFlagship.KancolleCore.Navigation;
using WoFlagship.KancolleCore.KancolleBattle;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace WoFlagship
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ChromiumWebBrowser webView;
        private SceneRecognizer sceneRecognizer = new SceneRecognizer();
        private PluginManager pluginManager = new PluginManager();
        private List<PluginWindow> pluginWindowList = new List<PluginWindow>();
        private AIManager aiManager = new AIManager();


        private Xceed.Wpf.Toolkit.SplitButton pluginButton = null;
        private Grid pluginGrid = null;

        private Xceed.Wpf.Toolkit.SplitButton aiButton = null;
        private Grid aiGrid = null;

        private SettingViewModel settingViewModel = new SettingViewModel();
        private GeneralViewModel generalViewModel = new GeneralViewModel();
        private MainInfoViewModel mainInfoViewModel = new MainInfoViewModel();

        private ObservableCollection<string> pluginNames { get; set; } = new ObservableCollection<string>();
        private IKancollePlugin currentPlugin = null;
        private ObservableCollection<string> aiNames { get; set; } = new ObservableCollection<string>();
        private IKancolleAI currentAI = null;

        private KancolleRequestHandler requsetHandler = new KancolleRequestHandler();

        private StreamWriter sw = new StreamWriter("outputAPI.txt");

        private readonly KancolleGameContext gameContext;
        private readonly KancolleBattleContext battleContext;

        private INavigator navigator = new SimpleNavigator();
        private KancolleActionExecutor actionExecutor;

        private DispatcherTimer timer;

        public MainWindow()
        {
            gameContext = new KancolleGameContext(GetCurrentScene);
            //初始化事件
            InitContextEvent();

            battleContext = new KancolleBattleContext(KancolleGameData.Instance);

            LogFactory.SystemLogger.Info("程序启动");
            

            aiManager.OnAILoaded += AiManager_OnAILoaded;
            aiManager.LoadAIs();

            pluginManager.OnPluginsLoaded += PluginManager_OnPluginsLoaded;
            pluginManager.LoadPlugins();

            settingViewModel.WebSetting = new WebSettingViewModel();
            settingViewModel.WebSetting.ProxyHost = "127.0.0.1";
            settingViewModel.WebSetting.ProxyPort = 8099;
            settingViewModel.WebSetting.ProxyType = ProxyTypes.Http;

            InitializeComponent();
            LogFactory.ConsoleLogger = new ConsoleLogger(Txt_MainLogger);

            battleContext.OnBattleHappened += BattleContext_OnBattleHappened;
           

            Cbx_ProxyType.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Source = settingViewModel.WebSetting, Path = new PropertyPath("ProxyType") });
            Txt_ProxyHost.SetBinding(TextBox.TextProperty, new Binding() { Source = settingViewModel.WebSetting, Path = new PropertyPath("ProxyHost") });
            Txt_ProxyPort.SetBinding(TextBox.TextProperty, new Binding() { Source = settingViewModel.WebSetting, Path = new PropertyPath("ProxyPort") });

            Txt_Level.SetBinding(Label.ContentProperty, new Binding() { Source=generalViewModel, Path=new PropertyPath("Level")});
            Txt_NickName.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("NickName") });
            Txt_ShipCount.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("ShipCount") });
            Txt_MaxShipCount.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("MaxShipCount") });
            Txt_ItemCount.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("ItemCount") });
            Txt_MaxItemCount.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("MaxItemCount") });
            Txt_Ran.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path=new PropertyPath("Ran")});
            Txt_Dan.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Dan") });
            Txt_Gang.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Gang") });
            Txt_Lv.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Lv") });
            Txt_Gaixiu.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Gaixiu") });
            Txt_Kaifa.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Kaifa") });
            Txt_Jianzao.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Jianzao") });
            Txt_Xiufu.SetBinding(Label.ContentProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Xiufu") });
            Lb_Mission.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("QuestList") });
            Tc_Deck.SetBinding(TabControl.ItemsSourceProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Decks") });
            Lb_Dock.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = generalViewModel, Path = new PropertyPath("Docks") });

            Grid_CurrentTask.SetBinding(Grid.DataContextProperty, new Binding() { Source = mainInfoViewModel, Path = new PropertyPath("CurrentTask") });
            Lb_Task.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = mainInfoViewModel, Path = new PropertyPath("TaskList") });
      
            LogFactory.SystemLogger.Info("开始插件初始化");
            foreach (var plugin in pluginManager.Plugins)
            {
                try
                {
                    plugin.OnInit(generalViewModel);
                    LogFactory.SystemLogger.Info($"插件'{plugin.Name}'初始化完成");
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Plugin init\n" + ex.Message);

                    LogFactory.SystemLogger.Error($"插件'{plugin.Name}'初始化失败", ex);
                }
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (generalViewModel.Docks != null && KancolleGameData.Instance != null)
            {
                for (int i = 0; i < KancolleGameData.Instance.DockArray.Count; i++)
                {
                    var dock = KancolleGameData.Instance.DockArray[i];
                    if(dock.State > 0)
                    {
                        TimeSpan remainTime;
                        if (dock.CompleteTime > DateTime.Now)
                        {
                            remainTime = dock.CompleteTime - DateTime.Now;
                        }
                        else {
                            remainTime = TimeSpan.FromTicks(0);
                        }
                        generalViewModel.Docks[i].RemainingTime = String.Format("{0:00}:{1:00}:{2:00}", (int)(remainTime.TotalHours), remainTime.Minutes, remainTime.Seconds); 
                    }
                }
            }
        }

        private void InitContextEvent()
        {
            gameContext.OnGameDataUpdated += e =>
            {
                UpdateViewModel(KancolleGameData.Instance);

                foreach (var plugin in pluginManager.Plugins)
                {
                    plugin.OnGameDataUpdated(generalViewModel, KancolleGameData.Instance);
                }

                if(currentAI != null)
                {
                    currentAI.OnGameDataUpdated(KancolleGameData.Instance);
                }
            };
        }

        private void UpdateViewModel(KancolleGameData gameData)
        {
            //基本信息
            generalViewModel.Level = "Lv." + gameData.BasicInfo.Level;
            generalViewModel.Rank = gameData.BasicInfo.Rank;
            generalViewModel.NickName = gameData.BasicInfo.NickName;
            generalViewModel.ShipCount = gameData.BasicInfo.OwnedShipCount;
            generalViewModel.MaxShipCount = gameData.BasicInfo.MaxShipCount;
            generalViewModel.MaxItemCount = gameData.BasicInfo.MaxSlotItemCount;

            //资源
            generalViewModel.Ran =gameData.Material.Ran;
            generalViewModel.Dan = gameData.Material.Dan;
            generalViewModel.Gang = gameData.Material.Gang;
            generalViewModel.Lv = gameData.Material.Lv;
            generalViewModel.Jianzao = gameData.Material.Jianzao;
            generalViewModel.Gaixiu =gameData.Material.Gaixiu;
            generalViewModel.Xiufu = gameData.Material.Xiufu;
            generalViewModel.Kaifa = gameData.Material.Kaifa;

            //编成
            for (int i = 0; i < generalViewModel.Decks.Length; i++)
            {
                generalViewModel.Decks[i].Name = (i + 1) + "";
                for (int j = 0; j < generalViewModel.Decks[i].Ships.Length; j++)
                {
                    int ownedShipId = gameData.OwnedShipPlaceArray[i, j];
                    if (ownedShipId > 0)
                    {
                        int shipId = gameData.OwnedShipDictionary[ownedShipId].ShipId;
                        generalViewModel.Decks[i].Ships[j].Name = gameData.ShipDataDictionary[shipId].Name;
                        generalViewModel.Decks[i].Ships[j].Slots = (from slotId in gameData.OwnedShipDictionary[ownedShipId].Slot
                                                                    where slotId>0
                                                                    select gameData.GetSlotItemName(slotId)).ToArray();
                    }
                    else
                        generalViewModel.Decks[i].Ships[j].Reset();
                }


            };

            //任务相关
            var runningQuest = (from q in gameData.QuestDictionary
                                where q.Value.State > 1
                                orderby q.Value.Id
                                select q.Value).ToArray();

            for (int i = 0; i < generalViewModel.QuestList.Length; i++)
            {
                if (i < runningQuest.Length)
                {
                    var quest = runningQuest[i];
                    generalViewModel.QuestList[i].Id = quest.Id;
                    generalViewModel.QuestList[i].Name = quest.Title;
                    generalViewModel.QuestList[i].Detail = quest.Detail;
                    generalViewModel.QuestList[i].State = quest.State;
                    generalViewModel.QuestList[i].ProgressFlag = quest.ProgressFlag;
                }
                else
                    generalViewModel.QuestList[i].Reset();
            }

            //入渠相关
            for(int i=0; i<gameData.DockArray.Count; i++)
            {
                var dock = gameData.DockArray[i];
                if (dock.State == -1)
                {
                    generalViewModel.Docks[i].Lock();
                }
                else if (dock.State == 0)
                {
                    generalViewModel.Docks[i].Empty();
                }
                else
                {
                    generalViewModel.Docks[i].ShipName = gameData.GetShipName(dock.ShipId);
                    generalViewModel.Docks[i].CompleteTime = dock.CompleteTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }


        private void AiManager_OnAILoaded(List<IKancolleAI> obj)
        {
            aiNames.Clear();
            //aiNames.Add("手动控制");
            foreach(var ai in obj)
            {
                aiNames.Add(ai.Name);
            }
        }

        private void BattleContext_OnBattleHappened(Battle obj)
        {
            string battleText = obj.ToString();

            battleText += "我方第一舰队最终状态\n";
            foreach(var ship in obj.MainFleet)
            {
                if (ship != null && ship.ShipId > 0)
                    battleText += KancolleGameData.Instance.ShipDataDictionary[ship.ShipId].Name + "\t" + ship.NowHP + "/" + ship.MaxHP + "\n";
            }

            if(obj.EscortFleet != null)
            {
                battleText += "我方第二舰队最终状态\n";
                foreach (var ship in obj.EscortFleet)
                {
                    if (ship != null && ship.ShipId > 0)
                        battleText += KancolleGameData.Instance.ShipDataDictionary[ship.ShipId].Name + "\t" + ship.NowHP + "/" + ship.MaxHP + "\n";
                }
            }

            battleText += "敌方第一舰队最终状态\n";
            foreach (var ship in obj.EnemyFleet)
            {
                if (ship != null && ship.ShipId>0)
                    battleText += KancolleGameData.Instance.ShipDataDictionary[ship.ShipId].Name + "\t" + ship.NowHP + "/" + ship.MaxHP + "\n";
            }

            if (obj.EnemyEscort != null)
            {
                battleText += "敌方第二舰队最终状态\n";
                foreach (var ship in obj.EnemyEscort)
                {
                    if (ship != null && ship.ShipId > 0)
                        battleText += KancolleGameData.Instance.ShipDataDictionary[ship.ShipId].Name + "\t" + ship.NowHP + "/" + ship.MaxHP + "\n";
                }
            }

            Dispatcher.Invoke(new Action(() => Txt_Battle.Text = battleText));
        }

        private void PluginManager_OnPluginsLoaded(List<IKancollePlugin> obj)
        {
            pluginNames.Clear();
            foreach(var plugin in obj)
            {
                pluginNames.Add(plugin.Name);
            }
        }

        private void Panel_MainBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            var setting = new CefSharp.CefSettings();
            setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";

            //settings
            if (settingViewModel.WebSetting.ProxyType == ProxyTypes.Http)
                 setting.CefCommandLineArgs.Add("--proxy-server", settingViewModel.WebSetting.ProxyHost + ":" + settingViewModel.WebSetting.ProxyPort);
            setting.CefCommandLineArgs.Add("ppapi-flash-version", "23.0.0.207");
            setting.CefCommandLineArgs.Add("ppapi-flash-path", "Resources\\PepperFlash\\pepflashplayer.dll");
            setting.CefCommandLineArgs.Add("ppapi-flash-args", "enable_hw_video_decode=1");
            setting.IgnoreCertificateErrors = true;
            setting.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WoFlagship";
            CefSharp.Cef.Initialize(setting, true, null);

            webView = new ChromiumWebBrowser();
            
            Panel_MainBrowser.Children.Add(webView);
            webView.StatusMessage += WebView_StatusMessage;
            webView.IsBrowserInitializedChanged += WebView_IsBrowserInitializedChanged;
            requsetHandler.OnAPIResponseReceived += RequsetHandler_OnAPIResponseReceived;
            webView.RequestHandler = requsetHandler;
            webView.FrameLoadEnd += WebView_FrameLoadEnd;
            webView.LoadError += WebView_LoadError;
            webView.MouseMove += WebView_MouseMove;

            actionExecutor = new KancolleActionExecutor(webView);
            actionExecutor.OnActionExecuted += ActionExecutor_OnActionExecuted;
            
            try
            {
                //创建单例类TaskExecutor，只能初始化这一次！访问方式为 KancolleTaskExecutor.Get()
                new KancolleTaskExecutor(actionExecutor, () => GetCurrentScene());
                KancolleTaskExecutor.Instance.OnTaskStart_Internal += TaskExecutor_OnTaskStart_Internal;
                KancolleTaskExecutor.Instance.OnTaskFinished_Internal += TaskExecutor_OnTaskFinished_Internal;
                KancolleTaskExecutor.Instance.OnTasksChanged_Internal += TaskExecutor_OnTasksChanged_Internal;
                KancolleTaskExecutor.Instance.Start();

                battleContext.OnBattleHappened += KancolleTaskExecutor.Instance.OnBattleHappenedHandler;
            }
            catch(Exception ex)
            {
#if DEBUG
                MessageBox.Show("taskExecutor启动失败\n" + ex.Message);
#endif
            }
        }

        private void TaskExecutor_OnTaskStart_Internal(KancolleTaskExecutor obj)
        {
            mainInfoViewModel.CurrentTask = new TaskViewModel() { Task = obj.RunningTask};
        }

        private void TaskExecutor_OnTasksChanged_Internal(KancolleTaskExecutor obj, NotifyCollectionChangedEventArgs args)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        TaskViewModel task = new TaskViewModel();
                        task.Task = args.NewItems[0] as KancolleTask;
                        mainInfoViewModel.TaskList.Add(task);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        mainInfoViewModel.TaskList.RemoveAt(0);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        mainInfoViewModel.TaskList.Clear();
                        break;
                    default:
                        break;
                }

                mainInfoViewModel.CurrentTask = new TaskViewModel() { Task = obj.RunningTask };
            }));     
        }


        private void ActionExecutor_OnActionExecuted(KancolleAction obj)
        {
            //KancolleGameData.Instance.CurrentScene = GetCurrentScene();
        }

        private void TaskExecutor_OnTaskFinished_Internal(KancolleTaskExecutor arg1, KancolleTaskResult arg2)
        {
            Dispatcher.Invoke(new Action(()=>
            {
                Run line = new Run(arg2.ToString() + "\n");
                if (arg2.IsSuccess)
                    line.Foreground = Brushes.Green;
                else
                    line.Foreground = Brushes.Red;
                Txt_MainLogger.Inlines.Add(line);
                if(Txt_MainLogger.Parent is ScrollViewer)
                {
                    ScrollViewer sv = Txt_MainLogger.Parent as ScrollViewer;
                    sv.ScrollToEnd();
                }

                mainInfoViewModel.CurrentTask = new TaskViewModel() { Task = arg1.RunningTask };
            }));
        }

        public RenderTargetBitmap GetWebViewBitmap()
        {
            var rtb = new RenderTargetBitmap((int)webView.ActualWidth, (int)webView.ActualHeight, 96, 96, PixelFormats.Default);
            rtb.Render(webView);
            return rtb;
        }

        private void WebView_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(webView);
            Point desktop_position = webView.PointToScreen(p);
            Txt_MousePosition.Content = p.X + "," + p.Y + " | " + desktop_position.X + "," + desktop_position.Y;
        }

        private void WebView_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           if((bool)e.NewValue)
            {
                webView.Address = KancolleCore.DMMUrls.KanColleUrl;
            }
        }

        private void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.Name == "game_frame")
            {
                //在这里修正偏移
                e.Frame.ExecuteJavaScriptAsync("document.getElementById('spacing_top').style.height='0px'");
            }
        }

        private void RequsetHandler_OnAPIResponseReceived(RequestInfo arg1, string arg2)
        {
            try
            {
                sw.WriteLine(arg1.RequestUrl + " " + arg1.DataString + " \n" + arg2);
                sw.Flush();
                Dispatcher.Invoke(new System.Action(() => Txt_Console.Content = arg1.RequestUrl.Substring(KancolleCore.DMMUrls.KanColleAPIUrl.Length)));
                if (arg2.StartsWith("svdata="))
                    arg2 = arg2.Substring(7);

                var data = JsonConvert.DeserializeObject(arg2) as JObject;
                var api_object = data["api_data"];
                string api = KancolleAPIs.GetAPI(arg1.RequestUrl);//= arg1.RequestUrl.Substring(KancolleCommon.DMMUrls.KanColleAPIUrl.Length);
                if(api == "api_start2")
                {
                    foreach (var plugin in pluginManager.Plugins)
                    {
                        plugin.OnGameStart(generalViewModel, KancolleGameData.Instance);
                    }
                }
                gameContext.OnAPIResponseReceivedHandler(arg1, arg2, api);
                switch (api)
                {
                    case "api_get_member/require_info":
                        var getmember_data = api_object.ToObject<api_requireinfo_data>();
                        generalViewModel.ItemCount = getmember_data.api_slot_item.Length;
                        break;
                    case "api_req_kousyou/createitem"://开发装备
                        var createitem_data = api_object.ToObject<api_createitem_data>();
                        if (createitem_data.api_create_flag == 1)
                            generalViewModel.ItemCount++;
                        break;
                    case "api_req_kousyou/destroyitem2":
                        var sv_data_destroyitem2 = JsonConvert.DeserializeObject<KancolleCore.destroyitem2_api.svdata>(arg2);
                        if (sv_data_destroyitem2 != null)
                            generalViewModel.ItemCount--;
                        break;
                }

                battleContext.OnAPIResponseReceivedHandler(arg1, arg2, api);
                KancolleTaskExecutor.Instance.OnAPIResponseReceivedHandler(arg1, arg2, api);
            }
            catch(Exception ex)
            {
                MessageBox.Show("API处理错误\n" + ex.Message + "\n" + ex.StackTrace);
                LogFactory.SystemLogger.Error($"API'{arg1.RequestUrl}'处理错误![{arg2}]", ex);
            }
            
        }

        private void WebView_StatusMessage(object sender, CefSharp.StatusMessageEventArgs e)
        {
           
           // MessageBox.Show("status\n"+e.Item2);
        }

        private void WebView_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            string errors = "";
            errors = e.ErrorText;
            LogFactory.SystemLogger.Warn($"网络连接错误！[{e.FailedUrl}][{e.ErrorCode}][{e.ErrorText}]");
        }

        private void Cbx_ProxyType_Loaded(object sender, RoutedEventArgs e)
        {
            Cbx_ProxyType.ItemsSource = Enum.GetValues(typeof(ProxyTypes));
           
        }

       


        private void Btn_ShowDevTool_Click(object sender, RoutedEventArgs e)
        {
            if(webView != null)
            {
                CefSharp.WebBrowserExtensions.ShowDevTools(webView);
            }
        }

        private void Btn_Reload_Click(object sender, RoutedEventArgs e)
        {
            if(webView != null)
            {
                webView.Reload();
            }
        }

        private void Btn_RefreshFilter_Click(object sender, RoutedEventArgs e)
        {
            var screen = GetWebViewBitmap();
            //Img_Filter.Source = screen;
            
            var sceneType = sceneRecognizer.GetSceneTypeFromBitmap(ToBitmap(screen));
            Txt_CurrentScene.Content = sceneType.ToString();
        }

        private KancolleScene GetCurrentScene()
        {
            return Dispatcher.Invoke(() =>
            {
                var screen = GetWebViewBitmap();
                return sceneRecognizer.GetSceneTypeFromBitmap(ToBitmap(screen));
            });
            
        }

        public System.Drawing.Bitmap ToBitmap(RenderTargetBitmap rbitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rbitmap));
                encoder.Save(stream);
                return new System.Drawing.Bitmap(stream);
            }
        }

        private void Btn_CaptureScreen_Click(object sender, RoutedEventArgs e)
        {
           ScreenCapture("Capture\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bmp");
        }

        /// <summary>
        /// 保存为bmp图片截图
        /// </summary>
        /// <param name="savedPath"></param>
        public void ScreenCapture(string savedPath)
        {
            var screen = GetWebViewBitmap();
            var encoder = new BmpBitmapEncoder();
            var frame = BitmapFrame.Create(screen);
            encoder.Frames.Add(frame);
            FileInfo fi = new FileInfo(savedPath);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            using (Stream stream = File.Create(savedPath))
            {
                encoder.Save(stream);

                if (File.Exists(savedPath))
                {
                    Process.Start(savedPath);
                }
            }
        }

        private void Btn_ReloadPlugin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var plugin in pluginManager.Plugins)
                {
                    plugin.OnInit(generalViewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Plugin init\n" + ex.Message);
            }
        }

        private void Btn_QuestEditor_Click(object sender, RoutedEventArgs e)
        {
            QuestEditor questEditor = new QuestEditor(KancolleGameData.Instance);
            questEditor.ShowDialog();
        }


        private string ToNavigationString(List<KancolleActionEdge> edges)
        {
            if (edges == null)
                return "无法到达";
            else if (edges.Count == 0)
                return "已经到达";
            else
            {
                string output = edges[0].Source + "";
                foreach(var edge in edges)
                {
                    output += " -> " + edge.Target;
                }
                return output;
            }
        }

        private void Btn_NavigateTo_Click(object sender, RoutedEventArgs e)
        {
            KancolleSceneTypes toSceneType = (KancolleSceneTypes)Cbx_NavigateTarget.SelectedItem;
            var edges = navigator.Navigate(GetCurrentScene().SceneType, toSceneType);
            Txt_Navigation.Text = ToNavigationString(edges);
            /*
           switch(toScene)
            {
                case Scenes.SupplyMain:
                    kancolleAI.EnqueueTask(new SupplyTask(0));
                    break;
                case Scenes.Quest:
                    kancolleAI.EnqueueTask(new QuestTask(null));
                    break;
                case Scenes.Mission:
                    kancolleAI.EnqueueTask(new MissionTask(1));
                    break;
            }*/
        }

        private void Cbx_NavigateTarget_Loaded(object sender, RoutedEventArgs e)
        {
            Cbx_NavigateTarget.ItemsSource = Enum.GetValues(typeof(KancolleSceneTypes));
            Cbx_NavigateTarget.SelectedIndex = 0;
        }

        private void Btn_Plugins_Click(object sender, RoutedEventArgs e)
        {
            Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_Plugins");
        }

        private void Lb_Plugins_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb.ItemsSource = pluginNames;
        }

        private void Lb_Plugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedValue == null)
                return;
            var plugin = pluginManager.Plugins.Find(p => p.Name == lb.SelectedValue.ToString());

            if(plugin != null)
            {
                if (plugin.NewWindow)
                {
                    var win = pluginWindowList.Find((w) => w.Plugin.Name == plugin.Name);
                    if (win != null)
                    {
                        win.Show();
                        win.Focus();
                    }
                    else
                    {
                        PluginWindow pw = new PluginWindow(plugin); 
                        pw.Show();
                        pluginWindowList.Add(pw);
                    }
                }
                else
                {
                    pluginGrid.Children.Clear();
                    if(plugin.PluginPanel != null)
                        pluginGrid.Children.Add(plugin.PluginPanel);
                    Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_Plugins");
                    pluginButton.Content = plugin.Name;
                }
                currentPlugin = plugin;
            }

            lb.UnselectAll();

            if (pluginButton != null)
                pluginButton.IsOpen = false;

        }

        private void Btn_Plugins_Loaded(object sender, RoutedEventArgs e)
        {
            pluginButton = sender as Xceed.Wpf.Toolkit.SplitButton;
        }

        private void Gird_PluginPanel_Loaded(object sender, RoutedEventArgs e)
        {
            pluginGrid = sender as Grid;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KancolleTaskExecutor.Instance?.Stop();
            sw.Close();
        }

        private void Btn_AIs_Click(object sender, RoutedEventArgs e)
        {
            Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_AIs");
        }

        private void Btn_AIs_Loaded(object sender, RoutedEventArgs e)
        {
            aiButton = sender as Xceed.Wpf.Toolkit.SplitButton; ;
        }

        private void Lb_AIs_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb.ItemsSource = aiNames;
        }

        private void Lb_AIs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedValue == null)
                return;

            var ai = aiManager.AIs.Find(p => p.Name == lb.SelectedValue.ToString());

            //切换至当前的ai
            if (ai == currentAI)
            {
                //就相当于切换tab
                Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_AIs");
            }
            else
            {
                //切换至新ai
                if (lb.SelectedIndex >= 0)//=0的情况下，永远不会有手动项，因为手动项已经被添加到了一般AI
                {
                    if (GetCurrentScene() != KancolleSceneTypes.Port)
                    {
                        MessageBox.Show("请将当前场景切换至[母港]再更改AI！");
                    }
                    else
                    {
                        if(currentAI == null)//手动控制
                        {
                            Grid_ManualAI.IsEnabled = false;
                            Grid_ManualAI.Visibility = Visibility.Collapsed;
                            aiGrid.Visibility = Visibility.Visible;
                        }
                        else//新ai
                        {
                            currentAI.Stop();
                            
                        }

                        aiGrid.Children.Clear();
                        if(ai.AIPanel != null)
                            aiGrid.Children.Add(ai.AIPanel);
                        currentAI = ai;
                        try
                        {
                            currentAI.Start();
                            currentAI.OnGameDataUpdated(KancolleGameData.Instance);                        
                            aiButton.Content = currentAI.Name;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("AI Change error\n" + ex.Message);
                            LogFactory.SystemLogger.Error($"AI切换失败", ex);
                            currentAI = null;
                        }
                        
                    }
                    
                }
                //else//切换至手动
                //{
                //    currentAI.Stop();
                //    aiGrid.Visibility = Visibility.Collapsed;
                //    Grid_ManualAI.Visibility = Visibility.Visible;
                //    Grid_ManualAI.IsEnabled = true;
                //    currentAI = null;
                //    aiButton.Content = aiNames[0];
                //}

                Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_AIs");
                
            }

            lb.UnselectAll();

            aiButton.IsOpen = false;
        }

        private void Grid_AIPanel_Loaded(object sender, RoutedEventArgs e)
        {
            aiGrid = sender as Grid;
        }

        private void Btn_MapEditor_Click(object sender, RoutedEventArgs e)
        {
            MapEditor me = new MapEditor();
            me.ShowDialog();
        }

        //重载ai
        private void Btn_AIs_ReLoadAIs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentAI != null)
                {
                    currentAI.Stop();
                }

                aiManager.LoadAIs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Reload\n" + ex.Message);
                LogFactory.SystemLogger.Error($"AI重载失败", ex);
            }
        }
    }
}
