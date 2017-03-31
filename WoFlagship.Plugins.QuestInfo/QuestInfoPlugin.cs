using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartFormat;
using SmartFormat.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleQuestData;
using WoFlagship.KancollePlugin.QuestInfo.Properties;
using WoFlagship.ViewModels;

namespace WoFlagship.KancollePlugin.QuestInfo
{
    public class QuestInfoPlugin : IKancollePlugin
    {
        //public const string QuestInfoFile = "Infos\\questinfo.json";
        private QuestInfoPanel panel = new QuestInfoPanel();

        public int Version { get { return 1; } }
        public string Name { get { return "任务信息插件"; } }
        public string Description { get { return "任务辅助"; } }
        public UserControl PluginPanel { get { return panel; } }

        /// <summary>
        /// 任务额外信息字典,其信息不是游戏本身中的,而是由第三方整理、翻译等得到的
        /// </summary>
        private Dictionary<int, KancolleQuestInfoItem> QuestInfoDictionary = new Dictionary<int, KancolleQuestInfoItem>();


        private SmartFormatter smart;
        private KancolleQuestFormatter formatter;

        public bool NewWindow
        {
            get
            {
                return false;
            }
        }

        public void OnDeckUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {

        }

        public void OnInit(GeneralViewModel generalViewModel)
        {
            smart = Smart.CreateDefaultSmartFormat();
            formatter = new KancolleQuestFormatter();
            smart.AddExtensions(formatter);
           
        }




        public void OnGameStart(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            UpdateQuestInfo();
        }

        public void OnGameDataUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            formatter.GameData = gameData;
            foreach (var quest in generalViewModel.QuestList)
            {
                KancolleQuestData.KancolleQuestInfoItem questInfo;
                if (QuestInfoDictionary.TryGetValue(quest.Id, out questInfo))
                {
                    if (questInfo.Requirements is AGouQuestRequirement)
                    {
                        quest.Detail = Resources.agou_format;
                    }
                    else if (questInfo.Requirements is SortieQuestRequirement)
                    {
                        quest.Detail = getSortieString(questInfo.Requirements as SortieQuestRequirement);
                    }
                    else
                    {
                        quest.Detail = questInfo.Detail;
                    }
                }
            }
        }

        private string getSortieString(SortieQuestRequirement require)
        {
            string str = smart.Format(Resources.sortie_format, require);

            return str;
        }

