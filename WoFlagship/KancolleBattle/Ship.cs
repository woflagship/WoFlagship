using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCommon;

namespace WoFlagship.KancolleBattle
{
    public enum ShipOwners
    {
        Friend,
        Enemy
    }

    public class Ship
    {
        public int ShipId { get; set; }
        public ShipOwners ShipOwner { get; set; }
        public int Position { get; set; }
        public int MaxHP { get; set; }
        public int NowHP { get; set; }
        public int FromHP { get; set; }
        public int ToHP { get; set; }
        public int[] Items { get; set; }
        public int ItemUsed { get; set; } = -1;
        public api_ship_item Raw { get; set; }

        public int UseItem()
        {
            if (ShipOwner == ShipOwners.Friend && NowHP <= 0 && Items != null)
            {
                foreach (var itemId in Items)
                {
                    // 応急修理要員
                    if (itemId == 42)
                    {
                        NowHP = (int)Math.Floor(MaxHP * 1.0 / 5);
                        return itemId;
                    }
                    // 応急修理女神
                    if (itemId == 43)
                    {
                        NowHP = MaxHP;
                        return itemId;
                    }
                }
            }
            return -1;
        }

        public void Damage(int damage)
        {
            FromHP = NowHP;
            NowHP -= damage;
           
            var item = UseItem();
            if (item > 0)
                ItemUsed = item;

            ToHP = NowHP;
        }
    }
}
