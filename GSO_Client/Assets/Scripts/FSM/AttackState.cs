using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
    public void Update()
    {
        /*
         * 공격을 완료했을때 플레이어가 포착거리 밖으로 나갈경우 => 의심
         *                 플레이어가 포착거리 안에 있음 => 추격 반복
         */
        timer += Time.deltaTime;
        if (timer > Owner.attackDelay) 
        {
            if(Owner.target == null)
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
