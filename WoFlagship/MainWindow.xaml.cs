using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WoFlagship.KancolleCommon;
using WoFlagship.KancolleAI;
using WoFlagship.KancolleNavigation;
using WoFlagship.Logger;
using WoFlagship.Plugins;
using WoFlagship.ToolWindows;
using WoFlagship.ViewModels;
using WoFlagship.KancolleBattle;
using System.Threading;
using WoFlagship.KancolleQuest;

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

        private ObservableCollection<string> pluginNames { get; set; } = new ObservableCollection<string>();
        private IPlugin currentPlugin = null;
        private ObservableCollection<string> aiNames { get; set; } = new ObservableCollection<string>();
        private IKancolleAI currentAI = null;

        private KancolleRequestHandler requsetHandler = new KancolleRequestHandler();

        private StreamWriter sw = new StreamWriter("outputAPI.txt");

        private KancolleGameData gameData = new KancolleGameData();
        

        private INavigator navigator = new SimpleNavigator();
        private KancolleActionExecutor actionExecutor;
        private KancolleBattleManager battleManager = new KancolleBattleManager();


        private KancolleTaskExecutor taskExecutor;

        public MainWindow()
        {
            ///初始化资源
            InitResources();

            LogFactory.SystemLogger.Info("程序启动");
            pluginManager.OnPluginsLoaded += PluginManager_OnPluginsLoaded;
            pluginManager.LoadPlugins();

            aiManager.OnAILoaded += AiManager_OnAILoaded;
            aiManager.LoadAIs();

            settingViewModel.WebSetting = new WebSettingViewModel();
            settingViewModel.WebSetting.ProxyHost = "127.0.0.1";
            settingViewModel.WebSetting.ProxyPort = 8099;
            settingViewModel.WebSetting.ProxyType = ProxyTypes.Http;
            InitializeComponent();

            battleManager.OnBattleHappened += BattleManager_OnBattleHappened;

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
#if DEBUG
                    MessageBox.Show("Plugin init\n" + ex.Message);
#endif
                    LogFactory.SystemLogger.Error($"插件'{plugin.Name}'初始化失败", ex);
                }
            }
        }

        private void InitResources()
        {
            InitQuestInfo();
        }

        private void InitQuestInfo()
        {
            if (!File.Exists(KancolleGameData.QuestInfoFile))
            {
                MessageBox.Show($"未能找到资源文件[{KancolleGameData.QuestInfoFile}]");
                LogFactory.SystemLogger.Error($"未能找到资源文件[{KancolleGameData.QuestInfoFile}]");
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(KancolleGameData.QuestInfoFile))
                    {
                     
                        string content = sr.ReadToEnd();
                        var questInfoObject = JsonConvert.DeserializeObject(content) as JToken;
                        int version = questInfoObject["Version"].ToObject<int>();
                        string updateTime = questInfoObject["UpdateTime"].ToString();
                        gameData.QuestInfoDic.Clear();
                        foreach (var quest in questInfoObject["QuestInfos"])
                        {
                            QuestInfoViewModel qi = new QuestInfoViewModel()
                            {
                                Id = quest["Id"].ToString(),
                                Name = quest["Name"].ToString(),
                                Detail = quest["Detail"].ToString(),
                                Ran = quest["Ran"].ToObject<int>(),
                                Dan = quest["Dan"].ToObject<int>(),
                                Gang = quest["Gang"].ToObject<int>(),
                                Lu = quest["Lu"].ToObject<int>(),
                                Other = quest["Other"].ToString(),
                                Note = quest["Note"].ToString(),
                                GameId = quest["GameId"].ToObject<int>(),
                                Prerequisite = quest["Prerequisite"].ToObject<int[]>(),
                                Category = quest["Category"].ToString(),
                            };

                            IQuestRequirement re = null;
                            bool unknownCat = false;
                            switch (qi.Category)
                            {
                                case "sortie":
                                     re = quest["Requirements"].ToObject<SortieQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "excercise":
                                     re = quest["Requirements"].ToObject<ExerciseQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "expedition":
                                     re = quest["Requirements"].ToObject<ExpeditionQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "equipexchange":
                                     re = quest["Requirements"].ToObject<EquipexchangeQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "modernization":
                                     re = quest["Requirements"].ToObject<ModernizationQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "fleet":
                                    re = quest["Requirements"].ToObject<FleetQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "sink":
                                     re = quest["Requirements"].ToObject<SinkQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "and":
                                     re = quest["Requirements"].ToObject<AndQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "simple":
                                     re = quest["Requirements"].ToObject<SimpleQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "a-gou":
                                    qi.Requirements = new AGouQuestRequirement();
                                    break;
                                case "modelconversion":
                                     re = quest["Requirements"].ToObject<ModelconversionQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                case "scrapequipment":
                                     re = quest["Requirements"].ToObject<ScrapequipmentQuestRequirement>();
                                    qi.Requirements = re;
                                    break;
                                default:
                                    if (!string.IsNullOrEmpty(qi.Category))
                                    {
                                        MessageBox.Show($"未知任务类型[{qi.Category}]!");
                                        LogFactory.SystemLogger.Warn($"未知任务类型[{qi.Category}]!");
                                    }
                                    unknownCat = true;
                                    break;
                            }
                            if (!unknownCat)
                            {
                                qi.Requirements = re;
                                gameData.QuestInfoDic.Add(qi.GameId, qi);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"任务信息资源初始化失败！\n{ex.Message}");
                    LogFactory.SystemLogger.Error("任务信息资源初始化失败！", ex);
                }
            }
        }

        private void AiManager_OnAILoaded(List<IKancolleAI> obj)
        {
            aiNames.Clear();
            aiNames.Add("手动控制");
            foreach(var ai in obj)
            {
                aiNames.Add(ai.Name);
            }
        }

        private void BattleManager_OnBattleHappened(KancolleBattle.KancolleBattle obj)
        {
            Dispatcher.Invoke(new Action(() => Txt_Battle.Text = obj.ToString()));
        }

        private void PluginManager_OnPluginsLoaded(List<IPlugin> obj)
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
           
            webView.LoadError += WebView_LoadError;
            webView.MouseMove += WebView_MouseMove;

            actionExecutor = new KancolleActionExecutor(webView);
            try
            {
                taskExecutor = new KancolleTaskExecutor(webView, () => GetCurrentScene(), () => gameData);
                taskExecutor.Start();
            }
            catch(Exception ex)
            {
#if DEBUG
                MessageBox.Show("taskExecutor启动失败\n" + ex.Message);
#endif
            }
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
                webView.Address = KancolleCommon.DMMUrls.KanColleUrl;
            }
        }

        private void RequsetHandler_OnAPIResponseReceived(RequestInfo arg1, string arg2)
        {
            try
            {
                sw.WriteLine(arg1.RequestUrl + " " + arg1.DataString + " \n" + arg2);
                sw.Flush();
                Dispatcher.Invoke(new System.Action(() => Txt_Console.Content = arg1.RequestUrl.Substring(KancolleCommon.DMMUrls.KanColleAPIUrl.Length)));
                if (arg2.StartsWith("svdata="))
                    arg2 = arg2.Substring(7);

                var data = JsonConvert.DeserializeObject(arg2) as JObject;
                var api_object = data["api_data"];
                string api = arg1.RequestUrl.Substring(KancolleCommon.DMMUrls.KanColleAPIUrl.Length);
                
                switch (api)
                {
                    case "api_start2":
                        var start_data = api_object.ToObject<api_start_data>();
                        UpdateShipDictionary(start_data.api_mst_ship);
                        UpdateMissionDictionary(start_data.api_mst_mission);
                        UpdateMapInfoDictionary(start_data.api_mst_mapinfo);
                        UpdateSlotDictionary(start_data.api_mst_slotitem);
                        foreach(var plugin in pluginManager.Plugins)
                        {
                            plugin.OnGameStart(generalViewModel, gameData.Clone());
                        }
                        break;
                    case "api_port/port":
                        var port_data = api_object.ToObject<api_port_data>();
                        UpdateOwnedShipDictionary(port_data.api_ship);
                        UpdateMaterial(port_data.api_material);
                        UpdatePort(port_data);
                        UpdateDeck(port_data.api_deck_port);
                        break;
                    case "api_get_member/material":
                        var material_data = api_object.ToObject<api_material_item[]>();
                        UpdateMaterial(material_data);
                        break;
                    case "api_req_hokyu/charge":
                        UpdateMaterial(api_object["api_material"].ToObject<int[]>());
                        break;
                    case "api_get_member/require_info":
                        var getmember_data = api_object.ToObject<api_requireinfo_data>();
                        UpdateOwnedSlotDictionary(getmember_data.api_slot_item);
                        generalViewModel.ItemCount = getmember_data.api_slot_item.Length;
                        break;
                    case "api_req_kousyou/createitem"://开发装备
                        var createitem_data = api_object.ToObject<api_createitem_data>();
                        if (createitem_data.api_create_flag == 1)
                            generalViewModel.ItemCount++;
                        break;
                    case "api_req_kousyou/destroyitem2":
                        KancolleCommon.destroyitem2_api.svdata sv_data_destroyitem2 = JsonConvert.DeserializeObject<KancolleCommon.destroyitem2_api.svdata>(arg2);
                        if (sv_data_destroyitem2 != null)
                            generalViewModel.ItemCount--;
                        break;
                    case "api_get_member/questlist":
                        var questlist_data = api_object.ToObject<api_questlist_data>();
                        if (arg1.Data["api_tab_id"] == "0")
                        {
                            UpdateQuest(questlist_data);
                        }
                        UpdateQuestDictionary(questlist_data);
                        break;
                    case "api_req_quest/start":
                        StartQuest(gameData.QuestDic[int.Parse(arg1.Data["api_quest_id"])]);
                        break;
                    case "api_req_quest/stop":
                    case "api_req_quest/clearitemget":
                        StopQuest(gameData.QuestDic[int.Parse(arg1.Data["api_quest_id"])]);
                        break;
                    case "api_req_hensei/change"://舰队编成修改
                        ChangeShip(arg1.Data);
                        break;
                    case "api_get_member/mission"://可进行的任务
                        var ownedMissionItems = api_object.ToObject<api_mission_item[]>();
                        UpdateOwnedMissionDictionary(ownedMissionItems);
                        break;
                    case "api_req_map/start"://出击
                    case "api_req_map/next":
                        var nextdata = api_object.ToObject<api_mapnext_data>();
                        break;
                    case "api_req_sortie/battle"://战斗
                        var battledata = api_object.ToObject<api_battle_data>();

                        break;
                    case "api_req_sortie/battleresult"://战斗结果
                        var result = api_object.ToObject<api_battleresult_data>();
                        break;
                    case "api_req_battle_midnight/battle"://夜战
                        var nightbattledata = api_object.ToObject<api_battle_data>();
                        break;
                    case "api_get_member/ship_deck":
                        UpdateShipDeck(api_object.ToObject<api_shipdeck_data>());
                        break;
                }
                
                battleManager.OnGameDataUpdatedHandler(gameData.Clone());
                battleManager.OnAPIResponseReceivedHandler(arg1, arg2, api);
                taskExecutor.OnAPIResponseReceivedHandler(arg1, arg2, api);
            }
            catch(Exception ex)
            {
                MessageBox.Show("API处理错误\n" + ex.Message);
                LogFactory.SystemLogger.Error($"API'{arg1.RequestUrl}'处理错误![{arg2}]", ex);
            }
            
        }

        private void WebView_StatusMessage(object sender, CefSharp.StatusMessageEventArgs e)
        {
           
           // MessageBox.Show("status\n"+e.Value);
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

        private void UpdateShipDictionary(api_mst_ship_item[] shipItems)
        {
            gameData.ShipDic = shipItems.ToDictionary(k=>k.api_id, k=>k);
        }

        private void UpdateSlotDictionary(api_mst_slotitem_item[] slotItems)
        {
            gameData.SlotDic = slotItems.ToDictionary(k => k.api_id, k => k);
        }

        private void UpdateOwnedShipDictionary(api_ship_item[] shipItems)
        {
            gameData.OwnedShipDic = shipItems.ToDictionary(k => k.api_id, k => k);
            foreach (var plugin in pluginManager.Plugins)
            {
                plugin.OnShipUpdated(generalViewModel, gameData.Clone());
            }
        }

        private void UpdateMissionDictionary(api_mst_mission_item[] missionItems)
        {
            gameData.MissionDic = missionItems.ToDictionary(k => k.api_id, k => k);
        }

        private void UpdateMapInfoDictionary(api_mst_mapinfo_item[] mapinfoItems)
        {
            gameData.MapDic = mapinfoItems.ToDictionary(k => k.api_id, k => k);
        }

        private void UpdateOwnedMissionDictionary(api_mission_item[] ownedMissionItems)
        {
            gameData.OwnedMissionDic = ownedMissionItems.ToDictionary(k => k.api_mission_id, k => k);
        }

        /// <summary>
        /// 调用port的API时会更新deck
        /// </summary>
        /// <param name="deckPorts"></param>
        private void UpdateDeck(KancolleCommon.api_deck_port_item[] deckPorts)
        {
            gameData.OwnedShipPlaceDic.Clear();
            for(int i=0; i<deckPorts.Length; i++)
            {
                for(int j=0; j<deckPorts[i].api_ship.Length; j++)
                {
                    int ownedShipId = deckPorts[i].api_ship[j];//在第i个舰队第j个位置的舰娘id
                    generalViewModel.Decks[i].Name = (i+1) + "";
                    if(ownedShipId > 0)
                    {
                        var ship = gameData.ShipDic[gameData.OwnedShipDic[ownedShipId].api_ship_id];//通过自己的舰娘id查找库中的舰娘
                        generalViewModel.Decks[i].Ships[j].Name = ship.api_name;
                        gameData.OwnedShipPlaceDic.Add(ownedShipId, new KeyValuePair<int, int>(i, j));
                    }
                    else
                        generalViewModel.Decks[i].Ships[j].Reset();
                    gameData.OwnedShipPlaceArray[i, j] = ownedShipId;
                    
                }
            }

            foreach(var plugin in pluginManager.Plugins)
            {
                plugin.OnDeckUpdated(generalViewModel, gameData.Clone());
            }
        }


        private void UpdateOwnedSlotDictionary(api_slot_item_item[] slotItems)
        {
            gameData.OwnedSlotDic = slotItems.ToDictionary(k=>k.api_id, k=>k);
        }

        private void UpdateShipDeck(api_shipdeck_data shipdeckData)
        {
            foreach(var ship in shipdeckData.api_ship_data)
            {
                if (gameData.OwnedShipDic.ContainsKey(ship.api_id))
                {
                    gameData.OwnedShipDic[ship.api_id] = ship;
                }
                else
                {
                    //事实上应该不可能
                    gameData.OwnedShipDic.Add(ship.api_id, ship);
                }
            }
        }

        /// <summary>
        /// 编成更改时调用
        /// </summary>
        /// <param name="postData"></param>
        private void ChangeShip(Dictionary<string, string> postData)
        {
            int api_id = int.Parse(postData["api_id"]);
            int deck_index = api_id - 1;
            int api_ship_id = int.Parse(postData["api_ship_id"]);
            int api_ship_idx = int.Parse(postData["api_ship_idx"]);
            if (api_ship_id > 0)//添加
            {
                var ship = gameData.ShipDic[gameData.OwnedShipDic[api_ship_id].api_ship_id];
                
                //交换自己的位置和目标位置
                if (gameData.OwnedShipPlaceDic.ContainsKey(api_ship_id))//本来就在编队中
                {
                    var place = gameData.OwnedShipPlaceDic[api_ship_id];//本来的位置
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDic[api_ship_id] = new KeyValuePair<int, int>(deck_index, api_ship_idx);
                        generalViewModel.Decks[deck_index].Ships[api_ship_idx].Name = ship.api_name;
                        //原位置清空      
                        int i = 0;
                        for (i = place.Value; i < generalViewModel.Decks[place.Key].Ships.Length - 1; i++)
                        {
                            generalViewModel.Decks[place.Key].Ships[i].Name = generalViewModel.Decks[place.Key].Ships[i + 1].Name;
                            int nextShipId = gameData.OwnedShipPlaceArray[place.Key, i + 1];
                            if (nextShipId > 0)
                                gameData.OwnedShipPlaceDic[nextShipId] = new KeyValuePair<int, int>(place.Key, i);
                            gameData.OwnedShipPlaceArray[place.Key, i] = nextShipId;
                        }
                        generalViewModel.Decks[place.Key].Ships[i].Reset();
                        gameData.OwnedShipPlaceArray[place.Key, i] = -1;
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceArray[place.Key, place.Value] = dstShip;
                        gameData.OwnedShipPlaceDic[api_ship_id] = new KeyValuePair<int, int>(deck_index, api_ship_idx);
                        gameData.OwnedShipPlaceDic[dstShip] = new KeyValuePair<int, int>(place.Key, place.Value);
                        string temp = generalViewModel.Decks[place.Key].Ships[place.Value].Name;
                        generalViewModel.Decks[place.Key].Ships[place.Value].Name = generalViewModel.Decks[deck_index].Ships[api_ship_idx].Name;
                        generalViewModel.Decks[deck_index].Ships[api_ship_idx].Name = temp;
                    }
                }
                else//本来不在编队中
                {
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDic.Add(api_ship_id, new KeyValuePair<int, int>(deck_index, api_ship_idx));
                        generalViewModel.Decks[0].Ships[api_ship_idx].Name = ship.api_name;
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDic.Add(api_ship_id, new KeyValuePair<int, int>(deck_index, api_ship_idx));
                        gameData.OwnedShipPlaceDic.Remove(dstShip);
                        generalViewModel.Decks[deck_index].Ships[api_ship_idx].Name = ship.api_name;

                    }
                }
            }
            else if (api_ship_id == -1)//移除
            {
                int i;
                gameData.OwnedShipPlaceDic.Remove(gameData.OwnedShipPlaceArray[deck_index, api_ship_idx]);
                for (i = api_ship_idx; i < generalViewModel.Decks[deck_index].Ships.Length - 1; i++)
                {//后面的向前补充
                    generalViewModel.Decks[deck_index].Ships[i].Name = generalViewModel.Decks[deck_index].Ships[i + 1].Name;
                    int nextShipId = gameData.OwnedShipPlaceArray[deck_index, i + 1];
                    if (nextShipId > 0)
                        gameData.OwnedShipPlaceDic[nextShipId] = new KeyValuePair<int, int>(deck_index, i);
                    gameData.OwnedShipPlaceArray[deck_index, i] = nextShipId;
                }
                generalViewModel.Decks[deck_index].Ships[i].Reset();
                gameData.OwnedShipPlaceArray[deck_index, i] = -1;

            }
            else if(api_ship_id == -2)//移除旗舰外所有舰船
            {
                for(int i=1; i<gameData.OwnedShipPlaceArray.GetLength(1); i++)
                {
                    gameData.OwnedShipPlaceDic.Remove(gameData.OwnedShipPlaceArray[deck_index, i]);
                    gameData.OwnedShipPlaceArray[deck_index, i] = -1;
                    generalViewModel.Decks[deck_index].Ships[i].Reset();
                }
            }

            foreach (var plugin in pluginManager.Plugins)
            {
                plugin.OnDeckUpdated(generalViewModel, gameData.Clone());
            }
        }

        private void UpdateMaterial(int[] materials)
        {
            if(materials != null)
            {
                generalViewModel.Ran = materials[0];
                generalViewModel.Dan = materials[1];
                generalViewModel.Gang = materials[2];
                generalViewModel.Lv= materials[3];
            }
        }


        /// <summary>
        /// 更新资源信息
        /// </summary>
        /// <param name="materials"></param>
        private void UpdateMaterial(api_material_item[] materials = null)
        {
            if(materials != null)
            {
                foreach(var material in materials)
                {
                    switch(material.api_id)
                    {
                        case 1:
                            generalViewModel.Ran = material.api_value;
                            break;
                        case 2:
                            generalViewModel.Dan = material.api_value;
                            break;
                        case 3:
                            generalViewModel.Gang = material.api_value;
                            break;
                        case 4:
                            generalViewModel.Lv = material.api_value;
                            break;
                        case 5:
                            generalViewModel.Jianzao = material.api_value;
                            break;
                        case 6:
                            generalViewModel.Xiufu = material.api_value;
                            break;
                        case 7:
                            generalViewModel.Kaifa = material.api_value;
                            break;
                        case 8:
                            generalViewModel.Gaixiu = material.api_value;
                            break;
                    }
                   
                }
                
            }
            foreach (var plugin in pluginManager.Plugins)
            {
                plugin.OnMaterialUpdated(generalViewModel, gameData.Clone());
            }
        }

        /// <summary>
        /// 更新提督基本信息
        /// </summary>
        /// <param name="portdata"></param>
        private void UpdatePort(api_port_data portdata)
        {
            if(portdata != null)
            {
                generalViewModel.Level = "Lv." + portdata.api_basic.api_level;
                generalViewModel.Rank = KancolleAPIs.RankText[portdata.api_basic.api_rank];
                generalViewModel.NickName = portdata.api_basic.api_nickname;
                generalViewModel.ShipCount = portdata.api_ship.Length;
                generalViewModel.MaxShipCount = portdata.api_basic.api_max_chara;
                generalViewModel.MaxItemCount = portdata.api_basic.api_max_slotitem;
            }
        }

        /// <summary>
        /// 更新任务字典，用于任务信息的查询
        /// </summary>
        /// <param name="questData"></param>
        private void UpdateQuestDictionary(api_questlist_data questData)
        {
            if(questData != null && questData.api_list !=null)
            {
                List<api_questlist_item> validQuestList = new List<api_questlist_item>();

                for (int i = 0; i < questData.api_list.Length; i++)
                {
                    JObject jo = questData.api_list[i] as JObject;
                    if (jo != null)
                    {
                        validQuestList.Add(jo.ToObject<api_questlist_item>());
                    }

                }
                foreach(var quest in validQuestList)
                {
                    if (gameData.QuestDic.ContainsKey(quest.api_no))
                        gameData.QuestDic[quest.api_no] = quest;
                    else
                        gameData.QuestDic.Add(quest.api_no, quest);
                }
            }
            foreach (var plugin in pluginManager.Plugins)
            {
                plugin.OnQuestUpdated(generalViewModel, gameData.Clone());
            }
        }

        /// <summary>
        /// 获取到任务列表后更新任务列表
        /// </summary>
        /// <param name="questData"></param>
        private void UpdateQuest(KancolleCommon.api_questlist_data questData)
        {
            if (questData != null && questData.api_list.Length > 0)
            {
                List<KancolleCommon.api_questlist_item> validQuestList = new List<KancolleCommon.api_questlist_item>();
                
                for(int i=0; i< questData.api_list.Length; i++)
                {
                    JObject jo = questData.api_list[i] as JObject;
                    if(jo != null)
                    {
                        validQuestList.Add(jo.ToObject<KancolleCommon.api_questlist_item>());
                    }
                   
                }
                if (validQuestList.Count == 0)
                    return;

                int minId = validQuestList[0].api_no;
                int maxId = validQuestList[validQuestList.Count - 1].api_no;
                if (questData.api_disp_page == 1)
                {
                    foreach (var quest in generalViewModel.QuestList)
                    {
                        if (quest.Id <= maxId)
                            quest.Reset();
                    }
                }
                else if (questData.api_disp_page == questData.api_page_count)
                {
                    foreach (var quest in generalViewModel.QuestList)
                    {
                        if (quest.Id >= minId)
                            quest.Reset();
                    }
                }
                else
                {
                    foreach (var quest in generalViewModel.QuestList)
                    {
                        if (quest.Id >= minId && quest.Id <= maxId)
                            quest.Reset();
                    }
                }

                generalViewModel.QuestList = (from q in generalViewModel.QuestList
                                              orderby q.Id ascending
                                              select q).ToArray();
                int j = generalViewModel.QuestList.Length - 1;
                foreach (var quest in validQuestList)
                {
                    if (quest.api_state > 1)
                    {
                        generalViewModel.QuestList[j].Id = quest.api_no;
                        generalViewModel.QuestList[j].Name = quest.api_title;
                        generalViewModel.QuestList[j].Detail = quest.api_detail;
                        if (quest.api_state == 3)
                            generalViewModel.QuestList[j].Progress = "完成";
                        else
                        {
                            if (quest.api_progress_flag == 0)
                                generalViewModel.QuestList[j].Progress = "进行中";
                            else
                                generalViewModel.QuestList[j].Progress = (30 * quest.api_progress_flag + 20) + "%";
                        }
                        j--;
                    }
                    if (j < 0)
                        break;
                }
                generalViewModel.QuestList = (from q in generalViewModel.QuestList
                                              orderby q.Id ascending
                                              select q).ToArray();

            }

            foreach(var plugin in pluginManager.Plugins)
            {
                plugin.OnQuestUpdated(generalViewModel, gameData.Clone());
            }
        }

        /// <summary>
        /// 开始某个任务后更新任务列表
        /// </summary>
        /// <param name="quest"></param>
        private void StartQuest(api_questlist_item quest)
        {
            generalViewModel.QuestList = (from q in generalViewModel.QuestList
                                          orderby q.Id ascending
                                          select q).ToArray();
            int j = generalViewModel.QuestList.Length - 1;
            generalViewModel.QuestList[j].Id = quest.api_no;
            generalViewModel.QuestList[j].Name = quest.api_title;
            generalViewModel.QuestList[j].Detail = quest.api_detail;
            if (quest.api_state == 3)
                generalViewModel.QuestList[j].Progress = "完成";
            else
            {
                if (quest.api_progress_flag == 0)
                    generalViewModel.QuestList[j].Progress = "进行中";
                else
                    generalViewModel.QuestList[j].Progress = (30 * quest.api_progress_flag + 20) + "%";
            }
            generalViewModel.QuestList = (from q in generalViewModel.QuestList
                                          orderby q.Id ascending
                                          select q).ToArray();
        }

        /// <summary>
        /// 停止某个任务后更新任务列表
        /// </summary>
        /// <param name="quest"></param>
        private void StopQuest(api_questlist_item quest)
        {
            foreach (var q in generalViewModel.QuestList)
            {
                if (q.Id == quest.api_no)
                {
                    q.Reset();
                    break;
                }
            }
            generalViewModel.QuestList = (from q in generalViewModel.QuestList
                                          orderby q.Id ascending
                                          select q).ToArray();
            foreach (var plugin in pluginManager.Plugins)
            {
                plugin.OnQuestUpdated(generalViewModel, gameData.Clone());
            }
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
            Img_Filter.Source = screen;
            
            var sceneType = sceneRecognizer.GetSceneTypeFromBitmap(ToBitmap(screen));
            Txt_CurrentScene.Content = sceneType.ToString();
        }

        private Scene GetCurrentScene()
        {
            var screen = GetWebViewBitmap();
            var scene = sceneRecognizer.GetSceneTypeFromBitmap(ToBitmap(screen));
            return scene;
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
            QuestEditor questEditor = new QuestEditor(gameData);
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
            var currentScene = GetCurrentScene();
            SceneTypes toSceneType = (SceneTypes)Cbx_NavigateTarget.SelectedItem;
            var edges = navigator.Navigate(currentScene.SceneType, toSceneType);
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
            Cbx_NavigateTarget.ItemsSource = Enum.GetValues(typeof(SceneTypes));
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
            taskExecutor?.Stop();
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
                if (lb.SelectedIndex > 0)
                {
                    if (GetCurrentScene() != SceneTypes.Port)
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
                        aiButton.Content = ai.Name;
                    }
                    
                }
                else//切换至手动
                {
                    currentAI.Stop();
                    aiGrid.Visibility = Visibility.Collapsed;
                    Grid_ManualAI.Visibility = Visibility.Visible;
                    Grid_ManualAI.IsEnabled = true;
                    currentAI = null;
                    aiButton.Content = aiNames[0];
                }

                Tc_Tool.SelectedItem = Tc_Tool.FindName("Ti_AIs");
                
            }

            lb.UnselectAll();

            aiButton.IsOpen = false;
        }

        private void Grid_AIPanel_Loaded(object sender, RoutedEventArgs e)
        {
            aiGrid = sender as Grid;
        }
    }
}
