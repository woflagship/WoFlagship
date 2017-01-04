using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using WoFlagship.KancolleCore;
using WoFlagship.KancolleQuestData;
using WoFlagship.Wiki;

namespace WoFlagship.ToolWindows
{
    /// <summary>
    /// QuestEditor.xaml 的交互逻辑
    /// </summary>
    public partial class QuestEditor : Window
    {
        private ObservableCollection<KancolleQuestInfoItem> questList = new ObservableCollection<KancolleQuestInfoItem>();

        private KancolleGameData gameData;
        Dictionary<string, KancolleShipData> nameDic;

        public QuestEditor(KancolleGameData gameData)
        {
            this.gameData = gameData;
            InitializeComponent();
           
        }

        private void Btn_LoadFromWikiFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt";
            if(ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                FileInfo fi = new FileInfo(ofd.FileName);
                string content = "";
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    content = sr.ReadToEnd();
                }
                var tableList = WikiTable.Parse(content);
                UpdateQuestList(tableList);
                
            }
        }

        private void Btn_LoadFromJsonFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Json文件(*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                questList.Clear();
                

            }
        }

        private void UpdateQuestList(List<WikiTable> tableList)
        {
            questList.Clear();
            foreach (var table in tableList)
            {
                foreach (var row in table.Rows)
                {
                    if (row.Count >= 8)
                    {
                        KancolleQuestInfoItem questInfo = new KancolleQuestInfoItem();
                        questInfo.Id = row[0].Substring(row[0].IndexOf('|') + 1).Trim();
                        questInfo.Name = row[1].Substring(row[1].LastIndexOf('|') + 1).Replace("}}", "").Trim();
                        questInfo.Detail = row[2].Trim().Replace("<br/>","\n");
                        questInfo.Ran = int.Parse(row[3]);
                        questInfo.Dan = int.Parse(row[4]);
                        questInfo.Gang = int.Parse(row[5]);
                        questInfo.Lu = int.Parse(row[6]);
                        questInfo.Other = row[7].Trim().Replace("<br/>", "\n");
                        if (row.Count == 9)
                            questInfo.Note = row[8].Replace("<br/>", "\n");
                        questList.Add(questInfo);
                    }

                }
            }
        }

        private void SaveQuestListToJson(string jsonFile)
        {
            using (StreamWriter sw = File.CreateText(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                KancolleQuestInfoMetadata qi = new KancolleQuestInfoMetadata()
                {
                    QuestInfos = questList.ToArray(),
                };
                serializer.Serialize(sw, qi);
            }
        }

        private void Dg_QuestList_Loaded(object sender, RoutedEventArgs e)
        {
            Dg_QuestList.ItemsSource = questList;
        }

        private void Btn_SaveToJson_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json文件(*.json)|*.json";
            if(sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                SaveQuestListToJson(fileName);
            }
        }


        //wiki id转到game id
        private void Btn_MergeFromPOI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Json文件(*.json)|*.json";
           nameDic  = new Dictionary<string, KancolleShipData>();
            
            foreach(var item in gameData.ShipDataDictionary)
            {
                if (!nameDic.ContainsKey(item.Value.Name))
                    nameDic.Add(item.Value.Name, item.Value);
            }
            if(ofd.ShowDialog() == true)
            {
                using (StreamReader sr = File.OpenText(ofd.FileName))
                {
                    string content = sr.ReadToEnd();
                    var questItems = JsonConvert.DeserializeObject(content) as JArray;
                    
                    var questDic = questItems.ToDictionary(k => {
                        string id = k["wiki_id"].ToObject<string>();
                        string cat = "";
                        string digit = "";
                        foreach(var ch in id)
                        {
                            if (char.IsLetter(ch))
                                cat += ch;
                            else
                                digit += ch;
                        }
                        return cat + int.Parse(digit).ToString();
                    }, k => k);
                    foreach(var quest in questList)
                    {
                        JToken item;
                        if(questDic.TryGetValue(quest.Id, out item))
                        {
                            quest.GameId = item["game_id"].ToObject<int>();
                            quest.Prerequisite = item["prerequisite"].ToObject<int[]>();
                            quest.Category = item["requirements"]["category"].ToObject<string>();

                            

                            try
                            {
                                JToken requireToken = item["requirements"];
                                quest.Requirements = getQuestRequirement(requireToken);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                        }
                    }
                }
            } 
        }

        private IQuestRequirement getQuestRequirement(JToken requireToken)
        {
            if (requireToken != null)
            {
                string Category = requireToken["category"].ToObject<string>();

                if (Category == "sortie")
                {
                    SortieQuestRequirement require = new SortieQuestRequirement()
                    {
                        Maps = parseMapFromString(requireToken["map"]?.ToString()),
                        MapStr = requireToken["map"]?.ToString(),
                        Boss = requireToken["boss"]==null?false: requireToken["boss"].ToObject<bool>(),
                        Result = requireToken["result"]?.ToString(),
                        Times = requireToken["times"]==null?1: requireToken["times"].ToObject<int>(),
                        Groups = parseFromGroups(requireToken["groups"] as JArray)?.ToArray(),
                        FleetId = requireToken["fleetid"]==null?0: requireToken["fleetid"].ToObject<int>(),
                        Disallowed = requireToken["disallowed"]==null? null :(int?) -KancolleAPIs.ShipTypeDic[requireToken["disallowed"].ToString()]
                    };
                   return require;
                }
                else if (Category == "fleet")
                {
                    FleetQuestRequirement require = new FleetQuestRequirement()
                    {
                        Groups = parseFromGroups(requireToken["groups"] as JArray)?.ToArray(),
                        FleetId = requireToken["fleetid"]?.ToObject<int?>(),
                        Disallowed = requireToken["disallowed"] == null ? null : (int?)-KancolleAPIs.ShipTypeDic[requireToken["disallowed"].ToString()]
                    };
                    return require;
                }
                else if (Category == "exercise")//演习
                {
                    ExerciseQuestRequirement require = new ExerciseQuestRequirement()
                    {
                        Times = requireToken["times"]?.ToObject<int?>(),
                        Daily = requireToken["daily"]?.ToObject<bool?>(),
                        Victory = requireToken["victory"]?.ToObject<bool?>()
                    };
                    return require;
                }
                else if (Category == "expedition")
                {
                    ExpeditionQuestRequirement require = new ExpeditionQuestRequirement()
                    {
                        Resources = requireToken["resources"]?.ToObject<int[]>(),

                        Objects = requireToken["objects"] == null ? null : parseObjects(requireToken["objects"] as JArray)
                    };
                    return require;
                }
                else if (Category == "equipexchange")
                {
                    EquipexchangeQuestRequirement require = new EquipexchangeQuestRequirement()
                    {
                        Equipments = parseEquip(requireToken["equipments"]),
                        Scraps = parseEquip(requireToken["scraps"]),
                        Consumptions = parseEquip(requireToken["consumptions"]),
                        Resources = requireToken["resources"]?.ToObject<int[]>()
                    };
                    return require;
                }
                else if(Category == "modernization")
                {
                    ModernizationQuestRequirement require = new ModernizationQuestRequirement()
                    {
                        Times = requireToken["times"]?.ToObject<int?>(),
                        ShipType = -KancolleAPIs.ShipTypeDic[requireToken["ship"].ToString()],
                      
                        Resources = requireToken["resources"]?.ToObject<int[]>()
                    };
                    if(requireToken["consumptions"] != null)
                    {
                        List<ModernizationQuestItem> items = new List<ModernizationQuestItem>();
                        foreach(var item in requireToken["consumptions"])
                        {
                            ModernizationQuestItem mi = new ModernizationQuestItem()
                            {
                                Amount = item["amount"].ToObject<int>()
                            };
                            var shipStr = item["ship"].ToString();
                            if (nameDic.ContainsKey(shipStr))
                                mi.ShipId = nameDic[shipStr].ShipId;
                            else if (KancolleAPIs.ShipTypeDic.ContainsKey(shipStr))
                                mi.ShipId = -KancolleAPIs.ShipTypeDic[shipStr];
                            else
                                MessageBox.Show("Unknown ship\n" + shipStr);
                            items.Add(mi);
                        }
                        require.Consumptions = items.ToArray();
                    }
                    return require;

                }
                else if (Category == "sink")
                {
                    SinkQuestRequirement require = new SinkQuestRequirement();
                    require.Amount = requireToken["amount"].ToObject<int>();
                    switch (requireToken["ship"].ToString())
                    {
                        case "敵補給艦":
                            require.ShipType = EnemySinkType.Supply;
                            break;
                        case "敵空母":
                            require.ShipType = EnemySinkType.CV;
                            break;
                        case "敵潜水艦":
                            require.ShipType = EnemySinkType.SS;
                            break;
                        default:
                            MessageBox.Show("Unkown enemy ship\n" + requireToken["ship"].ToString());
                            require.ShipType = EnemySinkType.Unknown;
                            break;

                    }

                }
                else if(Category == "and")
                {
                    AndQuestRequirement require = new AndQuestRequirement();
                    if (requireToken["list"] == null)
                        return require;
                    List<IQuestRequirement> requireList = new List<IQuestRequirement>();
                    foreach (var listItem in requireToken["list"])
                    {
                        var r = getQuestRequirement(listItem);
                        if (r != null)
                            requireList.Add(r);
                    }
                    require.List = requireList.ToArray();

                }
                else if(Category == "simple")
                {
                    SimpleQuestRequirement require = new SimpleQuestRequirement()
                    {
                        Subcategory = requireToken["subcategory"].ToString(),
                        Times = requireToken["times"].ToObject<int>(),
                        Batch = requireToken["batch"]?.ToObject<bool?>()
                    };
                    return require;
                }
                else if(Category=="a-gou")
                {
                    AGouQuestRequirement require = new AGouQuestRequirement();
                    return require;
                }
                else if(Category == "modelconversion")
                {
                    ModelconversionQuestRequirement require = new ModelconversionQuestRequirement()
                    {
                        Equipment = parseEquip(requireToken["equipment"]),
                        Scraps = parseEquip(requireToken["scraps"]),
                        //Secretary = .ToObject<int[]>(),
                        Fullyskilled = requireToken["fullyskilled"]?.ToObject<bool?>(),
                        UseSkilledCrew = requireToken["use_skilled_crew"]?.ToObject<bool?>(),
                        Maxmodified = requireToken["maxmodified"]?.ToObject<bool?>(),
                        Consumptions = parseEquip(requireToken["consumptions"])
                    };
                    var secretary = requireToken["secretary"];
                    if (secretary != null)
                    {
                        if (secretary is JArray)
                        {
                            List<int> ids = new List<int>();
                            foreach (var s in secretary)
                            {
                                ids.Add(nameDic[s.ToString()].ShipId);
                            }
                            require.Secretary = ids.ToArray();
                        }
                        else
                        {
                            require.Secretary = new int[] { nameDic[secretary.ToString()].ShipId };
                        }
                    }
                    return require;
                }
                else if(Category == "scrapequipment")
                {
                    ScrapequipmentQuestRequirement require = new ScrapequipmentQuestRequirement()
                    {
                        List = parseEquip(requireToken["list"])
                    };
                    return require;
                }
            }

            return null;
        }

        private QuestRequiredItem[] parseEquip(JToken slots)
        {
            if (slots == null)
                return null;

            List<QuestRequiredItem> objs = new List<QuestRequiredItem>();

            foreach (var i in slots)
            {
                QuestRequiredItem o = new QuestRequiredItem()
                {
                    SlotName = (i is JValue) ? i.ToString() : i["name"].ToString(),
                    Amount = (i is JValue) ? 1 : i["amount"].ToObject<int>()
                };
                try
                { var slot = (from s in gameData.SlotDictionary.Values
                              where s.Name == o.SlotName
                              select s).First();
                    if (slot == null)
                        MessageBox.Show("unknown slot name\n" + o.SlotName);
                    else
                        o.SlotId = slot.Id;
                }
                catch(Exception )
                {
                    MessageBox.Show("unknown slot name\n" + o.SlotName);
                }
                objs.Add(o);
            }

            return objs.ToArray();
        }

        private ExpeditionObject[] parseObjects(JArray objects)
        {
            if (objects == null)
                return null;
            List<ExpeditionObject> objs = new List<ExpeditionObject>();
            foreach(var i in objects)
            {
                ExpeditionObject o = new ExpeditionObject()
                {
                    Times = i["times"]?.ToObject<int?>(),
                };
                if (i["id"] == null)
                    o.Id = null;
                else
                {
                    if (i["id"] is JArray)
                    {
                        o.Id = i["id"].ToObject<int[]>();
                    }
                    else
                    {
                        o.Id = new int[] { i["id"].ToObject<int>() };
                    }
                }
                objs.Add(o);
            }
            return objs.ToArray();
        }

        private int[] parseMapFromString(string mapString)
        {
            if (mapString == null)
                return null;
            string[] maps = mapString.Trim().Split(new char[] { '~'});
            List<int> ids = new List<int>();
            foreach(var map in maps)
            {
                string[] m = map.Trim().Split(new char[] { '-' });
                int hight = int.Parse(m[0]);
                int low = int.Parse(m[1]);
                ids.Add(10 * hight + low);
            }
            return ids.ToArray();
        }

        private List<ShipRequirement> parseFromGroups(JArray groups)
        {
            if (groups == null)
                return null;
            List<ShipRequirement> requirements = new List<ShipRequirement>();
            foreach (var shipToken in groups)
            {
                ShipRequirement req = new ShipRequirement();
                req.Flagship = shipToken["flagship"]==null?false: shipToken["flagship"].ToObject<bool>();

                if (shipToken["ship"] is JValue)
                {
                    if (nameDic.ContainsKey(shipToken["ship"].ToString()))
                        req.Ship = new int[] { nameDic[shipToken["ship"].ToString()].ShipId };
                    else if (KancolleAPIs.ShipTypeDic.ContainsKey(shipToken["ship"].ToString()))
                        req.Ship = new int[] { -KancolleAPIs.ShipTypeDic[shipToken["ship"].ToString()] };
                    else
                        MessageBox.Show("unkown ship\n" + shipToken["ship"]);
                   // req.Ship = new int[] { shipToken["ship"].ToObject<int>() };
                }
                else
                {
                    string[] strs = shipToken["ship"].ToObject<string[]>();
                    int[] ids = new int[strs.Length];
                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (nameDic.ContainsKey(strs[i]))
                            ids[i] = nameDic[strs[i]].ShipId;
                        else if (KancolleAPIs.ShipTypeDic.ContainsKey(strs[i]))
                            ids[i] = -KancolleAPIs.ShipTypeDic[strs[i]];
                        else
                            MessageBox.Show("unkown ship\n" + strs[i]);
                    }
                    req.Ship = ids.ToArray();
                    //req.Ship = shipToken["ship"].ToObject<int[]>();
                }

                if (req.Ship[0] > 0)
                    req.RequirementType = RequirementTypes.SingleShip;
                else
                    req.RequirementType = RequirementTypes.ShipType;

                if (shipToken["amount"] != null)
                {

                    if (shipToken["amount"] is JValue)
                    {
                        req.Amount = new int[] { shipToken["amount"].ToObject<int>() };
                    }
                    else
                    {
                        req.Amount = shipToken["amount"].ToObject<int[]>();
                    }
                }
                else if (shipToken["select"] != null)
                {
                    if (shipToken["select"] is JValue)
                    {
                        req.Select = new int[] { shipToken["select"].ToObject<int>() };
                    }
                    else
                    {
                        req.Select = shipToken["select"].ToObject<int[]>();
                    }
                }

                requirements.Add(req);
            }

            return requirements;
        }
    }

    
}
