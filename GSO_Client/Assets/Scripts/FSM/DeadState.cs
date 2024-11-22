using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        //사망스프라이트로 변경
        //owner에서 OnDead코루틴 실행 = 몇초뒤 파괴
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
