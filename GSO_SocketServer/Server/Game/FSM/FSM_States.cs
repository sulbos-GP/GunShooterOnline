using Google.Protobuf.Protocol;
using Server.Game.Object;
using Server.Game.Utils;
using ServerCore;
using System;
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

        protected bool isStop; 

        public StateBase(EnemyAI owner)
        {
            Owner = owner;
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
        private Vector2 targetPos;
        public IdleState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Idle;
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {
            //스폰 주변을 배회함.
            //if (isStop) return;
            //float dist = Vector2.Distance(targetPos, Owner.CellPos);
            //Owner.MoveToTarget(targetPos, Owner.lowSpeed);

            //if (dist < 0.1f)
            //{
            //    //도착하면 5초 대기후 새로운 위치를 정하여 이동
            //    Owner.gameRoom.PushAfter(5000, SetNewTargetPos);
            //    isStop = true;
            //}
        }

        public void SetNewTargetPos()
        {
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }
        

        //종료
        public override void Exit()
        {

        }
    }


    public class CheckState : StateBase
    {
        private Vector2 targetPos;

        public CheckState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Check;
            targetPos = Owner.target.CellPos;
            Console.WriteLine($"targetPos : {targetPos}");
        }

        public override void Update()
        {
            if (isStop) return;

            if (Vector2.Distance(targetPos, Owner.CellPos) <= 0.1)
            {
                Owner.gameRoom.PushAfter(3000, CheckToOtherStates);
                isStop = true;
                return;
            }

            #region PathFinding
            List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, targetPos, checkObjects: false); //?이거 길찾기 디버그용인가?
            if(path != null)
            {
                S_AiMove MovePacket = new S_AiMove();
                for (int i = 0; i < path.Count; i++)
                {
                    MovePacket.ObjectId = Owner.Id;
                    MovePacket.PosList.Add(new Vector2IntInfo() { X = path[i].x, Y = path[i].y });
                }
                Owner.gameRoom.BroadCast(MovePacket);
            }
            #endregion

            Owner.MoveToTarget(targetPos, Owner.midSpeed);
            float distanceFromTargetPos = Vector2.Distance(targetPos, Owner.CellPos);
            Console.WriteLine($"이동.현위치 : {Owner.CellPos}\n목표위치 : {targetPos}\n남은거리 : {distanceFromTargetPos}");

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
        public ChaseState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Chase;
        }

        public override void Update()
        {
            if (isStop) return;

            if (Owner.target == null)
            {
                //타겟이 사라지면 3초동안 멈춰있다 전환
                Owner.gameRoom.PushAfter(3000, Owner._state.ChangeState, Owner.ReturnState);
                isStop = true;
                return;
            }

            Owner.MoveToTarget(Owner.target.CellPos, Owner.highSpeed);
            #region PathFinding
            List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, Owner.target.CellPos, checkObjects: false);


            S_AiMove MovePacket = new S_AiMove();
            for (int i = 0; i < path.Count; i++)
            {
                MovePacket.ObjectId = Owner.Id;
                MovePacket.PosList.Add(new Vector2IntInfo() { X= path[i].x, Y = path[i].y });
            }
            Owner.gameRoom.BroadCast(MovePacket);

            #endregion

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            if (distanceToTarget <= Owner.attackRange)
            {
                //공격 범위에 들어온다면 즉시 공격상태로 전환
                Owner._state.ChangeState(Owner.AttackState);
                return;
            }
            else if (distanceToTarget > Owner.chaseRange && distanceToTarget <=Owner.detectionRange)
            {
                //추격범위 밖, 감지범위 안이라면 1초간 더 이동하다가 경계로 전환
                Owner.gameRoom.PushAfter(1000, DelayCheckForCheckStates);
                return;
            }
        }

        public void DelayCheckForCheckStates()
        {
            if(Owner.target == null)
            {
                Owner.
            }
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
        public AttackState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }
        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Attack;
        }

        public override void Update()
        {
            if (isStop) return;

            //공격함수 추가

            //공격후 1초간의 딜레이 후 다음 행동 체크
            Console.WriteLine("1초뒤 다음행동 조건 체크");
            Owner.gameRoom.PushAfter(1000, CheckNextState);
            isStop = true;
        }

        public void CheckNextState()
        {
            if (Owner.target == null)  //공격후 타겟이 없어지면 귀환
            {
                Owner._state.ChangeState(Owner.ReturnState);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            if (distanceToTarget <= Owner.attackRange)
            {
                //아직 공격범위 안이라면 다시 공격
                isStop = false;
                Console.WriteLine("재공격");
                return;
            }
            else if (distanceToTarget <= Owner.chaseRange)
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

        //종료
        public override void Exit()
        {

        }
    }


    public class ReturnState : StateBase
    {
        public ReturnState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        private Vector2 targetPos;
       
        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Return;
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {
            if (isStop) return;

            Owner.MoveToTarget(Owner.spawnPoint, Owner.midSpeed);

            //스폰존 내의 랜덤한 타겟위치로 이동후 대기상태로 전환
            float distanceToTargetPos = Vector2.Distance(targetPos, Owner.CellPos);
            if (distanceToTargetPos <= 0.1f)
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
        public StunState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Stun;
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
        public DeadState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.curState = MobState.Dead;

            //사라지는 시간동안 대기 후 사망 후처리 진행
            Owner.gameRoom.PushAfter((int)(Owner.disappearTime*1000), DestroyOwner);
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
