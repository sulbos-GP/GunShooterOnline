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
            storage.Init(5, 5, 20.0);

            DB_ItemBase colt45Data = DatabaseHandler.Context.ItemBase.Get(1);
            ItemObject colt45Item = new ItemObject(colt45Data.item_id, 0, 0, 0, 1);
            storage.InsertItem(colt45Item);

            DB_ItemBase akData = DatabaseHandler.Context.ItemBase.Get(2);
            ItemObject akItem = new ItemObject(akData.item_id, 3, 0, 1, 1);
            storage.InsertItem(akItem);

            DB_ItemBase bandData = DatabaseHandler.Context.ItemBase.Get(11);
            ItemObject bandItem = new ItemObject(bandData.item_id, 1, 3, 0, 10);
            storage.InsertItem(bandItem);
        }

    }
}
