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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("ReturnState");
        Owner.curState = MobState.Return;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        /*
         *  ��ȯ���� ���� ������ => ���
         */

        float distanceToSpawner = Vector3.Distance(Owner.spawnPoint.position, Owner.transform.position);
        if (distanceToSpawner <= Owner.spawnerDistance)
        {
            Owner._state.ChangeState(Owner._idle);
            return;
        }
    }

    //����
    public void Exit()
    {

    }
}
