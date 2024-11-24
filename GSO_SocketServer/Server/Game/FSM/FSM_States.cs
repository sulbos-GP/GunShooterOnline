using Collision.Shapes;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Server.Game.Object;
using Server.Game.Utils;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Game.FSM
{
    public class StateBase : IState
    {
        public EnemyAI Owner;
        public MobState state { get; }

        protected bool isStop;

        public StateBase(EnemyAI owner, MobState type)
        {
            Owner = owner;
            state = type;
        }

        public virtual void Enter()
        {
            isStop = false;
        }

        public virtual void Update()
        {
            if(isStop) return;
        }

        public virtual void Exit()
        {

        }
    }

    public class IdleState : StateBase
    {
        private const int idleToRoundTime = 5000;
        private int storeTickCount = 0;
        
        public IdleState(EnemyAI owner) : base(owner, MobState.Idle)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            storeTickCount = Environment.TickCount;
        }

        public override void Update()
        {
            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            GameObject target = Owner.DetectObject.GetNearestObject();
            if (target != null)
            {
                Owner.target = target;
                Owner._state.ChangeState(Owner.CheckState);
                return;
            }

            int currentTickCount = Environment.TickCount;
            Console.WriteLine($"Wait idle to round:{(storeTickCount + idleToRoundTime) - currentTickCount}");
            if (storeTickCount + idleToRoundTime < currentTickCount)
            {
                Owner._state.ChangeState(Owner.RoundState);
            }
        }

        //종료
        public override void Exit()
        {

        }
    }

    public class RoundState : StateBase
    {
        private Vector2 targetPos;
        public RoundState(EnemyAI owner) : base(owner, MobState.Round)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {
            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            GameObject target = Owner.DetectObject.GetNearestObject();
            if (target != null)
            {
                Owner.target = target;
                Owner._state.ChangeState(Owner.CheckState);
                return;
            }

            Owner.MoveToTarget(targetPos, Owner.lowSpeed);

            float dist = Vector2.Distance(targetPos, Owner.CellPos);
            Console.WriteLine($"close to target pos:{dist}");
            if (dist < 0.2f)
            {
                Owner._state.ChangeState(Owner.IdleState);
                return;
            }
        }

        //종료
        public override void Exit()
        {

        }
    }

    public class CheckState : StateBase
    {
        private const int checkToReturnTime = 5000;
        private Vector2 targetPos;
        private int storeTickCount = 0;

        public CheckState(EnemyAI owner) : base(owner, MobState.Check)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            targetPos = Owner.target.CellPos;
            storeTickCount = Environment.TickCount;
            Console.WriteLine($"check targetPos : [{targetPos.X}, {targetPos.Y}]");
        }

        public override void Update()
        {

            if (Owner == null)
            {
                return;
            }

            GameObject target = Owner.target;
            if (target == null) 
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            //이상이 없다면 돌아감
            int currentTickCount = Environment.TickCount;
            if (storeTickCount + checkToReturnTime < currentTickCount)
            {
                Owner._state.ChangeState(Owner.ReturnState);
                return;
            }

            Owner.MoveToTarget(targetPos, Owner.midSpeed);
            float distanceFromTargetPos = Vector2.Distance(targetPos, Owner.CellPos);
            Console.WriteLine($"close to check target pos:{distanceFromTargetPos}");
            //Console.WriteLine($"이동.현위치 : {Owner.CellPos}\n목표위치 : {targetPos}\n남은거리 : {distanceFromTargetPos}");

            //이동중 추격거리 안에 들어오면 추격상태로
            if (Owner.target != null)
            {
                float distanceToPlayer = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToPlayer <= Owner.chaseRange)
                {
                    Owner._state.ChangeState(Owner.ChaseState);
                    return;
                }
            }

            //if (Vector2.Distance(targetPos, Owner.CellPos) <= 0.2)
            //{
            //    Owner.gameRoom.PushAfter(3000, CheckToOtherStates);
            //    isStop = true;
            //    return;
            //}

            //#region PathFinding
            //List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, targetPos, checkObjects: false); //?이거 길찾기 디버그용인가?
            //if(path != null)
            //{
            //    S_AiMove MovePacket = new S_AiMove();
            //    for (int i = 0; i < path.Count; i++)
            //    {
            //        MovePacket.ObjectId = Owner.Id;
            //        MovePacket.PosList.Add(new Vector2IntInfo() { X = path[i].x, Y = path[i].y });
            //    }
            //    Owner.gameRoom.BroadCast(MovePacket);
            //}
            //#endregion


        }

        private void CheckToOtherStates()
        {
            //대기시간이 지난시점에서 타겟이 없으면 귀환 있다면 타겟위치 재할당
            if (Owner.target == null)
            {
                Owner._state.ChangeState(Owner.ReturnState);
            }
            else
            {
                SetNewTargetPos();
                return;
            }
        }

        public void SetNewTargetPos()
        {
            targetPos = Owner.target.CellPos; // 새로운 타겟 위치로 업데이트
            isStop = false;                   // 정지상태 해제
        }

        //종료
        public override void Exit()
        {

        }
    }

    public class ChaseState : StateBase
    {
        private const int chaseToReturnTime = 10000;
        private int storeTickCount = 0;

        public ChaseState(EnemyAI owner) : base(owner, MobState.Chase)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            storeTickCount = Environment.TickCount;
        }

        public override void Update()
        {
            if (Owner == null)
            {
                return;
            }

            GameObject target = Owner.target;
            if (target == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            //이상이 없다면 돌아감
            int currentTickCount = Environment.TickCount;
            if (storeTickCount + chaseToReturnTime < currentTickCount)
            {
                Owner._state.ChangeState(Owner.ReturnState);
                return;
            }

            //if (Owner.target == null)
            //{
            //    //타겟이 사라지면 3초동안 멈춰있다 전환
            //    Owner.gameRoom.PushAfter(3000, Owner._state.ChangeState, Owner.ReturnState);
            //    isStop = true;
            //    return;
            //}

            Owner.MoveToTarget(Owner.target.CellPos, Owner.highSpeed);
            //#region PathFinding
            //List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, Owner.target.CellPos, checkObjects: false);


            //S_AiMove MovePacket = new S_AiMove();
            //for (int i = 0; i < path.Count; i++)
            //{
            //    MovePacket.ObjectId = Owner.Id;
            //    MovePacket.PosList.Add(new Vector2IntInfo() { X= path[i].x, Y = path[i].y });
            //}
            //Owner.gameRoom.BroadCast(MovePacket);

            //#endregion

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //Console.WriteLine($"close to chase target pos:{distanceToTarget}");
            
            if (distanceToTarget <= Owner.attackRange)
            {
                //공격 범위에 들어온다면 즉시 공격상태로 전환
                Owner._state.ChangeState(Owner.AttackState);
                return;
            }
            //else if (distanceToTarget > Owner.chaseRange && distanceToTarget <=Owner.detectionRange)
            //{
            //    //추격범위 밖, 감지범위 안이라면 1초간 더 이동하다가 경계로 전환
            //    Owner.gameRoom.PushAfter(1000, DelayCheckForCheckStates);
            //    return;
            //}
        }

        public void DelayCheckForCheckStates()
        {

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);

            if (distanceToTarget <= Owner.attackRange)
            {
                //공격 범위에 들어온다면 즉시 공격상태로 전환
                Owner._state.ChangeState(Owner.AttackState);
                return;
            }
            if (distanceToTarget > Owner.chaseRange && distanceToTarget <= Owner.detectionRange)
            {
                Owner._state.ChangeState(Owner.CheckState);
            }
        }

        //종료
        public override void Exit()
        {

        }
    }

    public class AttackState : StateBase
    {
        private const int waitAttackTime = 1000;
        private int storeTickCount = 0;
        public AttackState(EnemyAI owner) : base(owner, MobState.Attack)
        {
            Owner = owner;
        }
        //초기화
        public override void Enter()
        {
            base.Enter();

            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            storeTickCount = Environment.TickCount;

            S_AiAttack attackPacket = new S_AiAttack()
            {
                ObjectId = Owner.OwnerId,
                Shape = new ShapeInfo()
                {
                    ShpapeType = (Google.Protobuf.Protocol.ShapeType)Owner.attackPolygon.Type,
                    CenterPosX = Owner.attackPolygon.position.x,
                    CenterPosY = Owner.attackPolygon.position.y,
                    Roatation = Owner.attackPolygon.rotation
                }

            };
            Shape s = Owner.attackPolygon;
            switch (attackPacket.Shape.ShpapeType)
            {
                case Google.Protobuf.Protocol.ShapeType.Shape:
                    break;
                case Google.Protobuf.Protocol.ShapeType.Circle:
                    attackPacket.Shape.Radius = ((Circle)s).radius;
                    break;
                case Google.Protobuf.Protocol.ShapeType.Rectangle:
                    attackPacket.Shape.Left = ((Rectangle)s).Left;
                    attackPacket.Shape.Bottom = ((Rectangle)s).Bottom;
                    attackPacket.Shape.Width = ((Rectangle)s).Width;
                    attackPacket.Shape.Height = ((Rectangle)s).Height;
                break;
                case Google.Protobuf.Protocol.ShapeType.Polygon:
                    Console.WriteLine("Polygon Error");

                    break;
                case Google.Protobuf.Protocol.ShapeType.Arcpoly:
                    Console.WriteLine("Arcpoly Error");

                    break;
                default:
                    break;
            }

                   
            room.BroadCast(attackPacket);
        }

        public override void Update()
        {
            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            GameObject target = Owner.target;
            if (target == null)
            {
                Owner._state.ChangeState(Owner.ReturnState);
                return;
            }

            int currentTickCount = Environment.TickCount;
            Console.WriteLine($"Wait attack:{(storeTickCount + waitAttackTime) - currentTickCount}");
            if (storeTickCount + waitAttackTime < currentTickCount)
            {

                //TODO : 공격
                Owner.DoAttack();

                float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToTarget <= Owner.chaseRange)
                {
                    //추격범위 안이라면 추격상태 전환
                    Owner._state.ChangeState(Owner.ChaseState);
                    return;
                }
                else if (distanceToTarget <= Owner.detectionRange)
                {
                    //감지범위 안이라면 경계상태 전환
                    Owner._state.ChangeState(Owner.CheckState);
                    return;
                }
            }



            //공격함수 추가

            //공격후 1초간의 딜레이 후 다음 행동 체크
            //Console.WriteLine("1초뒤 다음행동 조건 체크");
            //Owner.gameRoom.PushAfter(1000, CheckNextState);
            //isStop = true;
        }

        public void CheckNextState()
        {
            if (Owner.target == null)  //공격후 타겟이 없어지면 귀환
            {

                return;
            }


        }

        //종료
        public override void Exit()
        {

        }
    }


    public class ReturnState : StateBase
    {
        public ReturnState(EnemyAI owner) : base(owner, MobState.Return)
        {
            Owner = owner;
        }
   
        //초기화
        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            //집으로 귀환
            Owner.MoveToTarget(Owner.spawnPoint, Owner.highSpeed);

            //스폰존 내의 랜덤한 타겟위치로 이동후 대기상태로 전환
            float distanceToTargetPos = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            Console.WriteLine($"close to spawn point:{distanceToTargetPos}");
            if (distanceToTargetPos <= 0.2f)
            {
                Owner._state.ChangeState(Owner.IdleState);
                return;
            }
        }

        //종료
        public override void Exit()
        {

        }
    }
    public class StunState : StateBase
    {
        public float stunTime = 5;
        public StunState(EnemyAI owner) : base(owner, MobState.Stun)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
        }

        public void SetStunTime(float _stunTime)
        {
            stunTime = _stunTime;
        }

        public override void Update()
        {
            if (isStop) return;

            //스턴시간만큼 대기 후 조건 체크 후 만족하는 다음 상태로 즉시 전환
            Owner.gameRoom.PushAfter((int)(stunTime*1000), CheckNextState);
        }

        public void CheckNextState()
        {
            //거리에 따른 상태 변화
            if (Owner.target == null)  //타겟이 없다면 귀환
            {
                Owner._state.ChangeState(Owner.ReturnState);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            if (distanceToTarget <= Owner.attackRange)
            {
                Owner._state.ChangeState(Owner.AttackState);
                return;
            }
            else if (distanceToTarget <= Owner.chaseRange)
            {
                Owner._state.ChangeState(Owner.ChaseState);
                return;
            }
            else if (distanceToTarget <= Owner.detectionRange)
            {
                Owner._state.ChangeState(Owner.CheckState);
                return;
            }
        }

        //종료
        public override void Exit()
        {

        }
    }

    public class DeadState : StateBase
    {
        public DeadState(EnemyAI owner) : base(owner, MobState.Dead)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();

            //사라지는 시간동안 대기 후 사망 후처리 진행
            //Owner.gameRoom.PushAfter((int)(Owner.disappearTime*1000), DestroyOwner);
        }

        public void DestroyOwner()
        {
            //오브젝트 삭제? 사망후 후처리
        }
        
        //업데이트
        public override void Update()
        {
            base.Update();
        }

        //종료
        public override void Exit()
        {
            //사실상 필요없음
        }
    }
}
