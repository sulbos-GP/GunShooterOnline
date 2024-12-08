using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.FSM;
using Server.Game.Object.Attack;
using Server.Game.Object.Item;
using Server.Game.Object.Shape;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks.Dataflow;

namespace Server.Game.Object
{
    public class BaseAI : CreatureObj
    {

        #region FSM
        public FSMController _state;

        public IdleState IdleState;
        public RoundState RoundState;
        public CheckState CheckState;
        public ChaseState ChaseState;
        public AttackState AttackState;
        public ReturnState ReturnState;
        public StunState StunState;



        public Vector2 spawnPoint;
        public float maxDistance; //스폰지점과 10만큼 떨어지면 귀환
        public float spawnerDistance; //스폰존 범위


        public float Speed;
        public int AttackDamage;

        public float detectionRange;   //감지 거리 , 가장 길어야함 감지범위의 크기 = *2 . 트리거로 감지범위 안에 들어온 적이 있으면 해당 방향으로 중간속도 이동. 감지범위밖으로 나가고 3초뒤 귀환
        public float chaseRange;        //추격 거리 , 범위안에 타겟이 들어오면 타겟을 향해 빠르게 이동, 해당 범위 밖으로 나가고 3초뒤 의심으로 전환
        public float attackRange;      //공격 거리 , 범위안에 들어오면 타겟을 향해 공격 실행, 공격이 끝나면 타겟의 거리에 따라 반복 공격, 추격 혹은 의심 혹은 귀환
        public float attackDelay;      //공격후 잠시 대기하는 시간
        public float disappearTime; //죽은 적이 3초뒤에 삭제됨

        //public List<GameObject> targets;
        public GameObject target
        {
            get; set;
        }
        public float targetDistance;

        public ScopeObject DetectObject = new ScopeObject();
        #endregion


        //private IJob _job;

        public Polygon attackPolygon;


        public BaseAI()
        {
            ObjectType = GameObjectType.Enemyai;

            #region FSM
            IdleState = new IdleState(this);
            RoundState = new RoundState(this);
            CheckState = new CheckState(this);
            ChaseState = new ChaseState(this);
            AttackState = new AttackState(this);
            ReturnState = new ReturnState(this);
            StunState = new StunState(this);

            _state = new FSMController(this);

            maxDistance = 10;
            spawnerDistance = 5;

            Speed = 5f;
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

            _state.ChangeState(IdleState);
            #endregion

            {
                float width = 3;
                float left = 3;
                float top = 3;
                attackPolygon = ShapeManager.CreateCenterSquare(left, top, width);
            }

            #region DetectObject
            DetectObject.gameRoom = Program.gameserver.gameRoom as BattleGameRoom;
            DetectObject.currentShape = ShapeManager.CreateCircle(CellPos.X, CellPos.Y, detectionRange);


            #endregion
        }

        public virtual void Init(Vector2 pos)
        {
            float width = 0.5f;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;
            currentShape = rectangle;

            info.Name = "Dog";
            ObjectType = GameObjectType.Enemyai;


            DetectObject.Init(this);

            CellPos = pos;
            spawnPoint = CellPos; //임시. 스폰위치를 일시적으로 자신이 생성된 위치로 함

        }

        public override void Update()
        {

            /*  gameRoom.Push(Update);
              if (gameRoom != null) 
                  _job = gameRoom.PushAfter(Program.ServerIntervalTick, Update);*/

            if (gameRoom.IsGameStarted == false)
                return;

            //Console.WriteLine("test");
            //return;

            //만약에 FSM전용으로 만들거면 Update넣을 때 거리 있으면 좋을듯
            //이거는 


            _state.Update();

            DetectObject.Update();


             gameRoom.HandleMove( this, PosInfo );
        }



        public override void OnCollision(GameObject other)
        {
            base.OnCollision(other);
            if (other.ObjectType == GameObjectType.Noneobject)
            {
                return;
            }

        }

        public override void OnCollisionList(GameObject[] others)
        {
            base.OnCollisionList(others);



        }


