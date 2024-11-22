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

        lowSpeed = 3f;
        midSpeed = 5f;
        highSpeed = 7f;
        detectionRange = 4.5f; //���� �Ÿ� , ���������� ũ�� = *2 . Ʈ���ŷ� �������� �ȿ� ���� ���� ������ �ش� �������� �߰��ӵ� �̵�. �������������� ������ 3�ʵ� ��ȯ
        chaseRange = 3;        //�߰� �Ÿ� , �����ȿ� Ÿ���� ������ Ÿ���� ���� ������ �̵�, �ش� ���� ������ ������ 3�ʵ� �ǽ����� ��ȯ
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

    
    public void OnMoveToPos(Vector2 pos, float _speed)
    {
        transform.position = pos * _speed * Time.deltaTime;
    }
}
