﻿using Collision.Shapes;
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
        List<Player> hitPlayers = new List<Player>();

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

            CellPos = polygon.position;
        }

        public void Destroy()
        {
            ObjectManager.Instance.Remove(this.Id);
        }

        public override void OnCollision(GameObject other)
        {
            //Console.WriteLine("COLLL ========== " + other.ObjectType.ToString());
            if (other != null && other.gameRoom != null && other.gameRoom == this.gameRoom)
            {
                BaseAI owner = ObjectManager.Instance.Find<BaseAI>(this.OwnerId);
                if(owner == null)
                {
                    return;
                }

                Player player = other as Player;
                /*player ??= other.GetOwner() as Player;*/

                if(player == null)
                {
                    return;
                }

                if(true == hitPlayers.Contains(player))
                {
                    return;
                }
                hitPlayers.Add(player);

                Console.WriteLine($"{info.Name} to {this.damage}");
                player.OnDamaged(owner, this.damage);

                if(player.Hp <= 0)
                {
                    owner.target = null;
                }

            }
        }
    }
}
