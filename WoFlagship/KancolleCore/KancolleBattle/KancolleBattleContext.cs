using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WoFlagship.Utils;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class KancolleBattleContext : IKancolleAPIReceiver
    {
        public event Action<Battle> OnBattleHappened;
        public event Action<Battle, BattleResult> OnBattleResultReceived;

        public KancolleGameData GameData { get; private set; }

       
        public int FleetType { get; private set; }
        public Fleet MainFleet { get; private set; } = null;
        public Fleet EscortFleet { get; private set; } = null;
        public Fleet SupportFleet { get; private set; } = null;
        public Fleet NormalSF { get; private set; } = null;
        public Fleet BossSF { get; private set; } = null;
        public Battle CurrentBattle { get; private set; } = null;

        public KancolleBattleContext(KancolleGameData gameData)
        {
            GameData = gameData;
        }

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            var data = JsonConvert.DeserializeObject(response) as JObject;
            var api_object = data["api_data"];
            switch (api)
            {
                //回港后重置所有
                case "api_port/port":
                    Reset(api_object["api_combined_flag"]==null?0:api_object["api_combined_flag"].ToObject<int>());
                    break;

                //点击出击按钮后，更新舰队信息
                case "api_req_map/start":
                case "api_req_map/next":
                    if (MainFleet == null)
                    {
                        int deckId = -1;
                        if (requestInfo.Data.ContainsKey("api_deck_id"))
                            deckId = int.Parse(requestInfo.Data["api_deck_id"]);
                        else if (requestInfo.Data.ContainsKey("api_dock_id"))
                            deckId = int.Parse(requestInfo.Data["api_dock_id"]);
                        int escortId = FleetType > 0 ? 2 : -1;// HACK: -1 for empty fleet.
                        var mapnext_data = api_object.ToObject<api_mapnext_data>();
                        bool isBoss = mapnext_data.api_event_id == 5;

                        MainFleet = GetFleet(deckId, 0);
                        EscortFleet = escortId == -1 ? null : GetFleet(escortId, 6);
                        SupportFleet = isBoss ? BossSF : NormalSF;
                    }
                    CurrentBattle = new Battle(MainFleet, EscortFleet, FleetType, SupportFleet);
                    break;

                //演习
                case "api_req_practice/battle":
                    FleetType = 0;
                    SupportFleet = null;
                    //未完成！！！
                    break;

                //战斗
                case "api_req_sortie/battle":
                    CurrentBattle.Simulate(api_object.ToObject<api_battle_data>());
                    try
                    {
                        OnBattleHappened?.InvokeAll(CurrentBattle);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("OnBattleHappened 错误！\n"+ex.Message +"\n"+ex.StackTrace);
                    }
                    break;

                //战斗结果
                case "api_req_sortie/battleresult":
                    try
                    {
                        OnBattleResultReceived?.InvokeAll(CurrentBattle, new BattleResult(api_object.ToObject<api_battleresult_data>()));
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("OnBattleResultReceived 错误！\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                    break;

            }
        }

        private void Reset(int fleetType)
        {
            MainFleet = null;
            EscortFleet = null;
            CurrentBattle = null;
            SupportFleet = null;
            FleetType = fleetType;
        }

        /// <summary>
        /// deckId从1到4
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="shipOwner"></param>
        /// <param name="fleetOffset">普通舰队为0， 联合舰队第一舰队为0，第二舰队为6</param>
        /// <returns></returns>
        private Fleet GetFleet(int deckId,int fleetOffset)
        {
            if (deckId <= 0)
                return null;
            KancolleShip[] ships = (from s in GameData.OwnedShipPlaceArray.ToArray(deckId - 1)
                                       select s < 0 ? null : GameData.OwnedShipDictionary[s]).ToArray();
            return new Fleet(ships, ShipOwner.Friend, fleetOffset);
        }
    }
}
