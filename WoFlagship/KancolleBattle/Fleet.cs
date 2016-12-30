using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleBattle
{
    public class Fleet : IList<Ship>
    {

        //public int CombinedFlag { get; set; }
        private List<Ship> mainShips  = new List<Ship>();

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public Ship this[int index]
        {
            get
            {
                return mainShips[index];
            }
            set
            {
                mainShips[index] = value;
            }
        }

        public Fleet() { }
        public Fleet(api_ship_item[] rawFleet, int intl, ShipOwners owner = ShipOwners.Friend)
        {
            if (rawFleet == null) return;
            for (int i = 0; i < rawFleet.Length; i++)
            {
                var rawShip = rawFleet[i];
                if (rawShip != null)
                {
                    //slots!!!!!!!
                    Add(new Ship()
                    {
                        ShipId = rawShip.api_ship_id,
                        ShipOwner = owner,
                        Position = intl + i + 1,
                        MaxHP = rawShip.api_maxhp,
                        NowHP = rawShip.api_nowhp,
                        Items = null,//!!!!!!!!
                        Raw = rawShip
                    });
                }
                else
                {
                    Add(null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intl"></param>
        /// <param name="api_ship_ke">长度为7</param>
        /// <param name="api_eSlot">长度为7*5？</param>
        /// <param name="api_maxhps">长度为13</param>
        /// <param name="api_nowhps">长度为13</param>
        /// <param name="api_ship_lv">长度为7</param>
        public Fleet(int intl, dynamic[] api_ship_ke, int[][] api_eSlot, int[] api_maxhps, int[] api_nowhps, int[] api_ship_lv, ShipOwners owner = ShipOwners.Enemy)
        {
            if (api_ship_ke == null)
                return;
            for (int i = 1; i < 7; i++)
            {
                dynamic id = api_ship_ke[i];
                var slots = api_eSlot[i-1];
                Ship ship = null;//raw???
                if (!(id is string) &&  id > 0)
                {
                    /*
                    if (this.usePoiAPI)
                      raw = {
                        api_ship_id: id,
                        api_lv: api_ship_lv[i],
                        poi_slot: slots.map(id => window.$slotitems[id]),
                      }
                    */
                    ship = new Ship()
                    {
                        ShipId = (int)id,
                        ShipOwner = owner,
                        Position = intl + i,
                        MaxHP = api_maxhps[i + 6],
                        NowHP = api_nowhps[i + 6],
                        Items = new int[] { },//we don't care
                        Raw = null//raw!!!!!!!
                    };
                }
                Add(ship);
            }
        }

        public void Add(Ship ship)
        {
            mainShips.Add(ship);
        }

        public int IndexOf(Ship ship)
        {
            return mainShips.IndexOf(ship);
        }

        public void Insert(int index, Ship ship)
        {
            mainShips.Insert(index, ship);
        }

        public void RemoveAt(int index)
        {
            mainShips.RemoveAt(index);
        }

        public void Clear()
        {
            mainShips.Clear();
        }

        public bool Contains(Ship item)
        {
            return mainShips.Contains(item);
        }

        public void CopyTo(Ship[] array, int arrayIndex)
        {
            mainShips.CopyTo(array, arrayIndex);
        }

        public bool Remove(Ship item)
        {
            return mainShips.Remove(item);
        }

        public IEnumerator<Ship> GetEnumerator()
        {
            return mainShips.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mainShips.GetEnumerator();
        }
    }
}
