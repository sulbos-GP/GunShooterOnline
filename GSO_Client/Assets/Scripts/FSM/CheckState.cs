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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("CheckState");
        Owner.curState = MobState.Check;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        /*
         * Ÿ���� �������� �̵�
         * 
         * 
         * ������ҿ��� �ʹ� �־��� or Ÿ���� �ǽɹ������� ����� => ��ȯ
         * Ÿ���� ��������������ŭ ������� => ����
         */
        if (Owner.target == null) 
        {
            Debug.Log("Ÿ�ٰ� �ʹ� �־��� ��ȯ");
            Owner._state.ChangeState(Owner._return);
            return;
        }

        float distanceFromSpawn = Vector3.Distance(Owner.spawnPoint.position, Owner.transform.position);
        if (distanceFromSpawn > Owner.maxDistance)
        {
            Debug.Log("������ �ʹ� �־��� ��ȯ");
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

    //����
    public void Exit()
    {

    }
}
