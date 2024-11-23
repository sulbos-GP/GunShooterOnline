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
    [Header("사용할 상태 입력")]
    public IdleState _idle;
    public CheckState _check;
    public ChaseState _chase;
    public AttackState _attack;
    public ReturnState _return;
    public StunState _stun;


    [Header("스폰위치")]
    public Transform spawnPoint;
    public float maxDistance; //스폰지점과 10만큼 떨어지면 귀환
    public float spawnerDistance;

    [Header("스텟")]
    public float lowSpeed;
    public float midSpeed ;
    public float highSpeed;
    public float detectionRange; //감지 거리 , 감지범위의 크기 = *2 . 트리거로 감지범위 안에 들어온 적이 있으면 해당 방향으로 중간속도 이동. 감지범위밖으로 나가고 3초뒤 귀환
    public float chaseRange;        //추격 거리 , 범위안에 타겟이 들어오면 타겟을 향해 빠르게 이동, 해당 범위 밖으로 나가고 3초뒤 의심으로 전환
    public float attackRange;      //공격 거리 , 범위안에 들어오면 타겟을 향해 공격 실행, 공격이 끝나면 타겟의 거리에 따라 반복 공격, 추격 혹은 의심 혹은 귀환
    public float attackDelay ;      //공격후 잠시 대기하는 시간
    public float disappearTime ; //죽은 적이 3초뒤에 삭제됨
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
        detectionRange = 10f; //감지 거리 , 감지범위의 크기 = *2 . 트리거로 감지범위 안에 들어온 적이 있으면 해당 방향으로 중간속도 이동. 감지범위밖으로 나가고 3초뒤 귀환
        chaseRange = 6;        //추격 거리 , 범위안에 타겟이 들어오면 타겟을 향해 빠르게 이동, 해당 범위 밖으로 나가고 3초뒤 의심으로 전환
        attackRange = 2f;      //공격 거리 , 범위안에 들어오면 타겟을 향해 공격 실행, 공격이 끝나면 타겟의 거리에 따라 반복 공격, 추격 혹은 의심 혹은 귀환
        attackDelay = 2f;      //공격후 잠시 대기하는 시간
        disappearTime = 3f; //죽은 적이 3초뒤에 삭제됨
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
