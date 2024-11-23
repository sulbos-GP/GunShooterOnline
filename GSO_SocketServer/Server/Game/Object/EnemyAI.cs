using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.FSM;
using Server.Game.Object.Shape;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class EnemyAI : CreatureObj
    {

        #region FSM
        public FSMController FSMController;
        public MobState curState;

        public IdleState _idle;
        public CheckState _check;
        public ChaseState _chase;
        public AttackState _attack;
        public ReturnState _return;
        public StunState _stun;



        public Vector2 spawnPoint;
        public float maxDistance; //스폰지점과 10만큼 떨어지면 귀환
        public float spawnerDistance;

        public float lowSpeed;
        public float midSpeed;
        public float highSpeed;
        public float detectionRange;   //감지 거리 , 가장 길어야함 감지범위의 크기 = *2 . 트리거로 감지범위 안에 들어온 적이 있으면 해당 방향으로 중간속도 이동. 감지범위밖으로 나가고 3초뒤 귀환
        public float chaseRange;        //추격 거리 , 범위안에 타겟이 들어오면 타겟을 향해 빠르게 이동, 해당 범위 밖으로 나가고 3초뒤 의심으로 전환
        public float attackRange;      //공격 거리 , 범위안에 들어오면 타겟을 향해 공격 실행, 공격이 끝나면 타겟의 거리에 따라 반복 공격, 추격 혹은 의심 혹은 귀환
        public float attackDelay;      //공격후 잠시 대기하는 시간
        public float disappearTime; //죽은 적이 3초뒤에 삭제됨

        public List<GameObject> targets;
        public GameObject? target
        {
            get
            {
                if (targets != null && targets.Count > 0)
                {
                    GameObject closestTarget = null;
                    float closestDistance = float.MaxValue;

                    foreach (GameObject t in targets)
                    {
                        Vector2 targetPos = t.CellPos;
                        float distance = (CellPos - targetPos).Length();
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = t;
                        }
                    }

                    return closestTarget;
                }

                return null; // 타겟이 없을 경우 null 반환
            }
        }
        public float targetDistance;

        public ScopeObject DetectObject = new ScopeObject();
        #endregion


        private IJob _job;


        public EnemyAI()
        {
            ObjectType = GameObjectType.Enemyai;


            #region FSM
            _idle = new IdleState(this);
            _check = new CheckState(this);
            _chase = new ChaseState(this);
            _attack = new AttackState(this);
            _return = new ReturnState(this);
            _stun = new StunState(this);

            FSMController = new FSMController();

            maxDistance = 10;
            spawnerDistance = 5;

            lowSpeed = 0.5f;
            midSpeed = 1f;
            highSpeed = 2f;
            detectionRange = 10f;  //감지 거리 , 감지범위의 크기 = *2 . 트리거로 감지범위 안에 들어온 적이 있으면 해당 방향으로 중간속도 이동. 감지범위밖으로 나가고 3초뒤 귀환
            chaseRange = 6;        //추격 거리 , 범위안에 타겟이 들어오면 타겟을 향해 빠르게 이동, 해당 범위 밖으로 나가고 3초뒤 의심으로 전환
            attackRange = 2f;      //공격 거리 , 범위안에 들어오면 타겟을 향해 공격 실행, 공격이 끝나면 타겟의 거리에 따라 반복 공격, 추격 혹은 의심 혹은 귀환
            attackDelay = 2f;      //공격후 잠시 대기하는 시간
            disappearTime = 3f;    //죽은 적이 3초뒤에 삭제됨

            stat.MergeFrom(new StatInfo()
            {
                //Attack = 3,
                Hp = 20,
                MaxHp = 20,
            });

            FSMController.ChangeState(_idle);
            #endregion

            #region DetectObject
            DetectObject.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;
            DetectObject.currentShape = ShapeManager.CreateCircle(CellPos.X, CellPos.Y, detectionRange);


            #endregion
        }

        public void Init(Vector2 pos)
        {
            float width = 0.5f;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;
            currentShape = rectangle;



            Console.WriteLine("생성자 테스트");
            info.Name = "Dog";


            #region DetectObject
            DetectObject.info.Name = info.Name + "DetectObject";
            DetectObject.OwnerId = Id;
            DetectObject.currentShape.Parent = this;
            DetectObject.CellPos = CellPos;
            CellPos = pos;

            #endregion

        }


        public override void Update()
        {
            if (gameRoom != null) 
                _job = gameRoom.PushAfter(Program.ServerIntervalTick, Update);

            if (target != null)
            {
                float targetDis = Vector2.Distance(target.CellPos, CellPos);
            }
            if (curState == MobState.Dead)
            {
                return;
            }

            FSMController.Update();

            if (Hp <= 0)
            {
                FSMController.ChangeState(new DeadState(this));
            }


            //업데이트 코드
        }



        public override void OnCollision(GameObject other)
        {
            base.OnCollision(other);
        }

        public override void OnCollisionList(GameObject[] others)
        {
            base.OnCollisionList(others);



        }


        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnHealed(GameObject healer, int heal)
        {
            base.OnHealed(healer, heal);
        }


        public void MoveToTarget(Vector2 target, float speed)
        {
            Vector2 currentPosition = new Vector2(CellPos.X,CellPos.Y);
            Vector2 directionToTarget = Vector2.Normalize(target - currentPosition);
            Vector2 newPosition = currentPosition + directionToTarget * speed * Program.ServerIntervalTick;
            CellPos = newPosition;
            //transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }


        public Vector2 GetRandomPosInSpawnZone(Vector2 center, float radius)
        {
            Random random = new Random();

            float angle = (float)(random.NextDouble() * MathF.PI * 2);
            float distance = (float)(Math.Sqrt(random.NextDouble()) * radius);


            float x = MathF.Cos(angle) * distance;
            float z = MathF.Sin(angle) * distance;

            return new Vector2(center.X + x, center.Y);
        }






    }
}
