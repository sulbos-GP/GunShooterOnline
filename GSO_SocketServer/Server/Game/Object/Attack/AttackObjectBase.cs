using Collision.Shapes;
using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game.Object.Attack
{
    public class AttackObjectBase : GameObject
    {

        int damage;

        public AttackObjectBase()
        {
            ObjectType = GameObjectType.Attack;
        }

        public void Init(GameObject owner, int damage, Vector2 pos)
        {
            if (owner == null)
            {
                return;
            }

            OwnerId = owner.Id;
            info.Name = $"[{owner.info.Name}:{owner.Id}] ATTACK";
            CellPos = pos;

            this.damage = damage;

            ObjectType = GameObjectType.Attack;
            float width = 1;
            float left = 1;
            float top = 1;

            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;

        }

        public void Destroy()
        {
            ObjectManager.Instance.Remove(this.Id);
        }

        public override void OnCollision(GameObject other)
        {

            if (other != null && other.gameRoom != null && other.gameRoom == this.gameRoom)
            {
                Player p = other as Player;
                if (p != null)
                {
                    Console.WriteLine($"{info.Name} to {this.damage}");
                    p.OnDamaged(this, this.damage);
                }
            }
        }
    }
}
