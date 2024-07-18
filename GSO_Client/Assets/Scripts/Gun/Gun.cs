using System.Collections;
using System.IO.Pipes;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public enum GunState
    {
        Shootable,
        Empty,
        Reloading
    }
    public GunState gunState { get; private set; }
    public LineRenderer lineRenderer;

    [SerializeField]
    private GunStat _gunStat;

    [SerializeField]
    private int _shooterId; //��� ĳ������ id Ȥ�� �÷��̾��� id
    [SerializeField]
    private int _curAmmo; //���� ��ź
    [SerializeField]
    private int _reloadTime = 3; //���� ����

    private bool _isAmmoEmpty; //���� ��ź�� �������.
    private float _lastFireTime;
    private Transform fireStartPos;

    public bool isBulletPrefShoot = false;


    private void Awake()
    {
        _gunStat.PrintGunStatInfo();
        Init();
    }
    
    private void Init()
    {
        //���� ���۽� ������ ��ƾ
        _shooterId = 1; //���� ����. ĳ���ͳ� �÷��̾� id �����ϱ�
        _curAmmo = _gunStat.ammo; 
        gunState = GunState.Shootable;
        fireStartPos = transform.GetChild(0);
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer != null )
        {
            lineRenderer.positionCount = 5;
        }
    }

    private void Update()
    {
        SetFireLine();
    }

    private void SetFireLine()
    {
        //�߻���� �� 2�� �߱�
        if (fireStartPos == null) return;

        float halfAngle = _gunStat.accuracy * 0.5f;

        Vector3 direction1 = Quaternion.Euler(0, 0, halfAngle) * fireStartPos.up;
        //RaycastHit2D hit1 = Physics2D.Raycast(fireStartPos.position, direction1, _gunStat.range);
        Vector3 endPoint1 = fireStartPos.position + direction1 * _gunStat.range;

        /*if (hit1.collider != null) //�� ���ؼ��� ���� ������?
        {
            endPoint1 = hit1.point;
        }*/

        Vector3 direction2 = Quaternion.Euler(0, 0, -halfAngle) * fireStartPos.up;
        //RaycastHit2D hit2 = Physics2D.Raycast(fireStartPos.position, direction2, _gunStat.range);
        Vector3 endPoint2 = fireStartPos.position + direction2 * _gunStat.range;
        /*if (hit2.collider != null)
        {
            endPoint2 = hit2.point;
        }*/

        lineRenderer.SetPosition(0, fireStartPos.position);
        lineRenderer.SetPosition(1, endPoint1);
        lineRenderer.SetPosition(2, fireStartPos.position);
        lineRenderer.SetPosition(3, endPoint2);
        lineRenderer.SetPosition(4, fireStartPos.position);

    }

    //�߻��ư ������
    public bool Fire()
    {
        if(gunState == GunState.Shootable && Time.time >= _lastFireTime + 1/ _gunStat.fireRate )
        {
            Debug.Log("FireSuccess");
            /*
             �߻� �ڵ� �ۼ�.
             �Ѿ��� �߻��ϵ� �����ɽ�Ʈ�� �浹������ �ϵ�
             ���߽� ��Ŷ�� �������� ����
             */

            /* �׳� �������� �߻�
            float halfAngle = _gunStat.accuracy * 0.5f;
            float randomAngle = Random.Range(-halfAngle, halfAngle); // accuracy ���� �� ���� ����
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * fireStartPos.up;
            */

            //���Ժ����� ����� �߻�
            float halfAccuracyRange = _gunStat.accuracy / 2f;

            float meanAngle = 0f;  // �߻� ������ ��� (�߾�)
            float standardDeviation = halfAccuracyRange / 3f;  // �߻� ������ ǥ������ (��Ȯ�� ���)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * fireStartPos.up;

            //����ĳ��Ʈ�� ����� ���
            RaycastHit2D hit = Physics2D.Raycast(fireStartPos.position, direction, _gunStat.range);
            if (hit.collider != null)
            {
                //��Ŷ ����
                Debug.Log("Hit: " + hit.collider.name);
            }
            Debug.DrawRay(fireStartPos.position, direction * _gunStat.range, Color.yellow, 0.5f);

            if (isBulletPrefShoot)
            {
                //�Ѿ��� ����� ���
                Bullet bullet = _gunStat.bulletObj.GetComponent<Bullet>();
                bullet._shooterId = _shooterId;
                bullet._damage = _gunStat.damage;
                bullet._range = _gunStat.range;
                bullet._dir = direction;
                Instantiate(bullet, fireStartPos.position, fireStartPos.rotation);

            }

            _lastFireTime = Time.time;//������ ��� �ð� ������Ʈ

            _curAmmo--; //���� �Ѿ˰���
            _curAmmo = Mathf.Max(_curAmmo, 0);
            if(_curAmmo == 0 )
            {
                gunState = GunState.Empty;
            }

            return true;
        }
        Debug.Log("FireFailed");
        return false; //�߻� ���� ����
    }

    public float GetRandomNormalDistribution(float mean, float standard)   
    {
        // ���� ������ ���� �������� �������� �Լ�
        float x1 = Random.Range(0f, 1f);
        float x2 = Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2);
        float randNormal = mean + standard * randStdNormal; //��� + ǥ������* �������Ժ���
        return randNormal;
    }


    //������ ��ư ������
    public void Reload()
    {
        if(_curAmmo < _gunStat.ammo || gunState != GunState.Reloading)
        {
            Debug.Log("Reload");
            StartCoroutine(Reloading());
        }
    }

    //�������� ������
    private IEnumerator Reloading()
    {
        gunState = GunState.Reloading;
        yield return new WaitForSeconds(_reloadTime);

        _curAmmo = _gunStat.ammo;
        gunState = GunState.Shootable;
        Debug.Log("Reload Complete");
    }

    //FovPlayer�� �ڷ�ƾ���� ���
    public float GetFireRate()
    {
        return _gunStat.fireRate; // GunStat Ŭ�������� ������ �߻� �ӵ��� ��ȯ
    }
}
