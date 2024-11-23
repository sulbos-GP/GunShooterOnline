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

namespace Server.Game.FSM
{

    public class StateBase : IState
    {
        public EnemyAI Owner;

        protected float waitTimer = 0f; // 대기 타이머
        protected bool isWaiting = false; // 대기 상태 여부
        protected bool isMoveDone = false;
        public StateBase(EnemyAI owner)
        {
            Owner = owner;
        }

        public virtual void Enter()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Exit()
        {

        }

        protected virtual void StartWait(float duration)
        {
            waitTimer = duration;
            isWaiting = true;
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

                targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
                
            }

            
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
            if (Owner.target == null)
            {
                Owner.FSMController.ChangeState(Owner._return);
                return;
            }

            Owner.MoveToTarget(Owner.target.CellPos, Owner.midSpeed);

            float distanceFromSpawn = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            if (distanceFromSpawn > Owner.maxDistance)
            {
                Owner.FSMController.ChangeState(Owner._return);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            if (distanceToTarget <= Owner.chaseRange)
            {
                Owner.FSMController.ChangeState(Owner._chase);
                return;
            }


            //// 1. 이동 중이거나 대기 중이라면, 이동을 계속하거나 대기를 진행
            //if (isWaiting)
            //{
            //    waitTimer -= Time.deltaTime;
            //    if (waitTimer <= 0)
            //    {
            //        isWaiting = false; // 대기 종료

            //        Debug.Log("대기가 끝났습니다. 이동 또는 귀환 처리 시작.");
            //        if (Owner.target == null)
            //        {
            //            Debug.Log("타겟을 놓침. 이동 완료 후 귀환 상태로 전환.");
            //            Owner._state.ChangeState(Owner._return);
            //            return;
            //        }
            //        if (Vector3.Distance(Owner.target.transform.position, Owner.transform.position) <= Owner.detectionRange)
            //        {
            //            Debug.Log("타겟이 감지 범위에 있음. 새로운 타겟 위치 설정.");
            //            targetPos = Owner.target.transform.position; // 새로운 타겟 위치로 업데이트
            //            return;
            //        }
            //    }
            //    return;
            //}

            //Owner.MoveToTarget(targetPos, Owner.midSpeed);
            //float distanceFromTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
            //if (Owner.target != null)
            //{
            //    float distanceToPlayer = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            //    if (distanceToPlayer <= Owner.chaseRange)
            //    {
            //        Debug.Log("타겟이 추격 범위에 있음. 추격 상태로 전환.");
            //        Owner._state.ChangeState(Owner._chase);
            //        return;
            //    }
            //}


            //if (distanceFromTargetPos <= 0.1f)
            //{
            //    Debug.Log("타겟 위치에 도달. 대기 시작.");
            //    StartWait(3f); // 3초 대기
            //    return;
            //}
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
                Owner.FSMController.ChangeState(Owner._return);
                return;
            }

            Owner.MoveToTarget(Owner.target.CellPos, Owner.highSpeed);

            List<Vector2Int> path = Owner.gameRoom.map.FindPath(Owner.CellPos, Owner.target.CellPos, checkObjects: false);


            float distanceFromSpawn = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            if (distanceFromSpawn > Owner.maxDistance)
            {
                Owner.FSMController.ChangeState(Owner._return);
                return;
            }

            float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
            if (distanceToTarget <= Owner.attackRange)
            {
                Owner.FSMController.ChangeState(Owner._attack);
                return;
            }
            else if (distanceToTarget > Owner.chaseRange)
            {
                Owner.FSMController.ChangeState(Owner._check);
            }

            //if (isWaiting)
            //{
            //    waitTimer -= Time.deltaTime;
            //    if (waitTimer <= 0)
            //    {
            //        isWaiting = false; // 대기 종료

            //        if (Owner.target == null)
            //        {
            //            Owner._state.ChangeState(Owner._return);
            //            return;
            //        }
            //        else
            //        {
            //            float dis = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            //            if (dis > Owner.chaseRange && dis <= Owner.detectionRange)
            //            {
            //                Owner._state.ChangeState(Owner._check); // 경계 상태로 전환
            //            }
            //        }

            //    }
            //    return;
            //}

            //if (Owner.target == null) //타겟이 경계범위 밖으로 나가면 3초 대기후 귀환
            //{
            //    Debug.Log("타겟을 놓침. 정지 후 3초 대기 후 귀환 상태로 전환.");
            //    StartWait(3f);
            //    return;
            //}

            //Owner.MoveToTarget(Owner.target.transform.position, Owner.highSpeed);

