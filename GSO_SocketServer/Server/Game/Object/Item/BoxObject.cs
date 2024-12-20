using Collision.Shapes;
using Google.Protobuf.Protocol;
using Humanizer;
using Server.Database.Handler;
using Server.Database.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;
using static Humanizer.In;
using static Humanizer.On;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game.Object.Item
{
    public enum EBoxSize
    {
        Small,
        Medium,
        Large,
    }

    public class BoxObject : GameObject
    {
        public bool isOpen = false;
        public Storage storage = new Storage();

        public BoxObject()
        {
            info.Name = "Box" + Id;

            ObjectType = GameObjectType.Box;

            float width = 1;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;

            currentShape = rectangle;

            info.Box = new BoxInfo()
            {
                X = 6,
                Y = 6,
                Weight = 50,
            };

        }

        public void Open()
        {
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        public bool IsOpen()
        {
            return isOpen;
        }

        public void Init(Vector2 pos, bool isAdvancedBox)
        {
            if(isAdvancedBox)
            {
                SetBox(pos, EBoxSize.Large);
                AddItem("군용더블백", 1);
                AddItem("방탄조끼", 1);
                AddItem("금괴", 1);
                AddItem("의약품상자", 1);
                AddItem("아드레날린", 1);
                FitBox();
            }
            else
            {
                SetBox(pos, EBoxSize.Large);
                AddRandgeItem(EItemType.Spoil, 3, 5);
                FitBox();
            }
        }

        public void SetBox(Vector2 pos, EBoxSize size)
        {
            SetStorageAndBoxInfo(size);
            CellPos = pos;
        }

        public void SetBox(Vector2 pos, int x, int y, double weight)
        {
            SetStorageAndBoxInfo(x, y, weight);
            CellPos = pos;
        }

        public void SetStorage(Vector2 pos, Storage storage)
        {
            SetBox(pos, storage.Scale_X, storage.Scale_Y, storage.MaxWeight);
            this.storage = storage;
        }

        public bool AddItem(string name, int amount)
        {
            return AddItem(InstanceItemUnit(GetItemDataWithName(name), amount));
        }

        public bool AddItem(int id, int amount)
        {
            return AddItem(InstanceItemUnit(GetItemDataWithId(id), amount));
        }

        public bool AddItem(EItemType type, int amount)
        {
            return AddItem(InstanceItemUnit(GetItemDataWithType(type), amount));
        }

        public void AddRandgeItem(EItemType type, int min, int max, int retry = 10)
        {
            if(max < min)
            {
                min = max;
            }

            int maxRetry = retry;
            int curRetry = 0;

            Random rand = new Random();
            int maxCount = rand.Next(min, max);
            int curCount = 0;
            while (curCount < maxCount)
            {
                FMasterItemBase data = GetItemDataWithType(type);
                int amount = rand.Next(1, data.amount);

                if(true == AddItem(InstanceItemUnit(data, amount)))
                {
                    curCount++;
                }
                else
                {
                    if (curRetry > maxRetry)
                    {
                        break;
                    }
                    curRetry++;
                }
            }
        }

        public bool AddItem(ItemObject item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.LimitAmount < item.Amount)
            {
                return false;
            }

            if (false == PlaceItem(item))
            {
                ObjectManager.Instance.Remove(item.Id);
                return false;
            }

            return true;
        }

        private void SetStorageAndBoxInfo(int x, int y, double weight)
        {
            info.Box = new BoxInfo()
            {
                X = x,
                Y = y,
                Weight = (float)Math.Ceiling(weight),
            };

            this.storage.Init(x, y, weight);
        }

        private void SetStorageAndBoxInfo(EBoxSize boxSize)
        {
            var (boxX, boxY, boxWeight) = GetBoxSize(boxSize);
            SetStorageAndBoxInfo(boxX, boxY, (float)boxWeight);
        }

        public void FitBox()
        {
            this.storage.Fit();

            info.Box = new BoxInfo()
            {
                X = this.storage.Scale_X,
                Y = this.storage.Scale_Y,
                Weight = (float)Math.Ceiling(this.storage.MaxWeight),
            };
        }

        private bool PlaceItem(ItemObject item)
        {
            for(int gridY = 0; gridY < this.storage.Scale_Y; ++gridY)
            {
                for(int gridX = 0; gridX < this.storage.Scale_X; ++gridX)
                {
                    for(int r = 0; r <= 1; ++r)
                    {
                        item.X = gridX;
                        item.Y = gridY;
                        item.Rotate = r;

                        if(true == this.storage.InsertItem(item))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private (int, int, double) GetBoxSize(EBoxSize size)
        {
            int x = 0;
            int y = 0;
            double weight = 0.0;

            if(size == EBoxSize.Small)
            {
                x = 3;
                y = 4;
                weight = 15;
            }
            else if(size == EBoxSize.Medium)
            {
                x = 5;
                y = 5;
                weight = 20;
            }
            else if(size == EBoxSize.Large)
            {
                x = 5;
                y = 7;
                weight = 40;
            }

            return (x, y, weight);
        }

        private FMasterItemBase GetRandomItemDataWithScopeType(EItemType min, EItemType max)
        {
            Random rand = new Random();
            int range = rand.Next((int)min, (int)max);

            return GetItemDataWithType((EItemType)range);
        }

        private FMasterItemBase GetItemDataWithType(EItemType type)
        {
            Random rand = new Random();
            int range = 100 * (int)type;

            var items = DatabaseHandler.Context.MasterItemBase
                .Where(item => item.Value.item_id >= range && item.Value.item_id < range + 100)
                .ToDictionary();

            int item_id = rand.Next(items.Keys.Min(), items.Keys.Max());
            FMasterItemBase data = DatabaseHandler.Context.MasterItemBase.Find(item_id);

            return data;
        }

        private ItemObject InstanceItemUnit(FMasterItemBase data, int amount)
        {
            DB_ItemUnit item = new DB_ItemUnit()
            {
                storage = new DB_StorageUnit()
                {
                    grid_x = 0,
                    grid_y = 0,
                    rotation = 1,
                    unit_attributes_id = 0
                },

                attributes = new DB_UnitAttributes()
                {
                    item_id = data.item_id,
                    durability = 0,
                    unit_storage_id = null,
                    amount = amount
                }
            };

            ItemObject newItem = ObjectManager.Instance.Add<ItemObject>();
            newItem.Init(null, item);

            return newItem;
        }

        private FMasterItemBase GetItemDataWithName(string name)
        {
            return DatabaseHandler.Context.MasterItemBase.FirstOrDefault(item => item.Value.name == name).Value;
        }

        private FMasterItemBase GetItemDataWithId(int id)
        {
            return DatabaseHandler.Context.MasterItemBase.FirstOrDefault(item => item.Value.item_id == id).Value;
        }

    }
}
