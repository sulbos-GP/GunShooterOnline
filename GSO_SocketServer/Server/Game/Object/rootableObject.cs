using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class RootableObject : GameObject
    {
        protected bool destroyed = false;
        protected bool active = true;
        public Inventory inventory;

        public RootableObject()
        {
            ObjectType = GameObjectType.Box;


            float width = 1;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;

            currentShape = rectangle;
        }

        public void Init()
        {
            inventory = new Inventory(Id);
        }



      


        
    }
}
