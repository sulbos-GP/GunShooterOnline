using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class FSM_Enemy : MonoBehaviour
{
    public StateController _state;
    public MobState curState;
    [Header("����� ���� �Է�")]
    public IdleState _idle;
    public CheckState _check;
    public ChaseState _chase;
    public AttackState _attack;
    public ReturnState _return;
    public StunState _stun;


    [Header("������ġ")]
    public Transform spawnPoint;
    public float maxDistance; //���������� 10��ŭ �������� ��ȯ
    public float spawnerDistance;

    [Header("����")]
    public float lowSpeed;
    public float midSpeed ;
    public float highSpeed;
    public float detectionRange; //���� �Ÿ� , ���������� ũ�� = *2 . Ʈ���ŷ� �������� �ȿ� ���� ���� ������ �ش� �������� �߰��ӵ� �̵�. �������������� ������ 3�ʵ� ��ȯ
    public float chaseRange;        //�߰� �Ÿ� , �����ȿ� Ÿ���� ������ Ÿ���� ���� ������ �̵�, �ش� ���� ������ ������ 3�ʵ� �ǽ����� ��ȯ
    public float attackRange;      //���� �Ÿ� , �����ȿ� ������ Ÿ���� ���� ���� ����, ������ ������ Ÿ���� �Ÿ��� ���� �ݺ� ����, �߰� Ȥ�� �ǽ� Ȥ�� ��ȯ
    public float attackDelay ;      //������ ��� ����ϴ� �ð�
    public float disappearTime ; //���� ���� 3�ʵڿ� ������
    public int maxHP = 100;
    public int currentHP;

    public GameObject target;
    public float targetDistance;

    private void Awake()
    {
        _idle = new IdleState(this);
        _check = new CheckState(this);
        _chase = new ChaseState(this);
        _attack = new AttackState(this);
        _return = new ReturnState(this);
        _stun = new StunState(this);

        _state = new StateController();
    }

    private void Start()
    {
        maxDistance = 10;
        spawnerDistance = 5;

        lowSpeed = 0.5f;
        midSpeed = 1f;
        highSpeed = 2f;
        detectionRange = 10f; //���� �Ÿ� , ���������� ũ�� = *2 . Ʈ���ŷ� �������� �ȿ� ���� ���� ������ �ش� �������� �߰��ӵ� �̵�. �������������� ������ 3�ʵ� ��ȯ
        chaseRange = 6;        //�߰� �Ÿ� , �����ȿ� Ÿ���� ������ Ÿ���� ���� ������ �̵�, �ش� ���� ������ ������ 3�ʵ� �ǽ����� ��ȯ
        attackRange = 2f;      //���� �Ÿ� , �����ȿ� ������ Ÿ���� ���� ���� ����, ������ ������ Ÿ���� �Ÿ��� ���� �ݺ� ����, �߰� Ȥ�� �ǽ� Ȥ�� ��ȯ
        attackDelay = 2f;      //������ ��� ����ϴ� �ð�
        disappearTime = 3f; //���� ���� 3�ʵڿ� ������
        maxHP = 100;

        currentHP = maxHP;
        _state.ChangeState(_idle);
    }

    private void Update()
    {
        if (target != null)
        {
            float targetDis = Vector3.Distance(target.transform.position, transform.position);
            Debug.Log("targetDis : "+targetDis);
        }
        if (curState == MobState.Dead)
        {
            return;
        }

        _state.Update();

        if (currentHP <= 0)
        {
            _state.ChangeState(new DeadState(this));
        }
    }
    
    public void MoveToTarget(Vector2 target, float speed)
    {
        if (target == null) return;

        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 directionToTarget = (target - currentPosition).normalized;
        Vector2 newPosition = currentPosition + directionToTarget * speed * Time.deltaTime;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    public Vector2 GetRandomPosInSpawnZone(Transform center, float radius)
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float distance = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * radius;

        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        return new Vector2(center.position.x + x, center.position.y+y);
    }

    public void EnemyAttack()
    {

    }
}
