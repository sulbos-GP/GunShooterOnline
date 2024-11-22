using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
    public void Update()
    {
        /*
         *  귀환범위 내에 도착함 => 대기
         */

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
