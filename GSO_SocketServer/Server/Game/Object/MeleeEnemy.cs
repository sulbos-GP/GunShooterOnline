using Google.Protobuf.Protocol;
using Server.Game.Object.Attack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class MeleeEnemy : BaseAI
    {
        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            info.Name = "MelleEnemy";
            chaseRange = 8;
            attackRange = 6;

            stat.MergeFrom(new StatInfo()
            {
                //Attack = 3,
                Hp = 150,
                MaxHp = 150,
                AttackRange = 10,

                //방어력

            });

            Speed = 2;


        }



        public override void DoAttack()
        {
            base.DoAttack();
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
