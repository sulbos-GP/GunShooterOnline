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
using System.IO;
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
        public BaseAI Owner;
        public MobState state { get; }

        protected bool isStop;

        public StateBase(BaseAI owner, MobState type)
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
        
        public IdleState(BaseAI owner) : base(owner, MobState.Idle)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.target = null;

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

            GameObject _target = Owner.DetectObject.GetNearestObject();
            if (_target != null)
            {
                Owner.target = _target;
                Owner.state.ChangeState(Owner.CheckState);
                return;
            }

            int currentTickCount = Environment.TickCount;
            //`Console.WriteLine($"Wait idle to round:{(storeTickCount + idleToRoundTime) - currentTickCount}");
            if (storeTickCount + idleToRoundTime < currentTickCount)
            {
                Owner.state.ChangeState(Owner.RoundState);
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
        public RoundState(BaseAI owner) : base(owner, MobState.Round)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            //targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, 1); //1223


            Owner.PathFinding_Once(Owner.CellPos, targetPos, checkObjects: false);


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

            GameObject _target = Owner.DetectObject.GetNearestObject();
            if (_target != null)
            {
                Owner.target = _target;
                Owner.state.ChangeState(Owner.CheckState);
                return;
            }


            float dist = Vector2.Distance(targetPos, Owner.CellPos);
            //Console.WriteLine($"close to target pos:{dist}");
            if (dist < 1f)
            {
                Owner.state.ChangeState(Owner.IdleState);
                return;
            }


            Owner.MoveAI();

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

        public CheckState(BaseAI owner) : base(owner, MobState.Check)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            targetPos = Owner.target.CellPos;
            storeTickCount = Environment.TickCount;
            //Console.WriteLine($"check targetPos : [{targetPos.X}, {targetPos.Y}]");

    

            Owner.PathFinding_Once(Owner.CellPos, targetPos, checkObjects: false);
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
                Owner.state.ChangeState(Owner.ReturnState);
                return;
            }

         

            float distanceFromTargetPos = Vector2.Distance(targetPos, Owner.CellPos);
            //Console.WriteLine($"close to check target pos:{distanceFromTargetPos}");
            //Console.WriteLine($"이동.현위치 : {Owner.CellPos}\n목표위치 : {targetPos}\n남은거리 : {distanceFromTargetPos}");

            //이동중 추격거리 안에 들어오면 추격상태로
            if (Owner.target != null)
            {
                float distanceToPlayer = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToPlayer <= Owner.chaseRange)
                {
                    Owner.state.ChangeState(Owner.ChaseState);
                    return;
                }
            }

            //if (Vector2.Distance(targetPos, Owner.CellPos) <= 0.2)
            //{
            //    Owner.gameRoom.PushAfter(3000, CheckToOtherStates);
            //    isStop = true;
            //    return;
            //}

            Owner.MoveAI();


        }

        private void CheckToOtherStates()
        {
            //대기시간이 지난시점에서 타겟이 없으면 귀환 있다면 타겟위치 재할당
            if (Owner.target == null)
            {
                Owner.state.ChangeState(Owner.ReturnState);
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
        private const int chaseToReturnTime = 5000;
        private int storeTickCount = 0;

        public ChaseState(BaseAI owner) : base(owner, MobState.Chase)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            storeTickCount = Environment.TickCount;
            Owner.PathFinding_Update_Reset();
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
                Owner.state.ChangeState(Owner.ReturnState);
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
                Owner.state.ChangeState(Owner.ReturnState);
                return;
            }

            //if (Owner.target == null)
            //{
            //    //타겟이 사라지면 3초동안 멈춰있다 전환
            //    Owner.gameRoom.PushAfter(3000, Owner._state.ChangeState, Owner.ReturnState);
            //    isStop = true;
            //    return;
            //}
        

            Owner.PathFinding_Update(Owner.CellPos, Owner.target.CellPos, checkObjects: false);
            //Owner.MoveToTargetList(path);



            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //Console.WriteLine($"close to chase target pos:{distanceToTarget}");
            
            if (distanceToTarget < Owner.attackRange && Owner.CheakTargetVisual(Owner.target.CellPos - Owner.CellPos))
            {
                //공격 범위에 들어온다면 즉시 공격상태로 전환
                Owner.state.ChangeState(Owner.AttackState);
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
                Owner.state.ChangeState(Owner.AttackState);
                return;
            }
            if (distanceToTarget > Owner.chaseRange && distanceToTarget <= Owner.detectionRange)
            {
                Owner.state.ChangeState(Owner.CheckState);
            }
        }

        //종료
        public override void Exit()
        {
            Owner.PathFinding_Update_Reset();

        }
    }

    public class AttackState : StateBase
    {
        private const int waitAttackTime = 500;
        private int storeTickCount = 0;
        public AttackState(BaseAI owner) : base(owner, MobState.Attack)
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

           
        }

        public override void Update()
        {
            base.Update();

            if (Owner == null)
            {
                return;
            }

            BattleGameRoom room = Owner.gameRoom;
            if (room == null)
            {
                return;
            }

            CreatureObj target = (CreatureObj)Owner.target;
            if (target == null || target.IsDead)
            {
                Owner.state.ChangeState(Owner.ReturnState);
                return;
            }



            int currentTickCount = Environment.TickCount;
            //Console.WriteLine($"Wait attack:{(storeTickCount + waitAttackTime) - currentTickCount}");
            if (storeTickCount + waitAttackTime  + Owner.attackDelay * 1000 < currentTickCount)
            {

                //TODO : 공격
                Owner.DoAttack();

                float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToTarget <= Owner.chaseRange)
                {
                    //추격범위 안이라면 추격상태 전환
                    Owner.state.ChangeState(Owner.ChaseState);
                    return;
                }
                else if (distanceToTarget <= Owner.detectionRange)
                {
                    //감지범위 안이라면 경계상태 전환
                    Owner.state.ChangeState(Owner.CheckState);
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
        public ReturnState(BaseAI owner) : base(owner, MobState.Return)
        {
            Owner = owner;
        }
   
        //초기화
        public override void Enter()
        {
            base.Enter();


            Owner.target = null;
            Owner.PathFinding_Once(Owner.CellPos, Owner.spawnPoint, checkObjects: false);
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

       


            //스폰존 내의 랜덤한 타겟위치로 이동후 대기상태로 전환
            float distanceToTargetPos = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            //Console.WriteLine($"close to spawn point:{distanceToTargetPos}");
            if (distanceToTargetPos <= 1f)
            {
                Owner.state.ChangeState(Owner.IdleState);
                return;
            }


            //집으로 귀환
            Owner.MoveAI();
        }

        //종료
        public override void Exit()
        {

        }
    }
    public class StunState : StateBase
    {
        public float stunTime = 5;
        public StunState(BaseAI owner) : base(owner, MobState.Stun)
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
                Owner.state.ChangeState(Owner.ReturnState);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            if (distanceToTarget <= Owner.attackRange)
            {
                Owner.state.ChangeState(Owner.AttackState);
                return;
            }
            else if (distanceToTarget <= Owner.chaseRange)
            {
                Owner.state.ChangeState(Owner.ChaseState);
                return;
            }
            else if (distanceToTarget <= Owner.detectionRange)
            {
                Owner.state.ChangeState(Owner.CheckState);
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
        public DeadState(BaseAI owner) : base(owner, MobState.Dead)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            base.Enter();
            Owner.target = null;

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
