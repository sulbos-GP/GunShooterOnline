using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    //�߻�� ��ġ
    public Vector2 startPos;
    public Vector2 endPos;
    public float speed = 100.0f;

    private void Start()
    {
        transform.position = startPos;
        UpdateMove();
    }

    void Update()
    {
        UpdateMove();
    }

    public void UpdateMove()
    {
        transform.position = Vector2.MoveTowards(transform.position, endPos, speed * Time.deltaTime);

        if ((Vector2)transform.position == endPos)
            Destroy(gameObject);
    }
}
