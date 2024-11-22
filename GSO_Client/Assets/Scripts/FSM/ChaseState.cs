using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
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
