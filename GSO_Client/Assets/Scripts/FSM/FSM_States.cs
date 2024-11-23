using UnityEngine;

public class StateBase : IState
{
    public FSM_Enemy Owner;

    protected float waitTimer = 0f; // ��� Ÿ�̸�
    protected bool isWaiting = false; // ��� ���� ����
    protected bool isMoveDone = false;
    public StateBase(FSM_Enemy owner)
    {
        Owner = owner;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void Exit()
    {

    }

    protected virtual void StartWait(float duration)
    {
        waitTimer = duration;
        isWaiting = true;
    }
}
public class IdleState : StateBase
{
    private Vector2 targetPos; 

    public IdleState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    // �ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("IdleState");
        Owner.curState = MobState.Idle;
        SetNewTarget();
        waitTimer = 0f;
        isWaiting = false;
    }

    public override void Update()
    {
        if (isWaiting)
        {
            //�����ϸ� 5�ʵ��� ���ڸ����� ���
            waitTimer += Time.deltaTime;
            if (waitTimer >= 5f)
            {
                SetNewPositionWhenTimerDone(); //���ð��� ������ ���ο� ��ġ�� ���ϰ� �ٽ� �̵���
            }
        }
        else
        {
            //�ش� ��ġ�� ���� �ӵ��� �̵�
            float dist = Vector3.Distance(targetPos, Owner.transform.position);
            Owner.MoveToTarget(targetPos, Owner.lowSpeed);

            if (dist < 0.1f) // Ÿ�ٿ� �������� ��
            {
                isWaiting = true; // ��� ���·� ��ȯ
                waitTimer = 0f;   // Ÿ�̸� �ʱ�ȭ
            }
        }
    }

    private void SetNewPositionWhenTimerDone()
    {
        SetNewTarget();
        isWaiting = false;
    }

    // ����
    public override void Exit()
    {
        // ���°� ����� �� �ʿ��� ���� �۾�
    }

    private void SetNewTarget()
    {
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
        Debug.Log($"New Target Position: {targetPos}");
    }
}

public class CheckState : StateBase
{
    private Vector2 targetPos;
    public CheckState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("CheckState");
        Owner.curState = MobState.Check;
        targetPos = Owner.target.transform.position; //Ÿ���� ������ ��ġ�� �̵�
        isWaiting = false;
        isMoveDone = false;
    }

    public override void Update()
    {
        // 1. �̵� ���̰ų� ��� ���̶��, �̵��� ����ϰų� ��⸦ ����
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // ��� ����

                Debug.Log("��Ⱑ �������ϴ�. �̵� �Ǵ� ��ȯ ó�� ����.");
                if (Owner.target == null)
                {
                    Debug.Log("Ÿ���� ��ħ. �̵� �Ϸ� �� ��ȯ ���·� ��ȯ.");
                    Owner._state.ChangeState(Owner._return);
                    return;
                }
                if (Vector3.Distance(Owner.target.transform.position, Owner.transform.position) <= Owner.detectionRange)
                {
                    Debug.Log("Ÿ���� ���� ������ ����. ���ο� Ÿ�� ��ġ ����.");
                    targetPos = Owner.target.transform.position; // ���ο� Ÿ�� ��ġ�� ������Ʈ
                    return;
                }
            }
            return;
        }

        Owner.MoveToTarget(targetPos, Owner.midSpeed);
        float distanceFromTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
        if (Owner.target != null)
        {
            float distanceToPlayer = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
            if (distanceToPlayer <= Owner.chaseRange)
            {
                Debug.Log("Ÿ���� �߰� ������ ����. �߰� ���·� ��ȯ.");
                Owner._state.ChangeState(Owner._chase);
                return;
            }
        }


        if (distanceFromTargetPos <= 0.1f)
        {
            Debug.Log("Ÿ�� ��ġ�� ����. ��� ����.");
            StartWait(3f); // 3�� ���
            return;
        }
    }



    //����
    public override void Exit()
    {

    }

}

public class ChaseState : StateBase
{
    public ChaseState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    public override void Enter()
    {
        Debug.Log("ChaseState");
        Owner.curState = MobState.Chase;

    }

