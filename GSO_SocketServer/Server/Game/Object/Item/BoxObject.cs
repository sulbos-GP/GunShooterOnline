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

        public async void Init()
        {

            DB_ItemData data = await DatabaseHandler.MasterDB.GetItemData(1);

            ItemObject item = new ItemObject(data.item_id);
            item.X = 0;
            item.Y = 0;
            item.Rotate = 0;
            item.Amount = 1;

            ObjectManager.Instance.Add(item);

            storage.PushItem(item);
        }

    }
}
