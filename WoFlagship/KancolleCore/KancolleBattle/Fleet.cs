using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    /// <summary>
    /// 战斗舰队，必然包含6个Ship，如果某个位置没有舰娘，则该位置设为null
    /// </summary>
    public class Fleet : IReadOnlyCollection<Ship>
    {

        //public int CombinedFlag { get; set; }
        private List<Ship> ships = new List<Ship>();

        public int Count
        {
            get
            {
                return ships.Count;
            }
        }


        public Ship this[int index]
        {
            get
            {
                return ships[index];
            }
        }

        private Fleet() { }

        /// <summary>
        /// 从KancolleShip构造舰队
        /// </summary>
        /// <param name="rawFleet"></param>
        /// <param name="shipOwner"></param>
        /// <param name="fleetOffset">如果存在联合舰队，第一舰队为0，第二舰队为6；默认状况下为0</param>
        public Fleet(KancolleShip[] rawFleet, ShipOwner shipOwner, int fleetOffset = 0)
        {
            if (rawFleet == null)
                return;
          
            for(int i=0; i<rawFleet.Length; i++)
            {
                var rawShip = rawFleet[i];
                if(rawShip != null)
                {
                    ships.Add(new Ship(rawShip, ShipOwner.Friend, fleetOffset + i + 1));
                }
                else
                {
                    ships.Add(null);
                }
            }
        
        }

        /// <summary>
        /// 直接从api构造舰队
        /// </summary>
        /// <param name="api_ship_ke"></param>
        /// <param name="api_eSlot"></param>
        /// <param name="api_maxhps"></param>
        /// <param name="api_nowhps"></param>
        /// <param name="api_ship_lv"></param>
        /// <param name="shipOwner"></param>
        /// <param name="fleetOffset">如果存在联合舰队，第一舰队为0，第二舰队为6；默认状况下为0</param>
        public Fleet(JArray api_ship_ke, int[][] api_eSlot, int[] api_maxhps, int[] api_nowhps, int[] api_ship_lv, ShipOwner shipOwner, int fleetOffset = 0)
        {
            if (api_ship_ke == null)
                return;
            
            for(int i=1; i<7; i++)
            {
                var id = api_ship_ke[i];
                var slots = api_eSlot[i - 1];
                Ship ship = null;
                if(id.Type == JTokenType.Integer)
                {
                    ship = new Ship(id.ToObject<int>(), api_maxhps[i + 6], api_nowhps[i + 6], slots, shipOwner, fleetOffset + i);
                }
                ships.Add(ship);
            }
        }

        public IEnumerator<Ship> GetEnumerator()
        {
            return ships.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ships.GetEnumerator();
        }
    }
}
