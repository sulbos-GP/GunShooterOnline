using Google.Protobuf.Protocol;
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
        
        private int x;
        private int y;
        private int rotate;
        private int amount;

        private readonly DB_ItemData itemData = new DB_ItemData();
        private Dictionary<int, DateTime> viewers = new Dictionary<int, DateTime>();

        public ItemObject(int itemId, int gridX, int grixY, int rotate, int amount)
        {
            ObjectType = GameObjectType.Item;
            itemData = GetItemData(itemId).Result;

            this.x = gridX;
            this.y = grixY;
            this.rotate = rotate;
            this.amount = amount;
        }

        public ItemObject(int viewerId, int itemId, int gridX, int grixY, int rotate, int amount) : this(itemId, gridX, grixY, rotate, amount)
        {
            viewers.TryAdd(viewerId, DateTime.UtcNow.AddSeconds(-Data.inquiry_time));
        }

        public ItemObject(ItemObject other)
        {
            this.ObjectType = GameObjectType.Item;
            this.Id = other.Id;
            this.viewers = other.viewers;
            this.x = other.x;
            this.y = other.y;
            this.rotate = other.rotate;
            this.amount = other.amount;
            this.itemData = other.itemData;
        }

        public async Task<DB_ItemData> GetItemData(int itemId)
        {
            //TODO : 나중에는 마스터테이블을 미리 로드하여 메모리에 저장된 값을 불러와야함
            return await DatabaseHandler.MasterDB.GetItemData(itemId);
        }

        public void CreateItem()
        {
            ObjectManager.Instance.Add(this);
        }

        public void DestroyItem()
        {
            ObjectManager.Instance.Remove(this.Id);
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
                return Data.item_id;
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

        public PS_ItemInfo ConvertItemInfo(int viewerId)
        {
            PS_ItemInfo info = new PS_ItemInfo()
            {
                ObjectId = this.Id,
                ItemId = Data.item_id,
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
