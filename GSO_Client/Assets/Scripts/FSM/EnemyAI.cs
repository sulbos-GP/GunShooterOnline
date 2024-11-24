using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : CreatureController
{

    private Coroutine moveCoroutine;
    protected override void Init()
    {
        base.Init();
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