        /// <summary>
        /// 从questinfo文件中更新任务信息
        /// </summary>
        private void UpdateQuestInfo()
        {
            try
            {

                string content = Encoding.UTF8.GetString(Resources.questinfo);
                var questInfoObject = JsonConvert.DeserializeObject(content) as JToken;
                int version = questInfoObject["Version"].ToObject<int>();
                string updateTime = questInfoObject["UpdateTime"].ToString();
                QuestInfoDictionary.Clear();
                QuestInfoDictionary = new Dictionary<int, KancolleQuestInfoItem>();
                foreach (var quest in questInfoObject["QuestInfos"])
                {
                    KancolleQuestInfoItem qi = new KancolleQuestInfoItem()
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
                                // LogFactory.SystemLogger.Warn($"未知任务类型[{qi.Category}]!");
                            }
                            unknownCat = true;
                            break;
                    }
                    if (!unknownCat)
                    {
                        qi.Requirements = re;
                        QuestInfoDictionary.Add(qi.GameId, qi);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"任务信息资源初始化失败！\n{ex.Message}");
                // LogFactory.SystemLogger.Error("任务信息资源初始化失败！", ex);
            }


        }
    }

    class KancolleQuestFormatter : IFormatter
    {
        private string[] names = new[] { "Ship", "Result", "Group", "Select", "Amount" };
        public string[] Names { get { return names; } set { this.names = value; } }

        public KancolleGameData GameData { get; set; }

        public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            if (formattingInfo.Placeholder.FormatterName == "Ship")
            {
                if (formattingInfo.CurrentValue is int[])
                {
                    var ships = formattingInfo.CurrentValue as int[];
                    if (ships.Length > 0)
                    {
                        formattingInfo.Write(getShip(ships[0]));
                        for (int i = 1; i < ships.Length; i++)
                        {
                            formattingInfo.Write("," + getShip(ships[i]));
                        }
                    }
                    else
                    {
                        formattingInfo.Write(Resources.ship_any);
                    }
                }
                else if (formattingInfo.CurrentValue is int)
                {
                    var ships = (int)formattingInfo.CurrentValue;
                    formattingInfo.Write(getShip(ships));

                }
                else if (formattingInfo.CurrentValue is int?)
                {
                    var ships = formattingInfo.CurrentValue as int?;
                    formattingInfo.Write(getShip(ships));

                }
            }
            if (formattingInfo.Placeholder.FormatterName == "Result")
            {
                if (formattingInfo.CurrentValue is string)
                {
                    if (formattingInfo.CurrentValue.ToString() == "クリア")
                    {
                        formattingInfo.Write(Resources.result_clear);
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "S")
                    {
                        formattingInfo.Write(Resources.result_s);
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "A")
                    {
                        formattingInfo.Write(Resources.result_a);
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "B")
                    {
                        formattingInfo.Write(Resources.result_b);
                    }
                    else if (formattingInfo.CurrentValue.ToString() == "C")
                    {
                        formattingInfo.Write(Resources.result_c);
                    }
                    else
                    {
                        Console.WriteLine("未知Result！" + formattingInfo.CurrentValue);
                        return false;
                    }

                    return true;
                }
            }
            else if (formattingInfo.Placeholder.FormatterName == "Group")
            {
                if (formattingInfo.CurrentValue is ShipRequirement[])
                {
                    var requires = formattingInfo.CurrentValue as ShipRequirement[];
                    if (requires != null)
                    {
                        foreach (var req in requires)
                        {  
                            string str = formattingInfo.FormatDetails.Formatter.Format(Resources.group_format, req);
                            formattingInfo.Write(str);

                        }
                    }
                    else
                    {
                        formattingInfo.Write(Resources.ship_any);
                    }
                    return true;
                }
            }
            else if (formattingInfo.Placeholder.FormatterName == "Amount")
            {
                if (formattingInfo.CurrentValue is int[])
                {
                    int[] amount = formattingInfo.CurrentValue as int[];
                    if (amount.Length == 1)
                    {
                        formattingInfo.Write(amount[0] + "只");
                    }
                    else
                    {
                        formattingInfo.Write(amount[0] + "-" + amount[1] + "只");
                    }
                    return true;
                }
            }
            else if (formattingInfo.Placeholder.FormatterName == "Select")
            {
                if (formattingInfo.CurrentValue is int[])
                {

                    int[] select = formattingInfo.CurrentValue as int[];
                    if (select.Length == 1)
                    {
                        formattingInfo.Write("任意" + select[0] + "只");
                    }
                    else
                    {
                        formattingInfo.Write("任意" + select[0] + "-" + select[1] + "只");
                    }
                    return true;
                }
            }


            return false;
        }

        private string getShip(int? id)
        {
            if(id == null)
                return Resources.ship_any;
            if (id > 0)
            {
                KancolleShipData item = null;
                if(GameData?.ShipDataDictionary.TryGetValue((int)id, out item) == true)
                {
                    return item.Name;
                }
                else
                {
                    return Resources.ship_unknown;
                }
            }
            else if(id == 0)
            {
                return Resources.ship_any;
            }
            else
            {
                id = Math.Abs((int)id);
                if(id< KancolleAPIs.ShipTypeText.Length)
                {
                    return KancolleAPIs.ShipTypeText[(int)id];
                }
                else
                {
                    return Resources.ship_unknown;
                }
            }
        }
    }

}
