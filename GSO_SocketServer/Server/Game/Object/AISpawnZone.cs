using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class AISpawnZone : GameObject
    {

        public AISpawnZone()
        {
            ObjectType = GameObjectType.Noneobject;

        }


        public void Init(Vector2 pos)
        {
            CellPos = pos;
            /*//ObjectType = GameObjectType.S;
            float width = 1;
            float left = 1;
            float top = 1;

            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;*/

            info.Name = "AISpawnZone" + Id;
        }







    }
}
