using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : CreatureController
{


    public float DetectRange;
    public float ChaseRange ;
    public float AttackRange2;
    public Vector2Int SpawnZone;

    public void SetData(S_AiSpawn s_AiSpawn)
    {
        Debug.Log("SetData" + s_AiSpawn);
        DetectRange = s_AiSpawn.DetectRange;
        ChaseRange = s_AiSpawn.ChaseRange;
        AttackRange2 = s_AiSpawn.AttackRange;
        SpawnZone = new Vector2Int(s_AiSpawn.SpawnZone.X , s_AiSpawn.SpawnZone.Y);

    }


    private Coroutine moveCoroutine;
    protected override void Init()
    {
        base.Init();
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


    //public IEnumerator AIMove(List<Vector2IntInfo> moveList)  
    //{ 
    //    foreach(Vector2IntInfo move in moveList)
    //    {
            
    //    }
    //}

    public void MoveToTarget(Vector2 target, float speed)
    {
        if (target == null) return;

        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 directionToTarget = (target - currentPosition).normalized;
        Vector2 newPosition = currentPosition + directionToTarget * speed * Time.deltaTime;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
