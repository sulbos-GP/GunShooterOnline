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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("StunState");
        Owner.curState = MobState.Stun;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        /*
         * ������ ������ Ÿ���� �����ϴ��� Ȯ���� ������ �߰� ������ ��ȯ
         */
    }

    //����
    public void Exit()
    {

    }
}