        public override void OnDead(GameObject attacker)
        {
            if (gameRoom == null)
                return;

            

            {
                _state.ChangeState(new DeadState(this));
            }

            BoxObject boxObject = ObjectManager.Instance.Add<BoxObject>();
            boxObject.CellPos = this.CellPos;
            boxObject.SetRandomItem(1, 3, EBoxSize.Small);
            gameRoom.map.rootableObjects.Add(boxObject);

            S_Spawn spawnPacket = new S_Spawn();
            spawnPacket.Objects.Add(boxObject.info);
            gameRoom.BroadCast(spawnPacket);


            base.OnDead(attacker);


            gameRoom.LeaveGame(Id);
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            Console.WriteLine("Enemy Hit");
            base.OnDamaged(attacker, damage);

            if (Hp > 0)
            {
                this.target = attacker;
                _state.ChangeState(this.ChaseState);
            }

        }

        public override void OnHealed(GameObject healer, int heal)
        {
            base.OnHealed(healer, heal);
        }


        private bool MoveToTarget(Vector2 target, float speed)
        {
            if(Vector2.Distance(target, CellPos) <= 0.01)
            {
                return true;
            }
            Vector2 currentPosition = new Vector2(CellPos.X, CellPos.Y);
            Vector2 directdionToTarget = Vector2.Normalize(target - currentPosition);
            Vector2 newPosition = currentPosition + directdionToTarget * speed * LogicTimer.mFixedDelta; //fxxx
            CellPos = newPosition;

            return false;
            //transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }


        /// <summary>
        /// 리스트에 따라 AI 움직임
        /// </summary>

        int currentTargetIndex = 1; //Pathfinding시 초기화
        public void MoveAI() // 체크, 추적, 귀환
        {
            if (path == null || path.Count == 0)
            {
                //Console.WriteLine("Target list is empty or null!");
                return;
            }


            while (currentTargetIndex < path.Count)
            {
                Vector2Int currentTarget = path[currentTargetIndex];

                bool isArrived = MoveToTarget(new Vector2(currentTarget.x, currentTarget.y), Speed);

                if (isArrived)
                {
                    currentTargetIndex++;
                }
                else
                {
                    //이동 중
                    break;
                }

            }
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

        //원래는 상속해서 만들고 해당 몬스터에 따라서 만들어야 하는거 알쥐?
        //어택 오브젝트도 마찬가지인거 알쥐?
        public virtual void DoAttack()
        {
            
        }

        #region PathFidningCore
        public List<Vector2Int> path;

        public void PathFinding_Once(Vector2 start, Vector2 end, bool checkObjects = false)
        {
            Console.WriteLine(" PathFinding_Once");

            path = gameRoom.map.FindPath(start, end, checkObjects: false);
            if (path != null)
            {
                currentTargetIndex = 1;
                S_AiMove MovePacket = new S_AiMove();
                for (int i = 0; i < path.Count; i++)
                {
                    MovePacket.ObjectId = Id;
                    MovePacket.PosList.Add(new Vector2IntInfo() { X = path[i].x, Y = path[i].y });
                }
                gameRoom.BroadCast(MovePacket);


            }

        }

        public Vector2Int? lastTargetPos;
       /* private Vector2 laststart;
        private Vector2 lastend;*/


        public void PathFinding_Update(Vector2 start, Vector2 end, bool checkObjects = false)
        {

            if(lastTargetPos.HasValue == false) //처음이거나 도착 햇으면
            {
                /* if(start ==  laststart && end == lastend) //길을 못 찾았는데 계속 찾으려고 한다면 
                 {

                 }*/
                Console.WriteLine(" PathFinding_Update");

                path = gameRoom.map.FindPath(start, end, checkObjects: false); 

                if (path != null)
                {
                    S_AiMove MovePacket = new S_AiMove();
                    for (int i = 0; i < path.Count; i++)
                    {
                        MovePacket.ObjectId = Id;
                        MovePacket.PosList.Add(new Vector2IntInfo() { X = path[i].x, Y = path[i].y });
                    }
                    gameRoom.BroadCast(MovePacket);

                    if (path.Count > 1)
                    {
                        lastTargetPos = path[1];
                    }
                    else
                    {
                        lastTargetPos = path[0];
                    }

                    /*laststart = start;
                    lastend = end;*/

                }
            }
            else //목표 있음
            {
            }

            if (path != null)
            {
                bool t = MoveToTarget(new Vector2(lastTargetPos.Value.x, lastTargetPos.Value.y), Speed);
                if (t)
                {
                    lastTargetPos = null;
                }
            }


        }

        #endregion
    }
}
