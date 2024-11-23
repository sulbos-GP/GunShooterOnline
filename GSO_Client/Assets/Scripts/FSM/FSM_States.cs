using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class IdleState : IState
{
    public FSM_Enemy Owner;
    private Vector2 targetPos;
    public IdleState(FSM_Enemy owner)
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
    }

    public void Update()
    {
        //���� �� ���� ��ȸ
        float dist = Vector3.Distance(targetPos, Owner.transform.position);
        Owner.MoveToTarget(targetPos, Owner.lowSpeed);

        if (dist < 0.1f)
        {
            targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        }
    }

    //����
    public void Exit()
    {
        
    }
}


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

        Owner.MoveToTarget(Owner.target.transform.position, Owner.midSpeed);

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

        Owner.MoveToTarget(Owner.target.transform.position, Owner.highSpeed);

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

    public void Update()
    {
        /*
         * ������ �Ϸ������� �÷��̾ �����Ÿ� ������ ������� => �ǽ�
         *                 �÷��̾ �����Ÿ� �ȿ� ���� => �߰� �ݺ�
         */
        timer += Time.deltaTime;
        if (timer > Owner.attackDelay)
        {
            if (Owner.target == null)
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
    public void Update()
    {
        /*
         *  ��ȯ���� ���� ������ => ���
         */
        Owner.MoveToTarget(Owner.spawnPoint.transform.position, Owner.midSpeed);

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
public class StunState : IState
{
    public FSM_Enemy Owner;
    public float stunTime;

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
