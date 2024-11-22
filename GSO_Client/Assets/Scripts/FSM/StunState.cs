using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    public FSM_Enemy Owner;

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

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
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
