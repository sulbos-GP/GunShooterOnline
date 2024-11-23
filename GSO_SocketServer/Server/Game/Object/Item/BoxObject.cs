using Collision.Shapes;
using Google.Protobuf.Protocol;
using Humanizer;
using Server.Database.Handler;
using System;
using System.Numerics;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game.Object.Item
{
    public class BoxObject : GameObject
    {
        public bool isOpen = false;
        public Storage storage = new Storage();

        public BoxObject()
        {

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


            info.Name = "Box" + Id;

            //임의의 아이템
            storage.Init((int)info.Box.X, (int)info.Box.Y, info.Box.Weight);

            SetItem(501);
            CellPos = pos;



        }

        public void SetStorage(Storage storage)
        {
            info.Name = "Box" + Id;

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
            info.Name = "Box" + Id;

            info.Box = new BoxInfo()
            {
                X = itemObject.Width,
                Y = itemObject.Height,
                Weight = (float)itemObject.Weight + 1.0f,
            };

            this.storage.Init((int)info.Box.X, (int)info.Box.Y, info.Box.Weight * itemObject.Amount);
            this.storage.InsertItem(itemObject);
        }

        public bool SetItem(int id)
        {
            FMasterItemBase data = DatabaseHandler.Context.MasterItemBase.Find(id);
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
                    amount = 10,
                }
            };
            ItemObject newItem = new ItemObject(item);
            EStorageError error = storage.InsertItem(newItem);

            return error == EStorageError.None ? true : false;
        }

        public void SetRandomItem()
        {

            Random rand = new Random();
            int number = rand.Next(1, 6);

            switch (number)
            {
                case 1:
                    RandomItem(101, 103);
                    break;
                case 2:
                    RandomItem(201, 203);
                    break;
                case 3:
                    RandomItem(300, 303);
                    break;
                case 4:
                    RandomItem(401, 404);
                    break;
                case 5:
                    RandomItem(501, 502);
                    break;
                case 6:
                    RandomItem(601, 606);
                    break;
            }
        }

        private void RandomItem(int min, int max)
        {
            Random rand = new Random();
            int item_id = rand.Next(min, max);
            FMasterItemBase data = DatabaseHandler.Context.MasterItemBase.Find(item_id);
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
                    amount = 1
                }
            };
            ItemObject newItem = new ItemObject(item);
            SetItemObject(newItem);
        }

    }
}
