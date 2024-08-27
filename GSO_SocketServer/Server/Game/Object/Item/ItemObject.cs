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
        private DB_UnitAttributes attributes = new DB_UnitAttributes();
        private int x;
        private int y;
        private int rotate;

        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject()
        {
            ObjectType = GameObjectType.Item;
            x = 0;
            y = 0;
            rotate = 0;
        }

        public ItemObject(int gridX, int grixY, int rotate, DB_UnitAttributes attributes)
        {
            ObjectType = GameObjectType.Item;
            
            this.attributes = attributes;

            this.x = gridX;
            this.y = grixY;
            this.rotate = rotate;

        }

        public ItemObject(int viewerId, int gridX, int grixY, int rotate, DB_UnitAttributes attributes) : this(gridX, grixY, rotate, attributes)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow.AddSeconds(-Data.inquiry_time));
        }

        public ItemObject(ItemObject other) : this(other.x, other.y, other.rotate, other.attributes)
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

        public DB_UnitAttributes Attributes
        {
            get
            {
                return new DB_UnitAttributes()
                {
                    item_id = ItemId,
                    durability = Durability,
                    unit_storage_id = UnitStorageId,
                    amount = Amount
                };
            }
        }

        public int ItemId
        {
            get
            {
                return attributes.item_id;
            }
        }

        public int Durability
        {
            get
            {
                return attributes.durability;
            }
        }

        public int? UnitStorageId
        {
            get
            {
                return attributes.unit_storage_id;
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
                return attributes.amount;
            }
            set
            {
                attributes.amount = value;
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
            unit.grid_x = X;
            unit.grid_y = Y;
            unit.rotation = Rotate;

            unit.attributes.item_id = ItemId;
            unit.attributes.durability = attributes.durability;
            unit.attributes.unit_storage_id = attributes.unit_storage_id;
            unit.attributes.amount = Amount;
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
