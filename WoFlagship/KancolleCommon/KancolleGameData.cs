using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleQuestData;
using WoFlagship.Utils;

namespace WoFlagship.KancolleCommon
{

    public class KancolleGameData
    {
        public const string QuestInfoFile = "Resources\\Infos\\questinfo.json";

        public ReadOnlyDictionary<int, KancolleMapInfoData> MapInfoDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleMapInfoData>(new Dictionary<int, KancolleMapInfoData>());
        public KancolleBasicInfo BasicInfo { get; set; }
        public ReadOnlyDictionary<int, KancolleQuest> QuestDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleQuest>(new Dictionary<int, KancolleQuest>());
        public ReadOnlyDictionary<int, KancolleQuestInfoItem> QuestInfoDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleQuestInfoItem>(new Dictionary<int, KancolleQuestInfoItem>());
        public KancolleMaterial Material { get; set; }
        public ReadOnlyDictionary<int, KancolleShipData> ShipDataDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleShipData>(new Dictionary<int, KancolleShipData>());
        public ReadOnlyDictionary<int, KancolleShip> OwnedShipDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleShip>(new Dictionary<int, KancolleShip>());

        public ReadOnlyDictionary<int, KancolleMissionData> MissionDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleMissionData>(new Dictionary<int, KancolleMissionData>());
        public ReadOnlyDictionary<int, KancolleMissson> OwnedMissionDic { get; set; } = new ReadOnlyDictionary<int, KancolleMissson>(new Dictionary<int, KancolleMissson>());
        //用于双向查询舰娘的位置
        /// <summary>
        /// key:ownedShipId 
        /// value:Tuple(i,j),表示第i个舰队，第j个位置，从0开始算
        /// </summary>
        public ReadOnlyDictionary<int, Tuple<int, int>> OwnedShipPlaceDictionary { get; set; } = new ReadOnlyDictionary<int, Tuple<int, int>>(new Dictionary<int, Tuple<int, int>>());
        /// <summary>
        /// (i,j),表示第i个舰队，第j个位置，从0开始算
        /// 返回的是ownedShipId
        /// </summary>
        public ReadOnlyArray2<int> OwnedShipPlaceArray { get; set; } = new ReadOnlyArray2<int>(new int[4, 6]);

        public ReadOnlyDictionary<int, KancolleSlotItemData> SlotDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleSlotItemData>(new Dictionary<int, KancolleSlotItemData>());
        public ReadOnlyDictionary<int, KancolleSlotItem> OwnedSlotDictionary { get; set; } = new ReadOnlyDictionary<int, KancolleSlotItem>(new Dictionary<int, KancolleSlotItem>());

    }

    /// <summary>
    /// 提督基本信息
    /// </summary>
    public class KancolleBasicInfo
    {
        /// <summary>
        /// 提督等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 头衔id
        /// </summary>
        public int RankId { get; private set; }

        /// <summary>
        /// 头衔
        /// </summary>
        public string Rank { get; private set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; private set; }

        /// <summary>
        /// 拥有的舰娘个数
        /// </summary>
        public int OwnedShipCount { get; private set; }

        /// <summary>
        /// 最大可保有舰娘个数
        /// </summary>
        public int MaxShipCount { get; private set; }

        /// <summary>
        /// 最大可保有装备个数
        /// </summary>
        public int MaxSlotItemCount { get; private set; }

        public KancolleBasicInfo(api_port_data portdata)
        {
            Level = portdata.api_basic.api_level;
            RankId = portdata.api_basic.api_rank;
            Rank = KancolleAPIs.RankText[portdata.api_basic.api_rank];
            NickName = portdata.api_basic.api_nickname;
            OwnedShipCount = portdata.api_ship.Length;
            MaxShipCount = portdata.api_basic.api_max_chara;
            MaxSlotItemCount = portdata.api_basic.api_max_slotitem;
        }
    }

    /// <summary>
    /// 舰娘信息
    /// </summary>
    public class KancolleShip
    {
        /// <summary>
        /// OwnedId，按照船的获得顺序生成
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; private set; }

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

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHP { get; private set; }

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int NowHP { get; private set; }

        public int[] Slot { get; private set; }

        public int[] OnSlot { get; private set; }

