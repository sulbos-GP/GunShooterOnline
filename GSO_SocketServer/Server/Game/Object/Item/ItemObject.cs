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
        private PS_ItemInfo itemInfo = new PS_ItemInfo();
        private readonly DB_ItemData itemData = new DB_ItemData();

        public ItemObject(int itemId)
        {
            ObjectType = GameObjectType.Item;
            itemData = GetItemData(itemId).Result;
        }

        public async Task<DB_ItemData> GetItemData(int itemId)
        {
            //TODO : 나중에는 마스터테이블을 미리 로드하여 메모리에 저장된 값을 불러와야함
            return await DatabaseHandler.MasterDB.GetItemData(itemId);
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

        public bool CompInfo(PS_ItemInfo otherInfo)
        {
            if (otherInfo == null)
            {
                return false;
            }

            return this.itemInfo == otherInfo;
        }

        public void DestroyItem()
        {
            ObjectManager.Instance.Remove(Id);
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
