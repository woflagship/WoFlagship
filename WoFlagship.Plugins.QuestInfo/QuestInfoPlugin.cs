using Newtonsoft.Json;
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
using WoFlagship.Plugins.QuestInfo.Properties;
using WoFlagship.ViewModels;

namespace WoFlagship.Plugins.QuestInfo
{
    public class QuestInfoPlugin : IPlugin
    {
        private QuestInfoPanel panel = new QuestInfoPanel();

        public int Version { get { return 1; } }
        public string Name { get { return "任务信息插件"; } }
        public string Description { get { return "任务辅助"; } }
        public UserControl PluginPanel { get { return panel; } }

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

        }

        public void OnGameDataUpdated(GeneralViewModel generalViewModel, KancolleGameData gameData)
        {
            formatter.GameData = gameData;
            foreach (var quest in generalViewModel.QuestList)
            {
                KancolleQuestData.KancolleQuestInfoItem questInfo;
                if (gameData.QuestInfoDictionary.TryGetValue(quest.Id, out questInfo))
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
