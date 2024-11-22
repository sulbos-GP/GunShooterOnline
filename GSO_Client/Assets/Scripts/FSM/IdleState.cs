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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        //���� �� ���� ��ȸ

    }

    //����
    public void Exit()
    {
        
    }
}
