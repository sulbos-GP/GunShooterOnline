using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class Mine : GameObject
    {


        protected bool destroyed = false;
        protected bool active = true;


        public Mine()
        {
            ObjectType = GameObjectType.Mine;
        }


        public void Init(Vector2 pos)
        {
            ObjectType = GameObjectType.Mine;
            float width = 1;
            float left = 1;
            float top = 1;

            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;

            info.Name = "MineZone" + Id;
            CellPos = pos;

        }

        public override void OnCollision(GameObject other)
        {
            //Console.WriteLine("Git"); quadtree에서 지우기
            if (other != null && active && other.gameRoom != null && other.gameRoom == this.gameRoom) //  other.State != CreatureState.Dead) 
            {
                Player p = other as Player;
                if (p != null)
                {
                    Console.WriteLine("p hit Mine");
                    p.OnDamaged(this, 5);
                    active = false;

                }


            }
        }

    }
}
