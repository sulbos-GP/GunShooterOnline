using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletStat _bulletStat;

    //Gun에서 생성할때 업데이트
    [HideInInspector]
    public int _shooterId;
    [HideInInspector]
    public int _damage;
    [HideInInspector]
    public float _range;
    [HideInInspector]
    public Vector3 _dir;

    //발사된 위치
    private Vector3 _startPos;

    private void Awake()
    {
        if( _bulletStat != null)
        {
            _startPos = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        BulletMove();
    }

    private void BulletMove()
    {
        transform.Translate(_dir.normalized * _bulletStat.speed * Time.deltaTime, Space.World);
        float distance = Vector3.Distance(_startPos, transform.position);

        //거리가 range를 초과하면 총알 파괴
        if (distance >= _range)
        {
            Debug.Log($"거리로인한 파괴. 거리 = { _range }");
            Destroy(gameObject); //오브젝트 풀링 활용?
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //벽 혹은 적 오브젝트와 충돌시 파괴
        if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")){
            //hit패킷 전송

            Debug.Log($"{collision.gameObject.name} 에게 {_damage }의 데미지를 줌"); //충돌체의 스텟에서 hp감소로 교체
            Destroy(gameObject); //오브젝트 풀링 활용?
        }
    }
}
