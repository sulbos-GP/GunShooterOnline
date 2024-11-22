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
   
    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("AttackState");
        Owner.curState = MobState.Attack;
        timer = 0;
    }

    //������Ʈ -> ������ �ൿ + �ٸ� ���·� �Ѿ�� üũ
    public void Update()
    {
        /*
         * ������ �Ϸ������� �÷��̾ �����Ÿ� ������ ������� => �ǽ�
         *                 �÷��̾ �����Ÿ� �ȿ� ���� => �߰� �ݺ�
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
                //�ʱ�ȭ �� �����
                Debug.Log("�����");
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

    //����
    public void Exit()
    {

    }
}