        public KancolleShip(api_ship_item ship)
        {
            Id = ship.api_id;
            ShipId = ship.api_ship_id;
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
            MaxHP = ship.api_maxhp;
            NowHP = ship.api_nowhp;
            Slot = ship.api_slot.ToArray();
            OnSlot = ship.api_onslot.ToArray();
        }
    }

    /// <summary>
    /// 舰娘信息（数据库）
    /// </summary>
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

    public class KancolleMissson
    {
        /// <summary>
        /// 远征数据库对应的id
        /// </summary>
        public int MissionId { get; private set; }

        public int State { get; private set; }

        public KancolleMissson(api_mission_item mission_item)
        {
            MissionId = mission_item.api_mission_id;
            State = mission_item.api_state;
        }
    }


    /// <summary>
    /// 远征信息（数据库）
    /// </summary>
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

    /// <summary>
    /// 资源信息
    /// </summary>
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

    /// <summary>
    /// 任务信息
    /// </summary>
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

    /// <summary>
    /// 地图信息（数据库）
    /// </summary>
    public class KancolleMapInfoData
    {
        public int Id { get; private set; }

        /// <summary>
        /// 海域id
        /// </summary>
        public int MapAreaId { get; private set; }

        /// <summary>
        /// 海域的第几个地图
        /// </summary>
        public int MapNo { get; private set; }

        public string Name { get; private set; }

        public int Level { get; private set; }

        public string InfoText { get; private set; }

        public KancolleMapInfoData(api_mst_mapinfo_item mapinfo_item)
        {
            Id = mapinfo_item.api_id;
            MapAreaId = mapinfo_item.api_maparea_id;
            MapNo = mapinfo_item.api_no;
            Name = mapinfo_item.api_name;
            Level = mapinfo_item.api_level;
            InfoText = mapinfo_item.api_infotext;
        }
    }

    /// <summary>
    /// 装备信息
    /// </summary>
    public class KancolleSlotItem
    {
        public int Id { get; private set; }

        /// <summary>
        /// 对应到装备数据库的id
        /// </summary>
        public int SlotItemId { get; private set; }

        public bool Locked { get; private set; }

        public int Level { get; private set; }

        public KancolleSlotItem(api_slot_item_item slot_item_item)
        {
            Id = slot_item_item.api_id;
            SlotItemId = slot_item_item.api_slotitem_id;
            Locked = slot_item_item.api_locked != 0;
            Level = slot_item_item.api_level;
        }
    }

    /// <summary>
    /// 装备信息（数据库）
    /// </summary>
    public class KancolleSlotItemData
    {
        public int Id { get; private set; }

        /// <summary>
        /// 当前分类下顺序
        /// </summary>
        public int SortNo { get; private set; }

        /// <summary>
        /// 装备名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 装备类型
        /// </summary>
        public int[] Type { get; private set; }

        /// <summary>
        /// 最大HP
        /// </summary>
        public int Taik { get; private set; }

        /// <summary>
        /// 装甲
        /// </summary>
        public int Souk { get; private set; }

        /// <summary>
        /// 火力
        /// </summary>
        public int Houg { get; set; }

        /// <summary>
        /// 雷装
        /// </summary>
        public int Raig { get; private set; }

        /// <summary>
        /// 速度
        /// </summary>
        public int Soku { get; private set; }

        /// <summary>
        /// Dive bomber??
        /// </summary>
        public int Baku { get; private set; }

        /// <summary>
        /// 对空
        /// </summary>
        public int Tyku { get; private set; }

        /// <summary>
        /// 对潜
        /// </summary>
        public int Tais { get; private set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Atap { get; private set; }

        /// <summary>
        /// 命中
        /// </summary>
        public int Houm { get; private set; }

        /// <summary>
        /// Unkown
        /// </summary>
        public int Raim { get; private set; }

        /// <summary>
        /// 回避
        /// </summary>
        public int Houk { get; private set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Raik { get; private set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Bakk { get; private set; }

        /// <summary>
        /// Line of Sight??
        /// </summary>
        public int Saku { get; private set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Sakb { get; private set; }

        /// <summary>
        /// 运
        /// </summary>
        public int Luck { get; private set; }

        /// <summary>
        /// 射程
        /// </summary>
        public int Leng { get; private set; }

        /// <summary>
        /// 稀有度
        /// </summary>
        public int Rare { get; private  set; }

        /// <summary>
        /// 废弃后所能获得的材料(燃弹钢铝)
        /// </summary>
        public Tuple<int,int,int,int> Broken { get; private set; }

        /// <summary>
        /// 装备信息
        /// </summary>
        public string Info { get; private set; }

        /// <summary>
        /// unused
        /// </summary>
        public string UseBull { get; private set; }

        public KancolleSlotItemData(api_mst_slotitem_item mst_slotitem_item)
        {
            Id = mst_slotitem_item.api_id;
            SortNo = mst_slotitem_item.api_sortno;
            Name = mst_slotitem_item.api_name;
            Type = mst_slotitem_item.api_type;
            Taik = mst_slotitem_item.api_taik;
            Souk = mst_slotitem_item.api_souk;
            Houg = mst_slotitem_item.api_houg;
            Raig = mst_slotitem_item.api_raig;
            Soku = mst_slotitem_item.api_soku;
            Baku = mst_slotitem_item.api_baku;
            Tyku = mst_slotitem_item.api_tyku;
            Tais = mst_slotitem_item.api_tais;
            Atap = mst_slotitem_item.api_atap;
            Houm = mst_slotitem_item.api_houm;
            Raim = mst_slotitem_item.api_raim;
            Houk = mst_slotitem_item.api_houk;
            Raik = mst_slotitem_item.api_raik;
            Bakk = mst_slotitem_item.api_bakk;
            Saku = mst_slotitem_item.api_saku;
            Sakb = mst_slotitem_item.api_sakb;
            Luck = mst_slotitem_item.api_luck;
            Rare = mst_slotitem_item.api_rare;
            Broken = new Tuple<int, int, int, int>(mst_slotitem_item.api_broken[0], mst_slotitem_item.api_broken[1], mst_slotitem_item.api_broken[2], mst_slotitem_item.api_broken[3]);
            Info = mst_slotitem_item.api_info;
            UseBull = mst_slotitem_item.api_usebull;
        }
    }
}
