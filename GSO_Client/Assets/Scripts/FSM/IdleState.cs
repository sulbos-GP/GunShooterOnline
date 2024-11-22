using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class IdleState : IState
{
    public FSM_Enemy Owner;

    public IdleState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //초기화
    public void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
    }

    //업데이트 -> 지속할 행동 + 다른 상태로 넘어가기 체크
    public void Update()
    {
        //스폰 존 내를 배회

    }

    //종료
    public void Exit()
    {
        
    }
}
