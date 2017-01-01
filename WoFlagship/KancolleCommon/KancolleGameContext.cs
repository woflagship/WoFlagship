﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCommon
{
    public class KancolleGameContext
    {
        public event Action<KancolleGameContext> OnShipUpdated;
        public event Action<KancolleGameContext> OnDeckUpdated;
        public event Action<KancolleGameContext> OnMaterialUpdated;
        public event Action<KancolleGameContext> OnQuestUpdated;

        public event Action<KancolleGameContext> OnGameDataUpdated;

        private api_mst_ship_item[] api_mst_ship;
        private api_ship_item[] api_ship;
        private api_mst_mission_item[] api_mst_mission;

        private KancolleGameData gameData = new KancolleGameData();
        public KancolleGameData GameData { get { return gameData; } }

        /// <summary>
        /// 更新从api_mst_ship中获得的舰娘数据信息
        /// </summary>
        /// <param name="items"></param>
        public void UpdateShipDatas(api_mst_ship_item[] items)
        {
            api_mst_ship = items;
            gameData.ShipDataDictionary = new ReadOnlyDictionary<int, KancolleShipData>(items.ToDictionary(
                k=>k.api_id,
                k=>new KancolleShipData(k)
                ));

            OnGameDataUpdated?.Invoke(this);
        }

        /// <summary>
        /// 更新从api_ship中获得的当前已有的舰娘信息
        /// </summary>
        /// <param name="items"></param>
        public void UpdateOwnedShips(api_ship_item[] items)
        {
            api_ship = items;
            if (api_mst_ship == null)
                throw new Exception("需要先更新舰娘数据信息api_mst_item");
            gameData.OwnedShipDictionary = new ReadOnlyDictionary<int, KancolleShip>(items.ToDictionary(
                    k => k.api_id, 
                    k => new KancolleShip(k, gameData.ShipDataDictionary[k.api_ship_id])
                    ));

            OnShipUpdated?.Invoke(this);
            OnGameDataUpdated?.Invoke(this);
        }

        /// <summary>
        /// 更新从api_mst_mission中获得的远征数据信息
        /// </summary>
        /// <param name="missions"></param>
        public void UpdateMissions(api_mst_mission_item[] missions)
        {
            api_mst_mission = missions;
            gameData.MissionDictionary = new ReadOnlyDictionary<int, KancolleMissionData>(missions.ToDictionary(
                k=>k.api_id,
                k=>new KancolleMissionData(k)
                ));
            OnGameDataUpdated?.Invoke(this);
        }

        /// <summary>
        /// 调用port的API时会更新deck
        /// </summary>
        /// <param name="deckPorts"></param>
        public void UpdateDeck(api_deck_port_item[] deckPorts)
        {
            gameData.OwnedShipPlaceDictionary.Clear();
            for (int i = 0; i < deckPorts.Length; i++)
            {
                for (int j = 0; j < deckPorts[i].api_ship.Length; j++)
                {
                    int ownedShipId = deckPorts[i].api_ship[j];//在第i个舰队第j个位置的舰娘id
                    if (ownedShipId > 0)
                    {
                        var ship = gameData.OwnedShipDictionary[ownedShipId];
                        gameData.OwnedShipPlaceDictionary.Add(ownedShipId, new Tuple<int, int>(i, j));
                    }
                    gameData.OwnedShipPlaceArray[i, j] = ownedShipId;

                }
            }
            OnDeckUpdated?.Invoke(this);
            OnGameDataUpdated?.Invoke(this);
        }

        /// <summary>
        /// 当改变船的编成位置时更新deck
        /// </summary>
        /// <param name="shipChangePostData"></param>
        public void UpdateDeck(Dictionary<string, string> shipChangePostData)
        {
            int api_id = int.Parse(shipChangePostData["api_id"]);
            int deck_index = api_id - 1;
            int api_ship_id = int.Parse(shipChangePostData["api_ship_id"]);
            int api_ship_idx = int.Parse(shipChangePostData["api_ship_idx"]);
            if (api_ship_id > 0)//添加
            {
                var ship = gameData.OwnedShipDictionary[api_ship_id];

                //交换自己的位置和目标位置
                if (gameData.OwnedShipPlaceDictionary.ContainsKey(api_ship_id))//本来就在编队中
                {
                    var place = gameData.OwnedShipPlaceDictionary[api_ship_id];//本来的位置
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDictionary[api_ship_id] = new Tuple<int, int>(deck_index, api_ship_idx);
                        //原位置清空      
                        int i = 0;
                        for (i = place.Item2; i < gameData.OwnedShipPlaceArray.GetLength(1) - 1; i++)
                        {
                            int nextShipId = gameData.OwnedShipPlaceArray[place.Item1, i + 1];
                            if (nextShipId > 0)
                                gameData.OwnedShipPlaceDictionary[nextShipId] = new Tuple<int, int>(place.Item1, i);
                            gameData.OwnedShipPlaceArray[place.Item1, i] = nextShipId;
                        }
                        gameData.OwnedShipPlaceArray[place.Item1, i] = -1;
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceArray[place.Item1, place.Item2] = dstShip;
                        gameData.OwnedShipPlaceDictionary[api_ship_id] = new Tuple<int, int>(deck_index, api_ship_idx);
                        gameData.OwnedShipPlaceDictionary[dstShip] = new Tuple<int, int>(place.Item1, place.Item2);
                    }
                }
                else//本来不在编队中
                {
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDictionary.Add(api_ship_id, new Tuple<int, int>(deck_index, api_ship_idx));
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        gameData.OwnedShipPlaceArray[deck_index, api_ship_idx] = api_ship_id;
                        gameData.OwnedShipPlaceDictionary.Add(api_ship_id, new Tuple<int, int>(deck_index, api_ship_idx));
                        gameData.OwnedShipPlaceDictionary.Remove(dstShip);

                    }
                }
            }
            else if (api_ship_id == -1)//移除
            {
                int i;
                gameData.OwnedShipPlaceDictionary.Remove(gameData.OwnedShipPlaceArray[deck_index, api_ship_idx]);
                for (i = api_ship_idx; i < gameData.OwnedShipPlaceArray.GetLength(1) - 1; i++)
                {//后面的向前补充
                    int nextShipId = gameData.OwnedShipPlaceArray[deck_index, i + 1];
                    if (nextShipId > 0)
                        gameData.OwnedShipPlaceDictionary[nextShipId] = new Tuple<int, int>(deck_index, i);
                    gameData.OwnedShipPlaceArray[deck_index, i] = nextShipId;
                }
                gameData.OwnedShipPlaceArray[deck_index, i] = -1;

            }
            else if (api_ship_id == -2)//移除旗舰外所有舰船
            {
                for (int i = 1; i < gameData.OwnedShipPlaceArray.GetLength(1); i++)
                {
                    gameData.OwnedShipPlaceDictionary.Remove(gameData.OwnedShipPlaceArray[deck_index, i]);
                    gameData.OwnedShipPlaceArray[deck_index, i] = -1;
                }
            }

            OnDeckUpdated?.Invoke(this);
            OnGameDataUpdated?.Invoke(this);
        }

        /// <summary>
        /// 更新资源信息
        /// </summary>
        /// <param name="materials"></param>
        public void UpdateMaterial(api_material_item[] materials)
        {
            if (materials != null)
            {
                gameData.Material = new KancolleMaterial(materials);
                OnMaterialUpdated?.Invoke(this);
                OnGameDataUpdated?.Invoke(this);
            }

        }

        /// <summary>
        /// 更新资源信息，只更新燃弹钢铝
        /// </summary>
        /// <param name="materials"></param>
        public void UpdateMaterial(int[] materials)
        {
            if (materials != null)
            {
                gameData.Material = new KancolleMaterial(gameData.Material, materials);
                OnMaterialUpdated?.Invoke(this);
                OnGameDataUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// 更新任务信息
        /// </summary>
        /// <param name="questData"></param>
        public void UpdateQuest(api_questlist_data questData)
        {
            if (questData != null && questData.api_list != null)
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
                if (validQuestList.Count > 0)
                {
                    Dictionary<int, KancolleQuest> dic = gameData.QuestDictionary.ToDictionary(k=>k.Key, k=>k.Value);
                    foreach (var quest in validQuestList)
                    {
                        if (dic.ContainsKey(quest.api_no))
                            dic[quest.api_no] = new KancolleQuest(quest);
                        else
                            dic.Add(quest.api_no, new KancolleQuest(quest));
                    }
                    gameData.QuestDictionary = new ReadOnlyDictionary<int, KancolleQuest>(dic);
                    OnQuestUpdated?.Invoke(this);
                    OnGameDataUpdated?.Invoke(this);
                }
            }
        }

        
    }
}