    //�ʱ�ȭ
    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // ��� ����

                if (Owner.target == null)
                {
                    Owner._state.ChangeState(Owner._return);
                    return;
                }
                else
                {
                    float dis = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
                    if (dis > Owner.chaseRange && dis<= Owner.detectionRange)
                    {
                        Owner._state.ChangeState(Owner._check); // ��� ���·� ��ȯ
                    }
                }
                
            }
            return;
        }

        if (Owner.target == null) //Ÿ���� ������ ������ ������ 3�� ����� ��ȯ
        {
            Debug.Log("Ÿ���� ��ħ. ���� �� 3�� ��� �� ��ȯ ���·� ��ȯ.");
            StartWait(3f);
            return;
        }

        Owner.MoveToTarget(Owner.target.transform.position, Owner.highSpeed);

        float distanceToAttack = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToAttack <= Owner.attackRange)
        {
            Debug.Log("Ÿ���� ���� ������ ���Խ��ϴ�. ���� ���·� ��ȯ.");
            Owner._state.ChangeState(Owner._attack); // ���� ���·� ��ȯ
            return;
        }

        //*** �ٸ� ���(�����ϰ� ���ʵ� ��ȯ) �ʹ� �ٸ�. �� �̵��ϴٰ� ��ȯ
        float distanceToChase = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
        if (distanceToChase > Owner.chaseRange)
        {
            StartWait(1f); //�߰ݹ��� �� �������� �� �ϰ�� 1�ʰ� �� �i�ٰ� �����·� ��ȯ��
            return;
        }
    }

    public override void Exit()
    {

    }

}

public class AttackState : StateBase
{
    public AttackState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("AttackState");
        Owner.curState = MobState.Attack;
    }

    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // ��� ����

                if (Owner.target == null)  //������ Ÿ���� �������� ��ȯ
                {
                    Owner._state.ChangeState(Owner._return);
                    return;
                }

                float distanceToTarget = Vector3.Distance(Owner.target.transform.position, Owner.transform.position);
                //Ÿ���� �ִٸ� Ÿ�ٰ��� �Ÿ��� ���� ���� ����
                if (distanceToTarget <= Owner.attackRange)
                {
                    //�ʱ�ȭ �� �����
                    Debug.Log("�����");
                    StartWait(1f);
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
            return;
        }

        //���� �� ������
        StartWait(1f);
    }

    //����
    public override void Exit()
    {

    }

}


public class ReturnState : StateBase
{
    private Vector2 targetPos;  // ���� �� ���� ������ ��ġ
    public ReturnState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("ReturnState");
        Owner.curState = MobState.Return;
        targetPos = Owner.GetRandomPosInSpawnZone(Owner.spawnPoint, Owner.spawnerDistance);
    }
    public override void Update()
    {
        Owner.MoveToTarget(targetPos, Owner.midSpeed);

        float distanceToTargetPos = Vector3.Distance(targetPos, Owner.transform.position);
        Debug.Log(distanceToTargetPos);
        if (distanceToTargetPos <= 0.1f)
        {
            Owner._state.ChangeState(Owner._idle);
            return;
        }
    }

    //����
    public override void Exit()
    {

    }
}
public class StunState : StateBase
{
    public float stunTime;

    public StunState(FSM_Enemy owner) : base (owner) 
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("StunState");
        Owner.curState = MobState.Stun;
    }


    public override void Update()
    {
        /*
         * ������ ������ Ÿ���� �����ϴ��� Ȯ���� ������ �߰� ������ ��ȯ
         */
    }

    //����
    public override void Exit()
    {

    }
}

public class DeadState : StateBase
{
    public DeadState(FSM_Enemy owner) : base(owner)
    {
        Owner = owner;
    }

    //�ʱ�ȭ
    public override void Enter()
    {
        Debug.Log("DeadState");
        Owner.curState = MobState.Dead;
        StartWait(Owner.disappearTime); //3�ʵ� �����
    }

    //������Ʈ
    public override void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false; // ��� ����
                //���⿡ ������Ʈ ���� �ڵ� ����
            }
            return;
        }
    }

    //����
    public override void Exit()
    {
        //��ǻ� �ʿ����
    }
}
