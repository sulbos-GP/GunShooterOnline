using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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

    public Vector2 EndPos;
    public float speed = 80.0f;

    private void Awake()
    {
        //if (_bulletstat != null)
        //{
        //    _startpos = transform.position;
        //}
    }

    private void Start()
    {
        UpdateMove();
    }

    // Update is called once per frame
    void Update()
    {
        //BulletMove();
        UpdateMove();
    }

    public void UpdateMove()
    {
        Vector2 startPos = transform.position;
        transform.position = Vector2.MoveTowards(startPos, EndPos, speed * Time.deltaTime);
        if ((Vector2)transform.position == EndPos)
            Destroy(gameObject);
    }

    /*
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
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy(gameObject); //오브젝트 풀링 활용?
        return;
        //벽 혹은 적 오브젝트와 충돌시 파괴
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            //hit패킷 전송

            Debug.Log($"{collision.gameObject.name} 에게 {_damage}의 데미지를 줌"); //충돌체의 스텟에서 hp감소로 교체
            Destroy(gameObject); //오브젝트 풀링 활용?
        }
    }

    
}

[SerializeField]
public class BulletData
{
    public int item_id;
    public float speed;
    public BulletType bulletType;
    public string bulletObjPath;



    public void PrintBulletStatInfo()
    {
        Debug.Log($" Speed: {speed}, " +
                  $" BulletType: {bulletType}");
    }
}
public class BulletDB
{

    public static Dictionary<int, BulletData> bulletDB = new Dictionary<int, BulletData>();

    static BulletDB()
    {
        BulletDataInit();
    }


    private static BulletData b559mm = new BulletData
    {
        item_id = 501,
        speed = 30,
        bulletType = BulletType.b559mm,
        bulletObjPath = "Objects/BulletObjPref/5.59mm"
    };

    private static BulletData b729mm = new BulletData
    {
        item_id = 502,
        speed = 40,
        bulletType = BulletType.b729mm,
        bulletObjPath = "Objects/BulletObjPref/7.29mm"
    };

    public static void BulletDataInit()
    {
        bulletDB.Clear();
        bulletDB.Add(b559mm.item_id, b559mm);
        bulletDB.Add(b729mm.item_id, b729mm);
    }

    public static BulletData GetBulletData(int itemId)
    {
        if (bulletDB.ContainsKey(itemId))
        {
            return bulletDB[itemId];
        }
        else
        {
            return null; // 존재하지 않는 경우 null 반환
        }
    }
}