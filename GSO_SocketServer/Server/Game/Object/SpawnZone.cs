using Collision.Shapes;
using Google.Protobuf.Protocol;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class SpawnZone : GameObject
    {

        public SpawnZone()
        {
            ObjectType = GameObjectType.Spawnzone;

        }

        public void Init(Vector2 pos)
        {
            CellPos = pos;
            //ObjectType = GameObjectType.S;
            float width = 2;
            float left = 2;
            float top = 2;

            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;

            info.Name = "SpawnZone" + Id;

        }









    }
}
