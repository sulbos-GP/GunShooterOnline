using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Gun���� �����Ҷ� ������Ʈ
    [HideInInspector]
    public int _shooterId;
    [HideInInspector]
    public int _damage;
    [HideInInspector]
    public float _range;
    [HideInInspector]
    public Vector3 _dir;

    //�߻�� ��ġ
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

        //�Ÿ��� range�� �ʰ��ϸ� �Ѿ� �ı�
        if (distance >= _range)
        {
            Debug.Log($"�Ÿ������� �ı�. �Ÿ� = { _range }");
            Destroy(gameObject); //������Ʈ Ǯ�� Ȱ��?
        }
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy(gameObject); //������Ʈ Ǯ�� Ȱ��?
        return;
        //�� Ȥ�� �� ������Ʈ�� �浹�� �ı�
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            //hit��Ŷ ����

            Debug.Log($"{collision.gameObject.name} ���� {_damage}�� �������� ��"); //�浹ü�� ���ݿ��� hp���ҷ� ��ü
            Destroy(gameObject); //������Ʈ Ǯ�� Ȱ��?
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
            return null; // �������� �ʴ� ��� null ��ȯ
        }
    }
}