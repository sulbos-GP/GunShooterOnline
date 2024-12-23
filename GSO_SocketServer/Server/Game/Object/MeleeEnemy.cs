using Google.Protobuf.Protocol;
using Server.Game.Object.Attack;
using Server.Game.Object.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Enum;

namespace Server.Game.Object
{
    public class MeleeEnemy : BaseAI
    {
        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            info.Name = "MeleeEnemy";
            chaseRange = 8f;
            attackRange = 1f;
            attackDelay = 1f;

            stat.MergeFrom(new StatInfo()
            {
                //Attack = 3,
                Hp = 100,
                MaxHp = 100,
                AttackRange = 10,
                //방어력

            });
            Speed = 2;
        }

        public override void OnDead(GameObject attacker)
        {
            BoxObject boxObject = ObjectManager.Instance.Add<BoxObject>();
            boxObject.SetBox(this.CellPos, EBoxSize.Large);
            boxObject.AddRandgeItem(EItemType.Defensive, 1, 1);
            boxObject.AddRandgeItem(EItemType.Spoil, 1, 2);
            boxObject.FitBox();
            gameRoom.map.rootableObjects.Add(boxObject);

            S_Spawn spawnPacket = new S_Spawn();
            spawnPacket.Objects.Add(boxObject.info);
            gameRoom.BroadCast(spawnPacket);

            base.OnDead(attacker);
        }

        public override void DoAttack()
        {
            base.DoAttack();


            Console.WriteLine("melee Attack - 1");
            gameRoom.PushAfter((int)(1 * 500), AttakHandle);

        }

        public void AttakHandle()
        {
            Console.WriteLine("melee Attack - 2");

            AttackObjectBase attack = ObjectManager.Instance.Add<AttackObjectBase>();
            attack.Init(this, attackPolygon, 10);

            S_AiAttackShot attackShotPacket = new S_AiAttackShot()
            {
                ObjectId = this.Id
            };
            this.gameRoom.BroadCast(attackShotPacket);

            this.gameRoom.PushAfter(100, attack.Destroy);
        }
    }
}
