using MathNet.Numerics.Statistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailShooter : MonoBehaviour
{
    public GameObject projPref;
    public bool Fire = false;
    [Range(0,300)]
    public int projNum = 10;
    [Range(0, 100)]
    public float distance = 20;
    [Range(0, 5)]
    public float spawnTime = 1;

    private void Start()
    {
        // 코루틴을 시작하여 일정한 간격으로 발사합니다.
        StartCoroutine(FireProjectilesRoutine());
    }

    private IEnumerator FireProjectilesRoutine()
    {
        while (true)
        {
            if (!Fire)
            {
                yield return new WaitForSeconds(spawnTime);
                continue;
            }
            FireProjectiles();
            yield return new WaitForSeconds(spawnTime);
        }
    }


    public void FireProjectiles()
    {
        float angleStep = 360f / projNum;
        float angle = 0f;

        for (int i = 0; i < projNum; i++)
        {
            float dirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float dirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector2 projectileDirection = new Vector2(dirX, dirY) - (Vector2)transform.position;
            Vector2 normalizedDir = projectileDirection.normalized;

            Bullet bullet = Instantiate(projPref).GetComponent<Bullet>();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

            bullet.startPos = transform.position;
            bullet.endPos = (Vector2)transform.position + normalizedDir * distance;

            angle += angleStep;
        }
    }
}