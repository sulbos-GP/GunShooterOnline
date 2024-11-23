using UnityEngine;

public class StateBase : IState
{
    public FSM_Enemy Owner;

    protected float waitTimer = 0f; // 대기 타이머
    protected bool isWaiting = false; // 대기 상태 여부
    protected bool isMoveDone = false;
    public StateBase(FSM_Enemy owner)
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

    public IdleState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    // 초기화
    public override void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
        SetNewTarget();
        waitTimer = 0f;
        isWaiting = false;
    }

    public override void Update()
    {
        if (isWaiting)
        {
            //도착하면 5초동안 그자리에서 대기
            waitTimer += Time.deltaTime;
            if (waitTimer >= 5f)
            {
                SetNewPositionWhenTimerDone(); //대기시간이 끝나면 새로운 위치를 정하고 다시 이동함
            }
        }
        else
        {
            //해당 위치로 낮은 속도로 이동
            float dist = Vector3.Distance(targetPos, Owner.transform.position);
            Owner.MoveToTarget(targetPos, Owner.lowSpeed);

            if (dist < 0.1f) // 타겟에 도착했을 때
            {
                isWaiting = true; // 대기 상태로 전환
                waitTimer = 0f;   // 타이머 초기화
            }
        }
    }

    private void SetNewPositionWhenTimerDone()
    {
        SetNewTarget();
        isWaiting = false;
    }

    // 종료
    public override void Exit()
    {
        // 상태가 종료될 때 필요한 정리 작업
    }

    private void SetNewTarget()
    {
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        Debug.Log($"New Target Position: {targetPos}");
    }
}

public class CheckState : StateBase
{
    private Vector2 targetPos;
    public CheckState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //초기화
    public override void Enter()
    {
        Debug.Log("CheckState");
        Owner.curState = MobState.Check;
        targetPos = Owner.target.transform.position; //타겟이 감지된 위치로 이동
        isWaiting = false;
        isMoveDone = false;
    }

    public override void Update()
    {
        // 1. 이동 중이거나 대기 중이라면, 이동을 계속하거나 대기를 진행
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // 대기 종료

                Debug.Log("대기가 끝났습니다. 이동 또는 귀환 처리 시작.");
                if (Owner.target == null)
                {
                    Debug.Log("타겟을 놓침. 이동 완료 후 귀환 상태로 전환.");
                    Owner._state.ChangeState(Owner._return);
                    return;
                }
                if (Vector3.Distance(Owner.target.transform.position, Owner.transform.position) <= Owner.detectionRange)
                {
                    Debug.Log("타겟이 감지 범위에 있음. 새로운 타겟 위치 설정.");
                    targetPos = Owner.target.transform.position; // 새로운 타겟 위치로 업데이트
                    return;
                }
            }
            return;
        }

        Owner.MoveToTarget(targetPos, Owner.midSpeed);
        float distanceFromTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
        if (Owner.target != null)
        {
            float distanceToPlayer = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            if (distanceToPlayer <= Owner.chaseRange)
            {
                Debug.Log("타겟이 추격 범위에 있음. 추격 상태로 전환.");
                Owner._state.ChangeState(Owner._chase);
                return;
            }
        }


        if (distanceFromTargetPos <= 0.1f)
        {
            Debug.Log("타겟 위치에 도달. 대기 시작.");
            StartWait(3f); // 3초 대기
            return;
        }
    }



    //종료
    public override void Exit()
    {

    }

}

public class ChaseState : StateBase
{
    public ChaseState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("ChaseState");
        Owner.curState = MobState.Chase;

    }

    //초기화
    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // 대기 종료

                if (Owner.target == null)
                {
                    Owner._state.ChangeState(Owner._return);
                    return;
                }
                else
                {
                    float dis = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
                    if (dis > Owner.chaseRange && dis<= Owner.detectionRange)
                    {
                        Owner._state.ChangeState(Owner._check); // 경계 상태로 전환
                    }
                }
                
            }
            return;
        }

        if (Owner.target == null) //타겟이 경계범위 밖으로 나가면 3초 대기후 귀환
        {
            Debug.Log("타겟을 놓침. 정지 후 3초 대기 후 귀환 상태로 전환.");
            StartWait(3f);
            return;
        }

        Owner.MoveToTarget(Owner.target.transform.position, Owner.highSpeed);

        float distanceToAttack = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToAttack <= Owner.attackRange)
        {
            Debug.Log("타겟이 공격 범위에 들어왔습니다. 공격 상태로 전환.");
            Owner._state.ChangeState(Owner._attack); // 공격 상태로 전환
            return;
        }

        //*** 다른 대기(정지하고 몇초뒤 전환) 와는 다름. 더 이동하다가 전환
        float distanceToChase = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToChase > Owner.chaseRange)
        {
            StartWait(1f); //추격범위 밖 감지범위 안 일경우 1초간 더 쫒다가 경계상태로 전환함
            return;
        }
    }

    public override void Exit()
    {

    }

}

public class AttackState : StateBase
{
    public AttackState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //초기화
    public override void Enter()
    {
        Debug.Log("AttackState");
        Owner.curState = MobState.Attack;
    }

    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // 대기 종료

                if (Owner.target == null)  //공격후 타겟이 없어지면 귀환
                {
                    Owner._state.ChangeState(Owner._return);
                    return;
                }

                float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
                //타겟이 있다면 타겟과의 거리에 따라 패턴 변경
                if (distanceToTarget <= Owner.attackRange)
                {
                    //초기화 후 재공격
                    Debug.Log("재공격");
                    StartWait(1f);
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
            return;
        }

        //공격 후 딜레이
        StartWait(1f);
    }

    //종료
    public override void Exit()
    {

    }

}


public class ReturnState : StateBase
{
    private Vector2 targetPos;  // 스폰 존 내의 랜덤한 위치
    public ReturnState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //초기화
    public override void Enter()
    {
        Debug.Log("ReturnState");
        Owner.curState = MobState.Return;
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
    }
    public override void Update()
    {
        Owner.MoveToTarget(targetPos, Owner.midSpeed);

        float distanceToTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
        Debug.Log(distanceToTargetPos);
        if (distanceToTargetPos <= 0.1f)
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
    public float stunTime;

    public StunState(FSM_Enemy owner) : base (owner) 
    {
        Owner = owner;
    }

    //초기화
    public override void Enter()
    {
        Debug.Log("StunState");
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
    public DeadState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //초기화
    public override void Enter()
    {
        Debug.Log("DeadState");
        Owner.curState = MobState.Dead;
        StartWait(Owner.disappearTime); //3초뒤 사라짐
    }

    //업데이트
    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // 대기 종료
                //여기에 오브젝트 삭제 코드 삽입
            }
            return;
        }
    }

    //종료
    public override void Exit()
    {
        //사실상 필요없음
    }
}
