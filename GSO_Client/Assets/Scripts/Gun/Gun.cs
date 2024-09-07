using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GunState CurGunState { get; private set; }

    [SerializeField]
    public GunData CurGunData { get; private set; }

    public int curGunEquipSlot;

    [SerializeField]
    public int _curAmmo { get; private set; } //현재 장탄

    private float _lastFireTime;

    public bool isBulletPrefShoot = true;

    //총알궤적 라인 렌더러 (디버깅 라인을 라인렌더러로 표현)
    public LineRenderer bulletLine;
    private LineRenderer rangeLine;
    private Transform _fireStartPos;
    private Vector3 _direction;      // Ray가 향하는 방향

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        curGunEquipSlot = 0; //사용중인 총의 데이터가 없을 경우, 1슬롯 사용시 1 , 2슬롯 사용시 2
        //Debug Line
        bulletLine = GetComponent<LineRenderer>();
        rangeLine = transform.GetChild(0).GetComponent<LineRenderer>();
        bulletLine.positionCount = 2;
        rangeLine.positionCount = 5;

        //게임 시작시 실행할 루틴
        _fireStartPos = transform.GetChild(0);
        ResetGun();
    }

    public void SetGunStat(GunData newGun)
    {
        if(CurGunData == null)
        {
            CurGunData = new GunData(); 
        }
        CurGunData.item_id = newGun.item_id;
        CurGunData.range = newGun.range;
        CurGunData.damage = newGun.damage;
        CurGunData.distance = newGun.distance;
        CurGunData.reload_round = newGun.reload_round;
        CurGunData.attack_speed = newGun.attack_speed;
        CurGunData.reload_time = newGun.reload_time;
        CurGunData.bulletId = newGun.bulletId;

        //총이 정해졌을때 
        _curAmmo = CurGunData.reload_round;
        CurGunState = GunState.Shootable;

        rangeLine.enabled = true;
    }

    public void ResetGun()
    {
        CurGunData = null;
        _curAmmo = 0;
        CurGunState = GunState.Empty;

        rangeLine.enabled = false;
    }

    public GunData getGunStat()
    {
        return CurGunData;
    }



    private void Update()
    {
        if(CurGunData == null) return;
        SetRangeLine();
    }

    private void SetRangeLine()
    {
        //발사범위 선 2개 긋기
        if (_fireStartPos == null) return;

        float halfAngle = CurGunData.range * 0.5f;

        Vector3 direction1 = Quaternion.Euler(0, 0, halfAngle) * _fireStartPos.up;
        Vector3 endPoint1 = _fireStartPos.position + direction1 * CurGunData.distance;


        Vector3 direction2 = Quaternion.Euler(0, 0, -halfAngle) * _fireStartPos.up;
        Vector3 endPoint2 = _fireStartPos.position + direction2 * CurGunData.distance;
        

        rangeLine.SetPosition(0, _fireStartPos.position);
        rangeLine.SetPosition(1, endPoint1);
        rangeLine.SetPosition(2, _fireStartPos.position);
        rangeLine.SetPosition(3, endPoint2);
        rangeLine.SetPosition(4, _fireStartPos.position);
    }

    //발사버튼 누를시
    public bool Fire()
    {
        if (CurGunData == null) 
        {
            Debug.Log("현재 총을 들고 있지 않음");
            return false;
        }
        

        if(CurGunState == GunState.Shootable && Time.time >= _lastFireTime + 8/ CurGunData.attack_speed )
        {
            /*
             발사 코드 작성.
             총알을 발사하든 레이케스트로 충돌감지를 하든
             적중시 패킷을 서버에게 전달
             */

            //정규분포를 사용한 발사
            float halfAccuracyRange = CurGunData.range / 2f;

            float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
            float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * _fireStartPos.up;

            //레이캐스트를 사용한 방법
            RaycastHit2D hit = Physics2D.Raycast(_fireStartPos.position, direction, CurGunData.distance);

            if (hit.collider != null)
            {
                // 충돌 위치까지 LineRenderer 설정
                //bulletLine.SetPosition(0, _fireStartPos.position);
                //bulletLine.SetPosition(1, hit.point);

                // 패킷 전송
                var cRay = new C_RaycastShoot
                {
                    StartPosX = _fireStartPos.position.x,
                    StartPosY = _fireStartPos.position.y,
                    DirX = direction.x,
                    DirY = direction.y,
                    Length = Vector3.Distance(_fireStartPos.position, hit.point)  // 실제 충돌 위치까지의 거리
                };
                Managers.Network.Send(cRay);
            }
            else
            {
                // 충돌이 없으면 최대 사거리까지 LineRenderer 설정
                Vector3 endPos = _fireStartPos.position + direction * CurGunData.distance;
                //bulletLine.SetPosition(0, _fireStartPos.position);
                //bulletLine.SetPosition(1, endPos);
            }

            if (isBulletPrefShoot)
            {
                //총알을 사용한 방법
                Bullet bullet = Resources.Load<GameObject>($"Prefabs/{BulletDB.bulletDB[CurGunData.bulletId].bulletObjPath}").GetComponent<Bullet>();
                if(bullet == null)
                {
                    Debug.Log("리소스에서 총알 로드 실패");
                    return false;
                }
                if(hit.point != new Vector2(0,0))
                    bullet.EndPos = hit.point;
                else
                    bullet.EndPos = _fireStartPos.position + direction * CurGunData.distance;
                bullet._damage = CurGunData.damage;
                bullet._range = CurGunData.distance;
                bullet._dir = direction;
                Instantiate(bullet, _fireStartPos.position, _fireStartPos.rotation);
            }
            _lastFireTime = Time.time;//마지막 사격 시간 업데이트

            _curAmmo--; //현재 총알감소
            _curAmmo = Mathf.Max(_curAmmo, 0);
            if(_curAmmo == 0 )
            {
                CurGunState = GunState.Empty;
            }
            return true;
        }
        return false; //발사 성공 여부
    }

    public float GetRandomNormalDistribution(float mean, float standard)   
    {
        // 정규 분포로 부터 랜덤값을 가져오는 함수
        float x1 = Random.Range(0f, 1f);
        float x2 = Random.Range(0f, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2);
        float randNormal = mean + standard * randStdNormal; //평균 + 표준편차* 랜덤정규분포
        return randNormal;
    }


    //재장전 버튼 누를시
    public void Reload()
    {
        if(_curAmmo < CurGunData.reload_round || CurGunState != GunState.Reloading)
        {
            StartCoroutine(Reloading());
        }
    }

    //실질적인 재장전
    private IEnumerator Reloading()
    {
        CurGunState = GunState.Reloading;
        yield return new WaitForSeconds(CurGunData.reload_time);

        _curAmmo = CurGunData.reload_round;
        CurGunState = GunState.Shootable;
    }

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return CurGunData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
    }
}

