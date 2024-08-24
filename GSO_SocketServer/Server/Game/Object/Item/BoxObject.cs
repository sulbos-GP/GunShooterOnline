using Collision.Shapes;
using Google.Protobuf.Protocol;
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

        public async void Init()
        {
            //아이템 고정
            storage.Init(5, 5, 20.0);

            DB_ItemData data = await DatabaseHandler.MasterDB.GetItemData(1);

            ItemObject item = new ItemObject(data.item_id, 0, 0, 0, 1);
            storage.InsertItem(item);

            Console.WriteLine();
        }

        public IEnumerable<PS_ItemInfo> GetBoxItems()
        {
            List<PS_ItemInfo> infos = new List<PS_ItemInfo>();
            foreach (ItemObject item in storage.items)
            {
                infos.Add(item.Info);
            }
            return infos;
        }

    }
}
