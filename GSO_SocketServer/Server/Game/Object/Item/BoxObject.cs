using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Database.Handler;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

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

        public void Init()
        {
            //임의의 아이템
            storage.Init((int)info.Box.X, (int)info.Box.Y, info.Box.Weight);

            DB_ItemBase colt45Data = DatabaseHandler.Context.ItemBase.Get(101);
            DB_ItemUnit colt45 = new DB_ItemUnit()
            {
                storage = new DB_StorageUnit()
                {
                    grid_x = 0,
                    grid_y = 0,
                    rotation = 0,
                    unit_attributes_id = 0
                },

                attributes = new DB_UnitAttributes()
                {
                    item_id = colt45Data.item_id,
                    durability = 0,
                    unit_storage_id = null,
                    amount = 1
                }
            };
            ItemObject colt45Item = new ItemObject(colt45);
            storage.InsertItem(colt45Item);

            //
            DB_ItemBase akData = DatabaseHandler.Context.ItemBase.Get(102);
            DB_ItemUnit ak47 = new DB_ItemUnit()
            {
                storage = new DB_StorageUnit()
                {
                    grid_x = 3,
                    grid_y = 0,
                    rotation = 1,
                    unit_attributes_id = 0
                },

                attributes = new DB_UnitAttributes()
                {
                    item_id = akData.item_id,
                    durability = 0,
                    unit_storage_id = null,
                    amount = 1
                }
            };
            ItemObject akItem = new ItemObject(ak47);
            storage.InsertItem(akItem);

            //
            DB_ItemBase bandData = DatabaseHandler.Context.ItemBase.Get(402);
            for(int i = 0; i < 5; ++i)
            {
                DB_ItemUnit band = new DB_ItemUnit()
                {
                    storage = new DB_StorageUnit()
                    {
                        grid_x = i,
                        grid_y = 4,
                        rotation = 0,
                        unit_attributes_id = 0
                    },

                    attributes = new DB_UnitAttributes()
                    {
                        item_id = bandData.item_id,
                        durability = 0,
                        unit_storage_id = null,
                        amount = 1 + (i * 5)
                    }
                };
                ItemObject bandItem = new ItemObject(band);
                storage.InsertItem(bandItem);
            }

        }

    }
}
