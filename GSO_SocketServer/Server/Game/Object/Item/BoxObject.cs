using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Database.Data;
using Server.Database.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            DB_ItemBase colt45Data = DatabaseHandler.Context.ItemBase.Get(1);
            DB_UnitAttributes colt45Att = new DB_UnitAttributes()
            {
                item_id = colt45Data.item_id,
                durability = 0,
                unit_storage_id = null,
                amount = 1
            };

            ItemObject colt45Item = new ItemObject(0, 0, 0, colt45Att);
            storage.InsertItem(colt45Item);

            DB_ItemBase akData = DatabaseHandler.Context.ItemBase.Get(2);
            DB_UnitAttributes akAtt = new DB_UnitAttributes()
            {
                item_id = akData.item_id,
                durability = 0,
                unit_storage_id = null,
                amount = 1
            };

            ItemObject akItem = new ItemObject(3, 0, 1, akAtt);
            storage.InsertItem(akItem);

            DB_ItemBase bandData = DatabaseHandler.Context.ItemBase.Get(11);
            for(int i = 0; i < 5; ++i)
            {
                DB_UnitAttributes bandAtt = new DB_UnitAttributes()
                {
                    item_id = bandData.item_id,
                    durability = 0,
                    unit_storage_id = null,
                    amount = 1 + (i * 5)
                };

                ItemObject bandItem = new ItemObject(i, 4, 0, bandAtt);
                storage.InsertItem(bandItem);
            }

        }

    }
}
