using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleQuest;

namespace WoFlagship.KancolleCommon
{

    [Serializable]
    public class KancolleGameData
    {
        public const string QuestInfoFile = "Resources\\Infos\\questinfo.json";

        public Dictionary<int, api_mst_mapinfo_item> MapDic { get; set; } = new Dictionary<int, api_mst_mapinfo_item>();

        public Dictionary<int, api_questlist_item> QuestDic { get; set; } = new Dictionary<int, api_questlist_item>();
        public Dictionary<int, QuestInfoItem> QuestInfoDic { get; set; } = new Dictionary<int, QuestInfoItem>();


        /// <summary>
        /// key:shipId
        /// value:api_mst_ship_item
        /// </summary>
        public Dictionary<int, api_mst_ship_item> ShipDic { get; set; } = new Dictionary<int, api_mst_ship_item>();
        public Dictionary<int, api_ship_item> OwnedShipDic { get; set; } = new Dictionary<int, api_ship_item>();
        public Dictionary<int, api_mst_mission_item> MissionDic { get; set; } = new Dictionary<int, api_mst_mission_item>();
        public Dictionary<int, api_mission_item> OwnedMissionDic { get; set; } = new Dictionary<int, api_mission_item>();
        //用于双向查询舰娘的位置
        /// <summary>
        /// key:ownedShipId 
        /// value:KeyValuePair(i,j),表示第i个舰队，第j个位置，从0开始算
        /// </summary>
        public Dictionary<int, KeyValuePair<int, int>> OwnedShipPlaceDic { get; set; } = new Dictionary<int, KeyValuePair<int, int>>();
        /// <summary>
        /// (i,j),表示第i个舰队，第j个位置，从0开始算
        /// 返回的是ownedShipId
        /// </summary>
        public int[,] OwnedShipPlaceArray { get; set; } = new int[4, 6];

        public Dictionary<int, api_mst_slotitem_item> SlotDic { get; set; } = new Dictionary<int, api_mst_slotitem_item>();
        public Dictionary<int, api_slot_item_item> OwnedSlotDic { get; set; } = new Dictionary<int, api_slot_item_item>();

      
        /// <summary>
        /// 将ownedShipId转为ShipId
        /// </summary>
        /// <param name="ownedShipId"></param>
        /// <returns></returns>
        public int OwnedShipToShip(int ownedShipId)
        {
            if (ownedShipId == -1)
                return -1;
            api_ship_item shipItem;
            if (OwnedShipDic.TryGetValue(ownedShipId, out shipItem))
            {
                return shipItem.api_ship_id;
            }
            return -1;
        }

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
        }
    }

    public class KancolleShip
    {
        /// <summary>
        /// OwnedId，按照船的获得顺序生成
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; set; }

        /// <summary>
        /// 船的类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
    }
}
