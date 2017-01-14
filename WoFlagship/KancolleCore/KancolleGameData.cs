using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WoFlagship.KancolleQuestData;
using WoFlagship.Utils;

namespace WoFlagship.KancolleCore
{

    public class KancolleGameData
    {
        public const string QuestInfoFile = "Resources\\Infos\\questinfo.json";

        public ReadOnlyDictionary<int, KancolleShipType> ShipTypeDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShipType>(new Dictionary<int, KancolleShipType>());
        public ReadOnlyDictionary<int, KancolleItemEquipType> ItemEquipTypeDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleItemEquipType>(new Dictionary<int, KancolleItemEquipType>());

        public ReadOnlyDictionary<int, KancolleMapInfoData> MapInfoDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleMapInfoData>(new Dictionary<int, KancolleMapInfoData>());
        public KancolleBasicInfo BasicInfo { get; internal set; } = new KancolleBasicInfo();
        public ReadOnlyDictionary<int, KancolleQuest> QuestDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleQuest>(new Dictionary<int, KancolleQuest>());
        public ReadOnlyDictionary<int, KancolleQuestInfoItem> QuestInfoDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleQuestInfoItem>(new Dictionary<int, KancolleQuestInfoItem>());
        public KancolleMaterial Material { get; internal set; } = new KancolleMaterial();
        public ReadOnlyDictionary<int, KancolleShipData> ShipDataDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShipData>(new Dictionary<int, KancolleShipData>());
        public ReadOnlyDictionary<int, KancolleShip> OwnedShipDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShip>(new Dictionary<int, KancolleShip>());

        public ReadOnlyDictionary<int, KancolleMissionData> MissionDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleMissionData>(new Dictionary<int, KancolleMissionData>());
        public ReadOnlyDictionary<int, KancolleMissson> OwnedMissionDic { get; internal set; } = new ReadOnlyDictionary<int, KancolleMissson>(new Dictionary<int, KancolleMissson>());
        //用于双向查询舰娘的位置
        /// <summary>
        /// key:ownedShipId 
        /// value:Tuple(i,j),表示第i个舰队，第j个位置，从0开始算
        /// </summary>
        public ReadOnlyDictionary<int, Tuple<int, int>> OwnedShipPlaceDictionary { get; internal set; } = new ReadOnlyDictionary<int, Tuple<int, int>>(new Dictionary<int, Tuple<int, int>>());
        /// <summary>
        /// (i,j),表示第i个舰队，第j个位置，从0开始算
        /// 返回的是ownedShipId
        /// </summary>
        public ReadOnlyArray2<int> OwnedShipPlaceArray { get; internal set; } = new ReadOnlyArray2<int>(new int[4, 6]);

        public ReadOnlyDictionary<int, KancolleSlotItemData> SlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleSlotItemData>(new Dictionary<int, KancolleSlotItemData>());
        public ReadOnlyDictionary<int, KancolleSlotItem> OwnedSlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleSlotItem>(new Dictionary<int, KancolleSlotItem>());
        /// <summary>
        /// 已经装备的装备字典；key:itemNo,value:装备的舰娘no
        /// </summary>
        public ReadOnlyDictionary<int, int> EquipedSlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, int>(new Dictionary<int,int>());
        /// <summary>
        /// 尚未装备的装备列表，每个元素为itemNo
        /// </summary>
        public ReadOnlyCollection<int> UnEquipedSlotArray { get; internal set; } = new ReadOnlyCollection<int>(new List<int>());

