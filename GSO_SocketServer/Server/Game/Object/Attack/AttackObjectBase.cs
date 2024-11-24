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

        public void Init(GameObject owner, Polygon polygon, int damage)
        {
            if (owner == null)
            {
                return;
            }

            ObjectType = GameObjectType.Attack;
            OwnerId = owner.Id;
            info.Name = $"[{owner.info.Name}:{owner.Id}] Melee ATTACK";

            this.damage = damage;

            polygon.Parent = this;
            this.currentShape = polygon;

            CellPos = owner.CellPos;
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
