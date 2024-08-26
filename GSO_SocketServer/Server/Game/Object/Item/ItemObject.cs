using Google.Protobuf.Protocol;
using Server.Database.Data;
using Server.Database.Handler;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ItemObject : GameObject
    {
        private int itemId;
        private int x;
        private int y;
        private int rotate;
        private int amount;

        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject(int itemId, int gridX, int grixY, int rotate, int amount)
        {
            ObjectType = GameObjectType.Item;

            this.itemId = itemId;
            this.x = gridX;
            this.y = grixY;
            this.rotate = rotate;
            this.amount = amount;
        }

        public ItemObject(int viewerId, int itemId, int gridX, int grixY, int rotate, int amount) : this(itemId, gridX, grixY, rotate, amount)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow.AddSeconds(-Data.inquiry_time));
        }

        public ItemObject(ItemObject other) : this(other.itemId, other.x, other.y, other.rotate, other.amount)
        {
            this.Id = other.Id;
            this.viewers = other.viewers;
        }

        public void CreateItem()
        {
            ObjectManager.Instance.Add(this);
        }

        public void DestroyItem()
        {
            ObjectManager.Instance.Remove(this.Id);
        }

        public DB_ItemBase Data
        {
            get
            {
                return DatabaseHandler.Context.ItemBase.Get(ItemId);
            }
        }

        public int ItemId
        {
            get
            {
                return itemId;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                if (rotate % 2 == 0)
                {
                    return Data.scale_x;
                }
                return Data.scale_y;
            }
        }

        public int Height
        {
            get
            {
                if (rotate % 2 == 0)
                {
                    return Data.scale_y;
                }
                return Data.scale_x;
            }
        }

        public int Rotate
        {
            get => rotate;
            set
            {
                rotate = value;
            }
        }

        public double Weight
        {
            get
            {
                return Data.weight;
            }
        }

        public int Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }

        public int LimitAmount
        {
            get
            {
                return Data.stack_count;
            }
        }

        public bool IsViewer(int viewerId)
        {
            bool viewer = viewers.TryGetValue(viewerId, out var oldTime);
            if (false == viewer)
            {
                return false;
            }

            DateTime curTime = DateTime.UtcNow;
            TimeSpan elapsedTime = curTime - oldTime;
            return elapsedTime.TotalSeconds >= Data.inquiry_time;
        }

        public void AddViewer(int viewerId, int rtt)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow.AddMicroseconds(-rtt));
        }

        public DB_StorageUnit ConvertInventoryUnit()
        {
            DB_StorageUnit unit = new DB_StorageUnit();
            unit.item_id = ItemId;
            unit.grid_x = X;
            unit.grid_y = Y;
            unit.rotation = Rotate;
            unit.stack_count = Amount;
            return unit;
        }

        public PS_ItemInfo ConvertItemInfo(int viewerId)
        {
            PS_ItemInfo info = new PS_ItemInfo()
            {
                ObjectId = this.Id,
                ItemId = ItemId,
                X = X,
                Y = Y,
                Rotate = Rotate,
                Amount = Amount,
                IsSearched = IsViewer(viewerId),
            };
            return info;
        }

    }

}
