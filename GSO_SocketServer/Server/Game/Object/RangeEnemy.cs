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
    public class RangeEnemy : EnemyAI
    {

        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            info.Name = "RangeEnemy";
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


/*            {
                float width = 3;
                float left = 3;
                float top = 3;
                attackPolygon = ShapeManager.CreateCenterSquare(left, top, width);
            }*/
        }


        public override void DoAttack()
        {
            //base.DoAttack();
            Console.WriteLine();
            /*
                        AttackObjectBase attack = ObjectManager.Instance.Add<AttackObjectBase>();
                        attack.Init(this, attackPolygon, 10);*/

            Vector2 dir = Vector2.Normalize(CellPos - target.CellPos);
            RaycastHit2D hit2D = RaycastManager.Raycast(CellPos, dir, 100, new List<GameObject>() { this }); //충돌객체 체크

            //충돌된 객체 없음
            if (hit2D.Collider == null)
            {
                BroadcastHitPacket(-1, this, CellPos, hit2D.hitPoint.Value);

                Console.WriteLine("hit is null");
                return;
            }

            GameObject hitObject = hit2D.Collider.Parent;

            //충돌은 했는데 충돌한 객체에는 오브젝트가 없음
            if (hitObject == null)
            {
                BroadcastHitPacket(-1, this, CellPos, hit2D.hitPoint.Value);
                Console.WriteLine("hit is null");
                return;
            }

            if (CheckHitObjectType(hitObject))
            {
                CreatureObj creatureObj = hitObject as CreatureObj;

                //TODO : 피격자의 hp 변화  attacker 밑에 넣기 240814지승현
                creatureObj.OnDamaged(this, AttackDamage);

                {
                    S_ChangeHp ChangeHpPacket = new S_ChangeHp();
                    ChangeHpPacket.ObjectId = creatureObj.Id;
                    ChangeHpPacket.Hp = creatureObj.Hp;
                    this.gameRoom.BroadCast(ChangeHpPacket);
                }
              

                Console.WriteLine("attacker Id :" + this.Id + ", " + "HIT ID " + creatureObj.Id + "HIT Hp : " + creatureObj.Hp);

            }

            BroadcastHitPacket(hitObject.Id, this, CellPos, hit2D.hitPoint.Value);

        
        }



        private  bool CheckHitObjectType(GameObject hitObject)
        {
            return hitObject.ObjectType == GameObjectType.Player || hitObject.ObjectType == GameObjectType.Enemyai;
        }

        private  void BroadcastHitPacket(int hitObjectId, GameObject attacker, Vector2 startPos, Vector2 hitPoint)
        {
            S_RaycastShoot hit = new S_RaycastShoot
            {
                HitObjectId = hitObjectId,
                ShootPlayerId = attacker.Id,
                HitPointX = hitPoint.X,
                HitPointY = hitPoint.Y,
                StartPosX = startPos.X,
                StartPosY = startPos.Y
            };
            attacker.gameRoom.BroadCast(hit);
        }
    }
}
