using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleQuestData;

namespace WoFlagship.KancolleCommon
{

    public class KancolleGameData
    {
        public const string QuestInfoFile = "Resources\\Infos\\questinfo.json";

        public Dictionary<int, api_mst_mapinfo_item> MapDic { get; set; } = new Dictionary<int, api_mst_mapinfo_item>();

        public ReadOnlyDictionary<int, KancolleQuest> QuestDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleQuest>(new Dictionary<int, KancolleQuest>());
        public ReadOnlyDictionary<int, KancolleQuestInfoItem> QuestInfoDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleQuestInfoItem>(new Dictionary<int, KancolleQuestInfoItem>());
        public KancolleMaterial Material { get; set; }

        /// <summary>
        /// key:shipId
        /// value:api_mst_ship_item
        /// </summary>
      
        public ReadOnlyDictionary<int, KancolleShipData> ShipDataDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleShipData>(new Dictionary<int, KancolleShipData>());
        public ReadOnlyDictionary<int, KancolleShip> OwnedShipDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleShip>(new Dictionary<int, KancolleShip>());
     
        public Dictionary<int, api_mst_mission_item> MissionDic { get; set; } = new Dictionary<int, api_mst_mission_item>();
        public ReadOnlyDictionary<int, KancolleMissionData> MissionDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleMissionData>(new Dictionary<int, KancolleMissionData>());
        public Dictionary<int, api_mission_item> OwnedMissionDic { get; set; } = new Dictionary<int, api_mission_item>();
        //用于双向查询舰娘的位置
        /// <summary>
        /// key:ownedShipId 
        /// value:Tuple(i,j),表示第i个舰队，第j个位置，从0开始算
        /// </summary>
        public Dictionary<int, Tuple<int, int>> OwnedShipPlaceDictionary { get; set; } = new Dictionary<int, Tuple<int, int>>();
        /// <summary>
        /// (i,j),表示第i个舰队，第j个位置，从0开始算
        /// 返回的是ownedShipId
        /// </summary>
        public int[,] OwnedShipPlaceArray { get; set; } = new int[4, 6];

        public Dictionary<int, api_mst_slotitem_item> SlotDic { get; set; } = new Dictionary<int, api_mst_slotitem_item>();
        public Dictionary<int, api_slot_item_item> OwnedSlotDic { get; set; } = new Dictionary<int, api_slot_item_item>();


        /*
        public KancolleGameData Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                var clonedData = formatter.Deserialize(stream) as KancolleGameData;
                stream.Close();
                return clonedData;
            }
        }*/
    }

    public class KancolleShip
    {
        /// <summary>
        /// OwnedId，按照船的获得顺序生成
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 舰娘名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; private set; }

        /// <summary>
        /// 船的类型
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 疲劳值
        /// </summary>
        public int Condition { get; private set; }

        /// <summary>
        /// 火力（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Karyoku { get; private set; }

        /// <summary>
        /// 对空（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Taiku { get; private set; }

        /// <summary>
        /// 雷装（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Raisou { get; private set; }

        /// <summary>
        /// 装甲（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Soukou { get; private set; }

        /// <summary>
        /// 回避（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Kaihi { get; private set; }

        /// <summary>
        /// 对潜（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Taisen { get; private set; }

        /// <summary>
        /// 索敌
        /// </summary>
        public Tuple<int, int> Sakuteki { get; set; }

        /// <summary>
        /// 运（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Lucky { get; private set; }

        /// <summary>
        /// 是否已锁
        /// </summary>
        public bool Locked { get; private set; }

        public int MaxHP { get; set; }

        public int NowHP { get; set; }

