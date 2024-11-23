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


        protected virtual void ChangeStates(IState changeState)
        {
            Owner._state.ChangeState(changeState);
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
            Owner.curState = MobState.Idle;
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {
            //스폰 존 내를 배회
            float dist = Vector2.Distance(targetPos, Owner.CellPos);
            Owner.MoveToTarget(targetPos, Owner.lowSpeed);

            if (dist < 0.1f)
            {
                //도착하면 5초 대기후 새로운 위치를 정하여 이동
                Owner.gameRoom.PushAfter(5000, SetNewTargetPos);
                isStop = true;
            }
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
            Console.WriteLine("CheckState");

            Owner.curState = MobState.Check;
            targetPos = Owner.target.CellPos;
        }

        public override void Update()
        {
            Owner.MoveToTarget(targetPos, Owner.midSpeed);

            float distanceFromTargetPos = Vector2.Distance(targetPos, Owner.CellPos);

            //이동중 너무 추격범위 안이면 추격상태로
            if (Owner.target != null)
            {
                float distanceToPlayer = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToPlayer <= Owner.chaseRange)
                {
                    Owner._state.ChangeState(Owner._chase);
                    return;
                }
            }

            if (Owner.target == null)
            {
                //상대가 사라지면 3초뒤 귀환
                Owner.gameRoom.PushAfter(3000, Owner._state.ChangeState, Owner._return);
                isStop = true;
                return;
            }

            if (Vector2.Distance(Owner.target.CellPos, Owner.CellPos) <= Owner.detectionRange)
            {
                Owner.gameRoom.PushAfter(3000, SetNewTargetPos); // 새로운 타겟 위치로 업데이트
                isStop= true;
                return;
            }
        }

        public void SetNewTargetPos()
        {
            targetPos = Owner.target.CellPos; // 새로운 타겟 위치로 업데이트
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
            Console.WriteLine("ChaseState");

            Owner.curState = MobState.Chase;
        }

        public override void Update()
        {
            /*
             * 플레이어가 의심거리보다 멀어짐 or 스폰장소에서 너무 멀어짐 => 귀환
             * 플레이어가 공격거리 안으로 들어옴 => 공격
             * 
             */
            if (Owner.target == null)
            {
                Owner.gameRoom.PushAfter(3000, Owner._state.ChangeState, Owner._return);
                isStop = true;
                return;
            }

            Owner.MoveToTarget(Owner.target.CellPos, Owner.highSpeed);

            #region PathFinding
            List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, Owner.target.CellPos, checkObjects: false);


            S_AiMove MovePacket = new S_AiMove();
            for (int i = 0; i < path.Count; i++)
            {
                MovePacket.PosList.Add(new Vector2IntInfo() { X= path[i].x, Y = path[i].y });
            }
            Owner.gameRoom.BroadCast(MovePacket);

            #endregion

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            if (distanceToTarget <= Owner.attackRange)
            {
                Owner._state.ChangeState(Owner._attack);
                return;
            }
            else if (distanceToTarget > Owner.chaseRange && distanceToTarget <=Owner.detectionRange)
            {
                Owner.gameRoom.PushAfter(1000, Owner._state.ChangeState, Owner._check);
                isStop = true;
                return;
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
            Console.WriteLine("AttackState");
            Owner.curState = MobState.Attack;
        }

        public override void Update()
        {

            //공격함수 추가

            //공격후 1초간의 딜레이 후 다음 행동 체크
            Owner.gameRoom.PushAfter(1000, CheckNextState);
            isStop = true;
        }

        public void CheckNextState()
        {
            if (Owner.target == null)  //공격후 타겟이 없어지면 귀환
            {
                Owner._state.ChangeState(Owner._return);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            if (distanceToTarget <= Owner.attackRange)
            {
                isStop = false;
                return;
            }
            else if (distanceToTarget <= Owner.chaseRange)
            {
                Owner._state.ChangeState(Owner._chase);
                return;
            }
            else if (distanceToTarget <= Owner.detectionRange)
            {
                Owner._state.ChangeState(Owner._check);
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
            Owner.curState = MobState.Return;
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {

            Owner.MoveToTarget(Owner.spawnPoint, Owner.midSpeed);

            float distanceToSpawner = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            if (distanceToSpawner <= 0.1f)
            {
                Owner._state.ChangeState(Owner._idle);
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
            Owner.curState = MobState.Stun;
        }

        public void SetStunTime(float _stunTime)
        {
            stunTime = _stunTime;
        }

        public override void Update()
        {
            Owner.gameRoom.PushAfter((int)(stunTime*1000), CheckNextState);
        }

        public void CheckNextState()
        {
            //거리에 따른 상태 변화
            if (Owner.target == null)  //타겟이 없다면 귀환
            {
                Owner._state.ChangeState(Owner._return);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            if (distanceToTarget <= Owner.attackRange)
            {
                Owner._state.ChangeState(Owner._attack);
                return;
            }
            else if (distanceToTarget <= Owner.chaseRange)
            {
                Owner._state.ChangeState(Owner._chase);
                return;
            }
            else if (distanceToTarget <= Owner.detectionRange)
            {
                Owner._state.ChangeState(Owner._check);
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
            Owner.curState = MobState.Dead;
            Owner.gameRoom.PushAfter((int)(Owner.disappearTime*1000), DestroyOwner);
        }

        public void DestroyOwner()
        {
            //오브젝트 삭제? 사망후 후처리
        }
        
        //업데이트
        public override void Update()
        {
            
        }

        //종료
        public override void Exit()
        {
            //사실상 필요없음
        }
    }
}
