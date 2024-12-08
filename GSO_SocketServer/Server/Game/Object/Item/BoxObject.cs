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
        Large
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

        public void Init(Vector2 pos)
        {

            //SetAdvancedBox();

            SetRandomItem(1, 5, EBoxSize.Medium);

            CellPos = pos;
        }

        public void SetStorage(Storage storage)
        {
            info.Box = new BoxInfo()
            {
                X = storage.Scale_X,
                Y = storage.Scale_Y,
                Weight = (float)storage.MaxWeight,
            };

            this.storage.Init((int)info.Box.X, (int)info.Box.Y, info.Box.Weight);
            this.storage = storage;
        }

        public void SetItemObject(ItemObject itemObject)
        {
            info.Box = new BoxInfo()
            {
                X = itemObject.Width,
                Y = itemObject.Height,
                Weight = (float)itemObject.Weight + 1.0f,
            };

            this.storage.Init((int)info.Box.X, (int)info.Box.Y, info.Box.Weight * itemObject.Amount);
            this.storage.InsertItem(itemObject);
        }

        //기획상 고정 아이템
        public void SetAdvancedBox()
        {
            var (boxX, boxY, boxWeight) = GetBoxSize(EBoxSize.Medium);
            this.storage.Init(boxX, boxY, boxWeight);

            ItemObject backpack = InstanceItemUnit(GetItemDataWithName("군용더블백"), 1);
            ObjectManager.Instance.Add(backpack);
            PlaceItem(backpack);

            ItemObject armor = InstanceItemUnit(GetItemDataWithName("방탄조끼"), 1);
            ObjectManager.Instance.Add(armor);
            PlaceItem(armor);

            ItemObject medicine = InstanceItemUnit(GetItemDataWithName("의약품상자"), 2);
            ObjectManager.Instance.Add(medicine);
            PlaceItem(medicine);

            ItemObject adrenaline = InstanceItemUnit(GetItemDataWithName("아드레날린"), 2);
            ObjectManager.Instance.Add(adrenaline);
            PlaceItem(adrenaline);

            ItemObject gold = InstanceItemUnit(GetItemDataWithName("금괴"), 1);
            ObjectManager.Instance.Add(gold);
            PlaceItem(gold);


        }

        //랜덤 일반 상자
        public void SetRandomItem(int min, int max, EBoxSize boxSize)
        {

            var (boxX, boxY, boxWeight) = GetBoxSize(boxSize);
            this.storage.Init(boxX, boxY, boxWeight);

            const int MaxRetry = 10;
            int retry = 0;

            Random rand = new Random();
            int maxCount = rand.Next(min, max);
            int count = 0;
            while(count < maxCount)
            {
                FMasterItemBase data = GetItemDataWithType(EItemType.Spoil);
                ItemObject newItem = InstanceItemUnit(data, 1);

                if(true == PlaceItem(newItem))
                {
                    ObjectManager.Instance.Add(newItem);
                    count++;
                }
                else
                {
                    if(retry > MaxRetry)
                    {
                        break;
                    }
                    retry++;
                }

            }
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

        private FMasterItemBase GetRandomItemDataWithTypes(EItemType min, EItemType max)
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

            ItemObject newItem = new ItemObject();
            newItem.Init(null, item);

            return newItem;
        }

        private FMasterItemBase GetItemDataWithName(string name)
        {
            return DatabaseHandler.Context.MasterItemBase.FirstOrDefault(item => item.Value.name == name).Value;
        }

    }
}