        public KancolleShip(api_ship_item ship, KancolleShipData shipData)
        {
            Id = ship.api_id;
            ShipId = ship.api_ship_id;
            Name = shipData.Name;
            Type = shipData.Type;
            Level = ship.api_lv;
            Condition = ship.api_cond;
            Karyoku = new Tuple<int, int>(ship.api_karyoku[0], ship.api_karyoku[1]);
            Taiku = new Tuple<int, int>(ship.api_taiku[0], ship.api_taiku[1]);
            Raisou = new Tuple<int, int>(ship.api_raisou[0], ship.api_raisou[1]);
            Soukou = new Tuple<int, int>(ship.api_soukou[0], ship.api_soukou[1]);
            Kaihi = new Tuple<int, int>(ship.api_kaihi[0], ship.api_kaihi[1]);
            Taisen = new Tuple<int, int>(ship.api_taisen[0], ship.api_taisen[1]);
            Sakuteki = new Tuple<int, int>(ship.api_sakuteki[0], ship.api_sakuteki[1]);
            Lucky = new Tuple<int, int>(ship.api_lucky[0], ship.api_lucky[1]);
            Locked = ship.api_locked == 0;
        }
    }

    public class KancolleShipData
    {
        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; private set; }

        /// <summary>
        /// 舰娘名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 船的类型
        /// </summary>
        public int Type { get; private set; }

        public KancolleShipData(api_mst_ship_item mst_ship_item)
        {
            ShipId = mst_ship_item.api_id;
            Name = mst_ship_item.api_name;
            Type = mst_ship_item.api_stype;
        }
    }

    public class KancolleMissionData
    {
        /// <summary>
        /// 远征Id
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        /// 远征名
        /// </summary>
        public string Name { get; set; }

        public KancolleMissionData(api_mst_mission_item mst_mission_item)
        {
            MissionId = mst_mission_item.api_id;
            Name = mst_mission_item.api_name;
        }
    }

    public class KancolleMaterial
    {
        public int Ran { get; private set; }
        public int Dan { get; private set; }
        public int Gang { get; private set; }
        public int Lv { get; private set; }

        /// <summary>
        /// 建造资材
        /// </summary>
        public int Jianzao { get; private set; }

        /// <summary>
        /// 修复桶
        /// </summary>
        public int Xiufu { get; private set; }

        /// <summary>
        /// 开发资材
        /// </summary>
        public int Kaifa { get; private set; }

        /// <summary>
        /// 改修紫菜
        /// </summary>
        public int Gaixiu { get; private set; }

        public KancolleMaterial(api_material_item[] materials)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    switch (material.api_id)
                    {
                        case 1:
                            Ran = material.api_value;
                            break;
                        case 2:
                            Dan = material.api_value;
                            break;
                        case 3:
                            Gang = material.api_value;
                            break;
                        case 4:
                            Lv = material.api_value;
                            break;
                        case 5:
                            Jianzao = material.api_value;
                            break;
                        case 6:
                            Xiufu = material.api_value;
                            break;
                        case 7:
                            Kaifa = material.api_value;
                            break;
                        case 8:
                            Gaixiu = material.api_value;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 以materials为燃弹钢铝的值
        /// 剩余资材和桶以mat为准
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="materials"></param>
        public KancolleMaterial(KancolleMaterial mat, int[] materials)
        {
            Ran = materials[0];
            Dan = materials[1];
            Gang = materials[2];
            Lv = materials[3];

            Gaixiu = mat.Gaixiu;
            Kaifa = mat.Kaifa;
            Xiufu = mat.Xiufu;
            Jianzao = mat.Jianzao;
        }


    }

    public class KancolleQuest
    {
        public int Id { get; private set; }

        public int Category { get; private set; }

        public int Type { get; private set; }

        /// <summary>
        /// state=3表示完成，state=2表示任务接受 state=1表示未接收
        /// </summary>
        public int State { get; private set; }

        public string Title { get; set; }

        public string Detail { get; private set; }

        public Tuple<int, int, int, int> Material { get; private set; }

        /// <summary>
        /// 0-0% 1-50% 2-80%
        /// </summary>
        public int ProgressFlag { get; private set; }

        public KancolleQuest(api_questlist_item api_questlist)
        {
            Id = api_questlist.api_no;
            Category = api_questlist.api_category;
            Type = api_questlist.api_type;
            State = api_questlist.api_state;
            Title = api_questlist.api_title;
            Detail = api_questlist.api_detail;
            Material = new Tuple<int, int, int, int>(api_questlist.api_get_material[0], api_questlist.api_get_material[1], api_questlist.api_get_material[2], api_questlist.api_get_material[3]);
            ProgressFlag = api_questlist.api_progress_flag;
        }
    }
}