        #region public methods
        /// <summary>
        /// 从ownedShip获得数据库的shipData，若不存在返回null
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public KancolleShipData GetShip(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.No, out shipData))
                return shipData;
            return null;
        }

        /// <summary>
        /// 从ownedShipId获得数据库的shipData，若不存在返回null
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public KancolleShipData GetShip(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if (OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShip(ownedShip);
            return null;
        }

        /// <summary>
        /// 获取舰娘类型id，若不存在返回-1
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public int GetShipType(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.No, out shipData))
                return shipData.Type;
            return -1;
        }

        /// <summary>
        /// 获取舰娘类型id，若不存在返回-1
        /// </summary>
        /// <param name="ownedShipNo"></param>
        /// <returns></returns>
        public int GetShipType(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if (OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShipType(ownedShip);
            return -1;
        }

        /// <summary>
        /// 获取舰娘名，若不存在返回null
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public string GetShipName(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.ShipId, out shipData))
                return shipData.Name;
            return null;
        }

        /// <summary>
        /// 获取舰娘名，若不存在返回null
        /// </summary>
        /// <param name="ownedShipNo"></param>
        /// <returns></returns>
        public string GetShipName(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if(OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShipName(ownedShip);
            return null;
        }

        /// <summary>
        /// 获取装备名，若不存在则返回null
        /// </summary>
        /// <param name="ownedItem"></param>
        /// <returns></returns>
        public string GetSlotItemName(KancolleSlotItem ownedItem)
        {
            KancolleSlotItemData itemData;
            if (SlotDictionary.TryGetValue(ownedItem.SlotItemId, out itemData))
                return itemData.Name;
            return null;
        }

        /// <summary>
        /// 获取装备名，若不存在则返回null
        /// </summary>
        /// <param name="ownedItemNo"></param>
        /// <returns></returns>
        public string GetSlotItemName(int ownedItemNo)
        {
            KancolleSlotItem item;
            if (OwnedSlotDictionary.TryGetValue(ownedItemNo, out item))
                return GetSlotItemName(item);
            return null;
        }

        /// <summary>
        /// 舰娘能否佩戴该装备，可以返回true，否则返回false；舰娘、装备没有找到都返回false
        /// </summary>
        /// <param name="shipId">舰娘id</param>
        /// <param name="itemId">装备id</param>
        /// <returns></returns>
        public bool CanShipEquipItem(int shipId, int itemId)
        {
            KancolleShipData ship;
            if(ShipDataDictionary.TryGetValue(shipId, out ship))
            {
                KancolleShipType shipType;
                //找到舰娘类型
                if(ShipTypeDictionary.TryGetValue(ship.Type, out shipType))
                {
                    //舰娘的可装备字典
                    var dic = shipType.EquipItemType;
                    KancolleSlotItemData item;
                    //找到装备
                    if(SlotDictionary.TryGetValue(itemId, out item))
                    {
                        //item3为该装备的装备类型                  
                        if(dic.ContainsKey(item.Type.Item3))
                        {
                            //可装备（1）则为true，否则为false
                            return dic[item.Type.Item3] != 0;
                        }

                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// 舰娘能否佩戴该装备，可以返回true，否则返回false；舰娘、装备为null或者没有找到都返回false
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanShipEquipItem(KancolleShipData ship, KancolleSlotItemData item)
        {
            if (ship == null || item == null)
                return false;
            return CanShipEquipItem(ship.ShipId, item.Id);
        }
        #endregion

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

        internal KancolleBasicInfo() { }

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
        /// OwnedNo，按照船的获得顺序生成
        /// </summary>
        public int No { get; private set; }

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

        /// <summary>
        /// 当前装备,No集合
        /// </summary>
        public int[] Slot { get; private set; }

        /// <summary>
        /// 未知
        /// </summary>
        public int[] OnSlot { get; private set; }

        public KancolleShip(api_ship_item ship)
        {
            No = ship.api_id;
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

    public class KancolleItemEquipType
    {
        /// <summary>
        /// 装备类型id
        /// </summary>
        public int TypeId { get; private set; }

        /// <summary>
        /// 装备名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 不清楚
        /// </summary>
        public int ShowFlag { get; private set; }

        public KancolleItemEquipType(api_mst_slotitem_equiptype_item equiptype_item)
        {
            TypeId = equiptype_item.api_id;
            Name = equiptype_item.api_name;
            ShowFlag = equiptype_item.api_show_flg;
        }
    }

    /// <summary>
    /// 舰种
    /// </summary>
    public class KancolleShipType
    {
        /// <summary>
        /// 舰种id
        /// </summary>
        public int TypeId { get; private set; }

        public int SortNo { get; private set; }

        /// <summary>
        /// 舰种名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 入渠时间的倍率
        /// </summary>
        public int Scnt { get; private set; }

        /// <summary>
        /// 建造时的剪影
        /// </summary>
        public int Kcnt { get; private set; }

        /// <summary>
        /// 可装备的装备类型，key：ItemEquipType的id，value：0为不可用， 1为可用
        /// </summary>
        public ReadOnlyDictionary<int, int> EquipItemType { get; private set; }

        public KancolleShipType(api_mst_stype_item stype_item)
        {
            TypeId = stype_item.api_id;
            SortNo = stype_item.api_sortno;
            Name = stype_item.api_name;
            Scnt = stype_item.api_scnt;
            Kcnt = stype_item.api_kcnt;
            EquipItemType = new ReadOnlyDictionary<int, int>(stype_item.api_equip_type.ToDictionary(
                k=>int.Parse(k.Key),
                k=>k.Value));
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

        internal KancolleMaterial() { }

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
        /// <summary>
        /// 装备顺序编号
        /// </summary>
        public int No { get; private set; }

        /// <summary>
        /// 对应到装备数据库的id
        /// </summary>
        public int SlotItemId { get; private set; }

        public bool Locked { get; private set; }

        public int Level { get; private set; }

        public KancolleSlotItem(api_slot_item_item slot_item_item)
        {
            No = slot_item_item.api_id;
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
        /// [0]:大分類
        /// [1]:夜戦判定
        /// [2]:装備可能艦種判定(即api_mst_slotitem_equiptype)
        /// [3]:不明
        /// </summary>
        public Tuple<int,int,int,int> Type { get; private set; }

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
            Type = new Tuple<int, int, int, int>(mst_slotitem_item.api_type[0], mst_slotitem_item.api_type[1], mst_slotitem_item.api_type[2], mst_slotitem_item.api_type[3]);
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
