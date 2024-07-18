using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private BulletStat _bulletStat;

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

        //�Ÿ��� range�� �ʰ��ϸ� �Ѿ� �ı�
        if (distance >= _range)
        {
            Debug.Log($"�Ÿ������� �ı�. �Ÿ� = { _range }");
            Destroy(gameObject); //������Ʈ Ǯ�� Ȱ��?
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�� Ȥ�� �� ������Ʈ�� �浹�� �ı�
        if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy")){
            //hit��Ŷ ����

            Debug.Log($"{collision.gameObject.name} ���� {_damage }�� �������� ��"); //�浹ü�� ���ݿ��� hp���ҷ� ��ü
            Destroy(gameObject); //������Ʈ Ǯ�� Ȱ��?
        }
    }
}
