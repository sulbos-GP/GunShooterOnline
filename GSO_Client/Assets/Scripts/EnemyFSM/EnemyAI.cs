using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Unity.Collections;
using UnityEngine;

public class EnemyAI : CreatureController
{
    public float DetectRange;
    public float ChaseRange ;
    public float AttackRange2;
    public Vector2Int SpawnZone;

    public LineRenderer AttackShape;

    public void SetData(S_AiSpawn s_AiSpawn)
    {
        Debug.Log("SetData" + s_AiSpawn);
        DetectRange = s_AiSpawn.DetectRange;
        ChaseRange = s_AiSpawn.ChaseRange;
        AttackRange2 = s_AiSpawn.AttackRange;
        SpawnZone = new Vector2Int(s_AiSpawn.SpawnZone.X , s_AiSpawn.SpawnZone.Y);

    }

    protected override void Init()
    {
        base.Init();

        animator = GetComponent<Animator>();
        characterSprite = GetComponent<SpriteRenderer>();


        AttackShape = transform.GetChild(0).GetComponent<LineRenderer>();

        AttackShape.positionCount = 5; // 사각형을 만들기 위해 5개의 점이 필요합니다.
        AttackShape.loop = false; // 마지막 점이 처음으로 연결되지 않도록 설정합니다.
        AttackShape.startWidth = 0.1f;
        AttackShape.endWidth = 0.1f;
        AttackShape.startColor = Color.red;
        AttackShape.endColor = Color.red;
    }

    public void DrawAttackLine(Vector2 center,float width, float height)
    {
        Rectangle rect = new Rectangle(width, height);
        AttackShape.positionCount = 5;
        AttackShape.SetPosition(0, center + rect.topLeft);
        AttackShape.SetPosition(1, center + rect.topRight);
        AttackShape.SetPosition(2, center + rect.bottomRight);
        AttackShape.SetPosition(3, center + rect.bottomLeft);
        AttackShape.SetPosition(4, center + rect.topLeft);
        Debug.Log($"공격범위 그리기 완료 center : {center} ");
    }

    public void ClearLine()
    {
        AttackShape.positionCount = 0;
    }

    public override void UpdatePosInfo(PositionInfo info)
    {
        base.UpdatePosInfo(info);
        //이동방향 수정

    }

    public void SetAniamtionAttack()
    {
        animator.SetTrigger("IsAttack");
    }

    public void SetAniamtionDead()
    {
        animator.SetTrigger("IsDead");
    }

    public override void OnDead(int attackerId = -1)
    {
        if (attackerId == -1)
            return;

        SetAniamtionDead();
    }

    

    private void OnDrawGizmos()
    {
        if (SpawnZone == null) // �ʱ�ȭ
            return;

        Vector2 position2D = (Vector2)transform.position;

        // Detect Range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(position2D.x, position2D.y, 0), DetectRange);

        // Chase Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(position2D.x, position2D.y, 0), ChaseRange);

        // Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(position2D.x, position2D.y, 0), AttackRange);

        // Spawn Zone
        Gizmos.color = Color.blue;
        Vector2 zonePosition = new Vector2(position2D.x - SpawnZone.x / 2f, position2D.y - SpawnZone.y / 2f);
        Vector2 zoneSize = new Vector2(SpawnZone.x, SpawnZone.y);
        Gizmos.DrawWireCube(new Vector3(zonePosition.x + zoneSize.x / 2f, zonePosition.y + zoneSize.y / 2f, 0),
                            new Vector3(zoneSize.x, zoneSize.y, 0));
    }


    public void MoveToTarget(Vector2 target, float speed)
    {
        if (target == null) return;

        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 directionToTarget = (target - currentPosition).normalized;
        Vector2 newPosition = currentPosition + directionToTarget * speed * Time.deltaTime;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
