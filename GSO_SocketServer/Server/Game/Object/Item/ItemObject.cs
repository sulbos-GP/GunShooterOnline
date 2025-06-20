﻿using Google.Protobuf.Protocol;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game
{
    public class ItemObject : GameObject, ICloneable
    {
        private DB_ItemUnit unit = new DB_ItemUnit();
        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject()
        {
            ObjectType = GameObjectType.Item;
        }

        public void Init(GameObject gameObject, DB_ItemUnit unit)
        {

            this.unit = unit;

            if (gameObject != null)
            {
                viewers.TryAdd(gameObject.Id, DateTime.UtcNow.AddSeconds(-Data.inquiry_time));
            }
        }

        public FMasterItemBase Data
        {
            get
            {
                return DatabaseHandler.Context.MasterItemBase.Find(ItemId);
            }
        }

        public DB_ItemUnit Unit
        {
            get
            {
                return new DB_ItemUnit()
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

        public int Loaded_ammo
        {
            get
            {
                return unit.attributes.loaded_ammo;
            }
            set
            {
                unit.attributes.loaded_ammo = value;
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
                return Data.amount;
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

            PS_ItemAttributes attributes = new PS_ItemAttributes()
            {
                Durability = Durability,
                LoadedAmmo = Loaded_ammo,
            };

            PS_ItemInfo info = new PS_ItemInfo()
            {
                ObjectId = this.Id,
                ItemId = ItemId,
                X = X,
                Y = Y,
                Rotate = Rotate,
                Amount = Amount,
                Attributes = attributes,
                IsSearched = IsViewer(viewerId),
            };
            return info;
        }

        public object Clone()
        {
            ItemObject itemObject = new ItemObject();
            itemObject.Id = Id;
            itemObject.unit = Unit;
            itemObject.viewers = this.viewers;
            return itemObject;
        }
    }

}
