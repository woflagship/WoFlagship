using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class Ship
    {
        /// <summary>
        /// 舰娘Id
        /// </summary>
        public int ShipId { get; private set; }
        public ShipOwner ShipOwner { get; private set; }
        public int Position { get; private set; }
        public int MaxHP { get; private set; }
        public int NowHP { get; private set; }
        public int FromHP { get; private set; }
        public int ToHP { get; private set; }
        public ReadOnlyCollection<int> Items { get; private set; }
        public int ItemUsed { get; private set; } = -1;
        public KancolleShip Raw { get; private set; }

        /// <summary>
        /// 用于创建从KancolleShip而来的舰娘
        /// </summary>
        /// <param name="rawShip"></param>
        /// <param name="shipOwner"></param>
        /// <param name="position"></param>
        public Ship(KancolleShip rawShip, ShipOwner shipOwner, int position)
        {
            Raw = rawShip;
            ShipId = rawShip.ShipId;
            ShipOwner = shipOwner;
            Position = position;
            MaxHP = rawShip.MaxHP;
            NowHP = rawShip.NowHP;
            FromHP = NowHP;
            ToHP = NowHP;
            Items = new ReadOnlyCollection<int>(rawShip.Slot);
            ItemUsed = -1;
            
        }

        /// <summary>
        /// 用于创建从api而来的深海舰娘
        /// </summary>
        public Ship(int shipId, int maxHp, int nowHp, int[] slots, ShipOwner shipOwner, int position)
        {
            ShipId = shipId;
            ShipOwner = shipOwner;
            Position = position;
            MaxHP = maxHp;
            NowHP = nowHp;
            Items = new ReadOnlyCollection<int>(slots);
        }

        private int UseItem()
        {
            if (ShipOwner == ShipOwner.Friend && NowHP <= 0 && Items != null)
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

        internal void Damage(int damage)
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
