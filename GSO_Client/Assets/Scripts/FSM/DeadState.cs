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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("DeadState");
        Owner.curState = MobState.Dead;
        //�����������Ʈ�� ����
        //owner���� OnDead�ڷ�ƾ ���� = ���ʵ� �ı�
    }

    //������Ʈ
    public void Update()
    {
        //��ǻ� �ʿ����
    }

    //����
    public void Exit()
    {
        //��ǻ� �ʿ����
    }
}
