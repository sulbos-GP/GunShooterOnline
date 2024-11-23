using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class IdleState : IState
{
    public FSM_Enemy Owner;
    private Vector2 targetPos;
    public IdleState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
    }

    public void Update()
    {
        //스폰 존 내를 배회
        float dist = Vector3.Distance(targetPos, Owner.transform.position);
        Owner.MoveToTarget(targetPos, Owner.lowSpeed);

        if (dist < 0.1f)
        {
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }
    }

    //종료
    public void Exit()
    {
        
    }
}


public class CheckState : IState
{
    public FSM_Enemy Owner;

    public CheckState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("CheckState");
        Owner.curState = MobState.Check;
    }

    public void Update()
    {
        /*
         * 타겟의 방향으로 이동
         * 
         * 
         * 스폰장소에서 너무 멀어짐 or 타겟이 의심범위에서 사라짐 => 귀환
         * 타겟이 추적감지범위만큼 가까워짐 => 추적
         */
        if (Owner.target == null)
        {
            Debug.Log("타겟과 너무 멀어진 귀환");
            Owner._state.ChangeState(Owner._return);
            return;
        }

        Owner.MoveToTarget(Owner.target.transform.position, Owner.midSpeed);

        float distanceFromSpawn = Vector3.Distance(Owner.spawnPoint.position, Owner.transform.position);
        if (distanceFromSpawn > Owner.maxDistance)
        {
            Debug.Log("스폰과 너무 멀어진 귀환");
            Owner._state.ChangeState(Owner._return);
            return;
        }

        float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToTarget <= Owner.chaseRange)
        {
            Owner._state.ChangeState(Owner._chase);
            return;
        }
    }

    //종료
    public void Exit()
    {

    }
}

public class ChaseState : IState
{
    public FSM_Enemy Owner;

    public ChaseState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("ChaseState");
        Owner.curState = MobState.Chase;
    }
    
    public void Update()
    {
        /*
         * 플레이어가 의심거리보다 멀어짐 or 스폰장소에서 너무 멀어짐 => 귀환
         * 플레이어가 공격거리 안으로 들어옴 => 공격
         * 
         */
        if (Owner.target == null)
        {
            Owner._state.ChangeState(Owner._return);
            return;
        }

        Owner.MoveToTarget(Owner.target.transform.position, Owner.highSpeed);

        float distanceFromSpawn = Vector3.Distance(Owner.spawnPoint.position, Owner.transform.position);
        if (distanceFromSpawn > Owner.maxDistance)
        {
            Owner._state.ChangeState(Owner._return);
            return;
        }

        float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToTarget <= Owner.attackRange)
        {
            Owner._state.ChangeState(Owner._attack);
            return;
        }
        else if (distanceToTarget > Owner.chaseRange)
        {
            Owner._state.ChangeState(Owner._check);
        }
    }

    //종료
    public void Exit()
    {

    }
}

public class AttackState : IState
{
    public FSM_Enemy Owner;

    public AttackState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    float timer;

    //초기화
    public void Enter()
    {
        Debug.Log("AttackState");
        Owner.curState = MobState.Attack;
        timer = 0;
    }

    public void Update()
    {
        /*
         * 공격을 완료했을때 플레이어가 포착거리 밖으로 나갈경우 => 의심
         *                 플레이어가 포착거리 안에 있음 => 추격 반복
         */
        timer += Time.deltaTime;
        if (timer > Owner.attackDelay)
        {
            if (Owner.target == null)
            {
                Owner._state.ChangeState(Owner._return);
                return;
            }

            float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            if (distanceToTarget <= Owner.attackRange)
            {
                //초기화 후 재공격
                Debug.Log("재공격");
                timer = 0;
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
    }

    //종료
    public void Exit()
    {

    }
}


public class ReturnState : IState
{
    public FSM_Enemy Owner;

    public ReturnState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("ReturnState");
        Owner.curState = MobState.Return;
    }
    public void Update()
    {
        /*
         *  귀환범위 내에 도착함 => 대기
         */
        Owner.MoveToTarget(Owner.spawnPoint.transform.position, Owner.midSpeed);

        float distanceToSpawner = Vector3.Distance(Owner.spawnPoint.position, Owner.transform.position);
        if (distanceToSpawner <= Owner.spawnerDistance)
        {
            Owner._state.ChangeState(Owner._idle);
            return;
        }
    }

    //종료
    public void Exit()
    {

    }
}
public class StunState : IState
{
    public FSM_Enemy Owner;
    public float stunTime;

    public StunState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("StunState");
        Owner.curState = MobState.Stun;
    }


    public void Update()
    {
        /*
         * 스턴이 끝나면 타겟이 존재하는지 확인후 있으면 추격 없으면 귀환
         */
    }

    //종료
    public void Exit()
    {

    }
}

public class DeadState : IState
{
    public FSM_Enemy Owner;
    public DeadState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("DeadState");
        Owner.curState = MobState.Dead;
    }

    //업데이트
    public void Update()
    {
        //사실상 필요없음
    }

    //종료
    public void Exit()
    {
        //사실상 필요없음
    }
}
