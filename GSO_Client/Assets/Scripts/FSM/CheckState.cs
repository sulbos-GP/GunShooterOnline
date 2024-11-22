using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
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