            //float distanceToAttack = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            //if (distanceToAttack <= Owner.attackRange)
            //{
            //    Debug.Log("타겟이 공격 범위에 들어왔습니다. 공격 상태로 전환.");
            //    Owner._state.ChangeState(Owner._attack); // 공격 상태로 전환
            //    return;
            //}

            ////*** 다른 대기(정지하고 몇초뒤 전환) 와는 다름. 더 이동하다가 전환
            //float distanceToChase = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            //if (distanceToChase > Owner.chaseRange)
            //{
            //    StartWait(1f); //추격범위 밖 감지범위 안 일경우 1초간 더 쫒다가 경계상태로 전환함
            //    return;
            //}
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

        float timer;

        //초기화
        public override void Enter()
        {
            Console.WriteLine("AttackState");
            Owner.curState = MobState.Attack;
            timer = 0;
        }

        public override void Update()
        {
            /*
             * 공격을 완료했을때 플레이어가 포착거리 밖으로 나갈경우 => 의심
             *                 플레이어가 포착거리 안에 있음 => 추격 반복
             */
            timer += Program.ServerIntervalTick;
            if (timer > Owner.attackDelay)
            {
                if (Owner.target == null)
                {
                    Owner.FSMController.ChangeState(Owner._return);
                    return;
                }

                float distanceToTarget = Vector2.Distance(Owner.target.CellPos, Owner.CellPos);
                if (distanceToTarget <= Owner.attackRange)
                {
                    //초기화 후 재공격
                    timer = 0;
                    return;
                }
                else if (distanceToTarget <= Owner.chaseRange)
                {
                    Owner.FSMController.ChangeState(Owner._chase);
                    return;
                }
                else if (distanceToTarget <= Owner.detectionRange)
                {
                    Owner.FSMController.ChangeState(Owner._check);
                    return;
                }

            }




            //if (isWaiting)
            //{
            //    waitTimer -= Time.deltaTime;
            //    if (waitTimer <= 0)
            //    {
            //        isWaiting = false; // 대기 종료

            //        if (Owner.target == null)  //공격후 타겟이 없어지면 귀환
            //        {
            //            Owner._state.ChangeState(Owner._return);
            //            return;
            //        }

            //        float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            //        //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
            //        if (distanceToTarget <= Owner.attackRange)
            //        {
            //            //초기화 후 재공격
            //            Debug.Log("재공격");
            //            StartWait(1f);
            //            return;
            //        }
            //        else if (distanceToTarget <= Owner.chaseRange)
            //        {
            //            Owner._state.ChangeState(Owner._chase);
            //            return;
            //        }
            //        else if (distanceToTarget <= Owner.detectionRange)
            //        {
            //            Owner._state.ChangeState(Owner._check);
            //            return;
            //        }
            //    }
            //    return;
            //}

            ////공격 후 딜레이
            //StartWait(1f);
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

        //초기화
        public override void Enter()
        {
            Owner.curState = MobState.Return;
            //targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }

        public override void Update()
        {
            /*
             *  귀환범위 내에 도착함 => 대기
             */
            Owner.MoveToTarget(Owner.spawnPoint, Owner.midSpeed);

            float distanceToSpawner = Vector2.Distance(Owner.spawnPoint, Owner.CellPos);
            if (distanceToSpawner <= Owner.spawnerDistance)
            {
                Owner.FSMController.ChangeState(Owner._idle);
                return;
            }




            //Owner.MoveToTarget(targetPos, Owner.midSpeed);

            //float distanceToTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
            //Debug.Log(distanceToTargetPos);
            //if (distanceToTargetPos <= 0.1f)
            //{
            //    Owner._state.ChangeState(Owner._idle);
            //    return;
            //}
        }

        //종료
        public override void Exit()
        {

        }
    }
    public class StunState : StateBase
    {
        public float stunTime;

        public StunState(EnemyAI owner) : base(owner)
        {
            Owner = owner;
        }

        //초기화
        public override void Enter()
        {
            Owner.curState = MobState.Stun;
        }


        public override void Update()
        {
            /*
             * 스턴이 끝나면 타겟이 존재하는지 확인후 있으면 추격 없으면 귀환
             */
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
            //StartWait(Owner.disappearTime); //3초뒤 사라짐
        }

        //업데이트
        public override void Update()
        {
            //if (isWaiting)
            //{
            //    waitTimer -= Time.deltaTime;
            //    if (waitTimer <= 0)
            //    {
            //        isWaiting = false; // 대기 종료
            //                           //여기에 오브젝트 삭제 코드 삽입
            //    }
            //    return;
            //}
        }

        //종료
        public override void Exit()
        {
            //사실상 필요없음
        }
    }
}
