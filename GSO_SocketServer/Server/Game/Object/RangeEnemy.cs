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
    public class RangeEnemy : BaseAI
    {

        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            info.Name = "RangeEnemy";
            chaseRange = 8;
            attackRange = 6;
            //Attack = 3;

            stat.MergeFrom(new StatInfo()
            {
                AttackSpeed = 3,
                Hp = 70,
                MaxHp = 70,
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

        public override void OnDead(GameObject attacker)
        {
            BoxObject boxObject = ObjectManager.Instance.Add<BoxObject>();
            boxObject.SetBox(this.CellPos, EBoxSize.Large);
            boxObject.AddItem("Aug", 1);
            boxObject.AddItem("7.29mm", 20);
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
            //Console.WriteLine("Range Attack - 1");
            /*
                        AttackObjectBase attack = ObjectManager.Instance.Add<AttackObjectBase>();
                        attack.Init(this, attackPolygon, 10);*/


            /*  S_AiAttackReady s_AiAttackReady = new S_AiAttackReady();

              s_AiAttackReady.ObjectId = Id;
              s_AiAttackReady.DirX = _rangeAttackDir.X;
              s_AiAttackReady.DirY = _rangeAttackDir.Y;

              gameRoom.BroadCast(s_AiAttackReady);*/










            gameRoom.PushAfter((int)(1 * 1000), AttakHandle);

        }

        public void AttakHandle()
        {
            //Console.WriteLine("Range Attack - 2");


            RaycastHit2D hit2D = RaycastManager.Raycast(CellPos, rangeAttackDir, 100, new List<GameObject>() { this }); //충돌객체 체크

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
                Console.WriteLine("hit Wall");
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
