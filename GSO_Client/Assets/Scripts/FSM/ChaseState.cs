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

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("ChaseState");
        Owner.curState = MobState.Chase;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        /*
         * �÷��̾ �ǽɰŸ����� �־��� or ������ҿ��� �ʹ� �־��� => ��ȯ
         * �÷��̾ ���ݰŸ� ������ ���� => ����
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

    //����
    public void Exit()
    {

    }
}
