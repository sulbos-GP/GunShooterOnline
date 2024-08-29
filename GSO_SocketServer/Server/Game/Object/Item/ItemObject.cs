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
        private DB_Unit unit = new DB_Unit();
        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject()
        {
            ObjectType = GameObjectType.Item;
        }

        public ItemObject(ItemObject other) : this(other.unit)
        {
            this.Id = other.Id;
            this.viewers = other.viewers;
        }

        public ItemObject(DB_Unit unit)
        {
            ObjectType = GameObjectType.Item;
            this.unit = unit;
        }

        public ItemObject(int viewerId, DB_Unit unit) : this(unit)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow.AddSeconds(-Data.inquiry_time));
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

        public DB_Unit Unit
        {
            get
            {
                return new DB_Unit()
                {
                    storage = StorageUnit,
                    attributes = Attributes,
                };
            }
        }

        public DB_StorageUnit StorageUnit
        {
            get
            {
                return new DB_StorageUnit()
                {
                    grid_x = unit.storage.grid_x,
                    grid_y = unit.storage.grid_y,
                    rotation = unit.storage.rotation,
                    unit_attributes_id = unit.storage.unit_attributes_id
                };
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
                return unit.attributes.item_id;
            }
        }

        public string Type
        {
            get
            {
                return Data.type;
            }
        }

        public int Durability
        {
            get
            {
                return unit.attributes.durability;
            }
        }

        public int? UnitStorageId
        {
            get
            {
                return unit.attributes.unit_storage_id;
            }
            set
            {
                unit.attributes.unit_storage_id = value;
            }
        }

        public int UnitAttributesId
        {
            get
            {
                return unit.storage.unit_attributes_id;
            }
            set
            {
                unit.storage.unit_attributes_id = value;
            }
        }

        public int X
        {
            get
            {
                return unit.storage.grid_x;
            }
            set
            {
                unit.storage.grid_x = value;
            }
        }

        public int Y
        {
            get
            {
                return unit.storage.grid_y;
            }
            set
            {
                unit.storage.grid_y = value;
            }
        }

        public int Width
        {
            get
            {
                if (unit.storage.rotation % 2 == 0)
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
                if (unit.storage.rotation % 2 == 0)
                {
                    return Data.scale_y;
                }
                return Data.scale_x;
            }
        }

        public int Rotate
        {
            get => unit.storage.rotation;
            set
            {
                unit.storage.rotation = value;
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
                return unit.attributes.amount;
            }
            set
            {
                unit.attributes.amount = value;
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