[System.Serializable]
public class GunData
{
    public int item_id;
    public int range;          // 발사 각도(클수록 정확도 다운)
    public int damage;         // 데미지
    public int distance;       // 사거리
    public int reload_round;   // 재장전 되는 탄알 수
    public int attack_speed;   // 연사 속도
    public int reload_time;    // 재장전 시간 
    public int bulletId;  // 탄알 종류
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

//총 데이터베이스가 나오면 제거
public class GunDB
{
    public static Dictionary<int, GunData> gunDB = new Dictionary<int, GunData>();

    static GunDB()
    {
        GunDataInit();
    }


    private static GunData colt45 = new GunData
    {
        item_id = 101,
        range = 8,
        damage = 10,
        distance = 6,
        reload_round = 8,
        attack_speed = 8,
        reload_time = 2,
        bulletId = 501
    };

    private static GunData ak47 = new GunData
    {
        item_id = 102,
        range = 20,
        damage = 18,
        distance = 10,
        reload_round = 40,
        attack_speed = 20,
        reload_time = 4,
        bulletId = 502
    };

    private static GunData aug = new GunData
    {
        item_id = 103,
        range = 8,
        damage = 10,
        distance = 6,
        reload_round = 8,
        attack_speed = 21,
        reload_time = 4,
        bulletId = 502
    };

    public static void GunDataInit()
    {
        gunDB.Clear();
        gunDB.Add(colt45.item_id, colt45);
        gunDB.Add(ak47.item_id, ak47);
        gunDB.Add(aug.item_id, aug);
    }

    public static GunData GetGunData(int itemId)
    {
        if (gunDB.ContainsKey(itemId))
        {
            return gunDB[itemId];
        }
        else
        {
            return null; // 존재하지 않는 경우 null 반환
        }
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
