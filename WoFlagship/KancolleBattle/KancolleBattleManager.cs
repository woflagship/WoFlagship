using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleBattle
{
    public class KancolleBattleManager : IKancolleAPIReceiver, IKancolleGameDataReceiver
    {
        public event Action<KancolleBattle> OnBattleHappened;


        public int FleetType { get; private set; }
        public Fleet SupportFleet { get; private set; } = null;
        public Fleet NormalSF { get; private set; } = null;
        public Fleet BossSF { get; private set; } = null;
        public KancolleBattle Battle { get; private set; } = null;

        private KancolleGameData gameData = null;

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            dynamic body = JsonConvert.DeserializeObject<svdata>(response).api_data;
            // Support fleet
            // NOTICE: We didn't check support fleet map.
            if (api == "api_port/port")
            {
                //support fleet!!!
            }
            else if (api == "api_req_mission/start")
            {
                //req_mission
            }
            else if(api == "api_get_member/mapinfo")
            {
                // Land Base Air Corps
            }
            else if(api == "api_req_air_corps/supply" || api == "api_req_air_corps/set_plane")
            {

            }
            else if(api == "api_req_air_corps/set_action")
            {
            }
            else if(api == "api_req_member/get_practice_enemyinfo")
            {
                // Pratice Enemy Information
            }
            else if(api == "api_req_map/start")
            {
                // Oh fuck. Someone sorties with No.3/4 fleet when having combined fleet.
                if (FleetType != 0 && int.Parse(requestInfo.Data["api_deck_id"]) != -1)
                    FleetType = 0;
            }

            //Reset all
            if (api == "api_port/port")
            {
                // `api_combined_flag` is only available during event.
                // We assume it's 0 (normal fleet) because we can't combine fleet at peacetime.
                if (body["api_combined_flag"] == null)
                    Reset(0);
                else
                    Reset(body["api_combined_flag"].ToObject<int>());
               
                return;
            }

            // Enter sortie battle
            if(api== "api_req_map/start" || api== "api_req_map/next")
            {
                var _body = body.ToObject<api_mapnext_data>();
                bool isBoss = _body.api_event_id == 5;
                Battle = new KancolleBattle()
                {
                    BattleType = isBoss ? BattleTypes.Boss : BattleTypes.Normal,
                    Map = new int[] { _body.api_maparea_id, _body.api_mapinfo_no, _body.api_no },
                    /*
                        desc:   null,
                        time:   null,  // Assign later
                        fleet:  null,  // Assign later
                        packet: [],
                    */
                };
                SupportFleet = isBoss ? BossSF : NormalSF;
                return;
            }

            // Enter pratice battle
            if(api == "api_req_practice/battle")
            {
                Battle = new KancolleBattle()
                {
                    BattleType = BattleTypes.Practice,
                    Map = new int[] { },
                    /*
                     desc:   null,
                     time:   null,  // Assign later
                     fleet:  null,  // Assign later
                     packet: [],
                 */

                };
                FleetType = 0;
                SupportFleet = null;
                //+this.landBaseAirCorps = null
                // No `return`
            }

            //Process packet in battle
            if(Battle != null)
            {
                if(api == "api_req_map/start_air_base")
                {
                    //!!!!!!!!!!
                    return;
                }

                Battle.EnemyType = (api.Contains("ec_") || api.Contains("each_")) ? 1 : 0;

                /*
                 let packet = Object.clone(body)
      packet.poi_path = req.path
      packet.poi_time = timestamp

      if (!this.battle.time) {
        this.battle.time = packet.poi_time
      }
                */
                if(Battle.mainFleet == null)
                {
                    int fleetId = (int)(new int?[] { body["api_deck_id"]?.ToObject<int?>(), body["api_dock_id"]?.ToObject<int?>() }.First((x) => x != null));
                    int escortId = FleetType > 0 ? 2 : -1;// HACK: -1 for empty fleet.
                    Battle.FleetType = FleetType;
                    Battle.mainFleet = new Fleet(getRawFleet(fleetId), 0);//KancolleBattle.initFleet( getRawFleet(fleetId), 0);
                    Battle.escortFleet = escortId == -1 ? null : new Fleet(getRawFleet(escortId), 6);//KancolleBattle.initFleet(getRawFleet(escortId), 6);
                    Battle.SupportFleet = SupportFleet;
                    //Battle.LBAC
                }

                if (api.Contains("result"))
                {

                }
                else if(api.Contains("battle"))
                {
                    api_battle_data packet = body.ToObject<api_battle_data>();
                    Battle.Simulate(packet);
                    OnBattleHappened?.Invoke(Battle);
                }
            }
        }

        /// <summary>
        ///  deckId in [1, 2, 3, 4]
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns></returns>
        private KancolleShip[] getRawFleet(int deckId)
        {
            List<KancolleShip> fleet = new List<KancolleShip>();
            for(int i=0; i<gameData.OwnedShipPlaceArray.GetLength(1); i++)
            {
                int ownedShipId = gameData.OwnedShipPlaceArray[deckId - 1, i];
                if (ownedShipId > 0)
                {
                    fleet.Add(gameData.OwnedShipDictionary[ownedShipId]);
                }
                else
                    fleet.Add(null);
            }

            return fleet.ToArray();
        }

       

        private api_slot_item_item getItem(int itemId)
        {
            api_slot_item_item item = null;

            return item;
        }

        private void Reset(int fleetType)
        {
            FleetType = fleetType;
            Battle = null;
            SupportFleet = null;
            //landBaseAirCorps=null;
            //praticeOpponent=null
        }

        public void OnGameDataUpdatedHandler(KancolleGameData gameData)
        {
            this.gameData = gameData;
        }
    }
}
