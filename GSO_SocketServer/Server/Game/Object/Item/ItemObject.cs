using Google.Protobuf.Protocol;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ItemObject : GameObject
    {
        private PS_ItemInfo itemInfo = null;
        private readonly DB_ItemData itemData = new DB_ItemData();
        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject(int itemId, int gridX, int grixY, int rotate, int amount)
        {
            ObjectType = GameObjectType.Item;
            itemData = GetItemData(itemId).Result;

            ObjectManager.Instance.Add(this);

            itemInfo = new PS_ItemInfo()
            {
                ObjectId = this.Id,
                ItemId = itemId,
                X = gridX,
                Y = grixY,
                Rotate = rotate,
                Amount = amount
            };

        }

        public ItemObject(int viewerId, int itemId, int gridX, int grixY, int rotate, int amount) : this(itemId, gridX, grixY, rotate, amount)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow);
        }

        public ItemObject(ItemObject other)
        {
            ObjectType = GameObjectType.Item;
            ObjectManager.Instance.Add(this);
            viewers = other.viewers;
            itemInfo = other.itemInfo;
            itemData = other.itemData;
        }

        public async Task<DB_ItemData> GetItemData(int itemId)
        {
            //TODO : 나중에는 마스터테이블을 미리 로드하여 메모리에 저장된 값을 불러와야함
            return await DatabaseHandler.MasterDB.GetItemData(itemId);
        }

        public void DestroyItem()
        {
            ObjectManager.Instance.Remove(Id);
        }

        public PS_ItemInfo Info
        {
            get
            {
                return new PS_ItemInfo(itemInfo);
            }
        }

        public DB_ItemData Data
        {
            get
            {
                return itemData;
            }
        }

        public int ItemId
        {
            get
            {
                return itemInfo.ItemId;
            }
        }

        public int X
        {
            get
            {
                return itemInfo.X;
            }
            set
            {
                itemInfo.X = value;
            }
        }

        public int Y
        {
            get
            {
                return itemInfo.Y;
            }
            set
            {
                itemInfo.Y = value;
            }
        }

        public int Width
        {
            get
            {
                if (itemInfo.Rotate % 2 == 0)
                {
                    return itemData.scale_x;
                }
                return itemData.scale_y;
            }
        }

        public int Height
        {
            get
            {
                if (itemInfo.Rotate % 2 == 0)
                {
                    return itemData.scale_y;
                }
                return itemData.scale_x;
            }
        }

        public int Rotate
        {
            get => itemInfo.Rotate;
            set
            {
                itemInfo.Rotate = value;
            }
        }

        public int Amount
        {
            get
            {
                return itemInfo.Amount;
            }
            set
            {
                itemInfo.Amount = value;
            }
        }

        public int LimitAmount
        {
            get
            {
                return itemData.stack_count;
            }
        }

        public bool IsViewer(int viewerId)
        {
            return true;
            //bool viewer = viewers.TryGetValue(viewerId, out var oldTime);
            //if (false == viewer)
            //{
            //    return false;
            //}

            //DateTime curTime = DateTime.UtcNow;
            //TimeSpan elapsedTime = curTime - oldTime;
            //return elapsedTime.TotalSeconds >= itemData.inquiry_time;
        }

        public void AddViewer(int viewerId)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow);
        }

        public DB_InventoryUnit ConvertInventoryUnit()
        {
            DB_InventoryUnit unit = new DB_InventoryUnit();
            unit.item_id = ItemId;
            unit.grid_x = X;
            unit.grid_y = Y;
            unit.rotation = Rotate;
            unit.stack_count = Amount;
            return unit;
        }

    }

}
