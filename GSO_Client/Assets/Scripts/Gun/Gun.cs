using Google.Protobuf.Protocol;
using System.Collections;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GunState gunState { get; private set; }

    public GunData gunData { get; private set; }

    [SerializeField]
    private GunStat _gunStat;
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

    public void SetGunStat(GunStat gunStat)
    {
        gunData = new GunData();
        gunData.range = gunStat.range;
        gunData.ammo = gunStat.ammo;
        gunData.fireRate = gunStat.fireRate;
        gunData.gunType = gunStat.gunType;
        gunData.bulletObj = gunStat.bulletObj;
        gunData.accuracy = gunStat.accuracy;
        gunData.damage = gunStat.damage;
        gunData.reloadTime = gunStat.reloadTime;
    }

    public GunData getGunStat()
    {
        return gunData;
    }
    
    private void Init()
    {
        //Debug Line
        bulletLine = GetComponent<LineRenderer>();
        rangeLine = transform.GetChild(0).GetComponent<LineRenderer>();
        bulletLine.positionCount = 2;
        rangeLine.positionCount = 5;

        //게임 시작시 실행할 루틴
        SetGunStat(_gunStat);
        _curAmmo = gunData.ammo; 
        gunState = GunState.Shootable;
        _fireStartPos = transform.GetChild(0);

        
    }

    private void Update()
    {
        SetFireLine();
    }

    private void SetFireLine()
    {
        //발사범위 선 2개 긋기
        if (_fireStartPos == null) return;

        float halfAngle = gunData.accuracy * 0.5f;

        Vector3 direction1 = Quaternion.Euler(0, 0, halfAngle) * _fireStartPos.up;
        Vector3 endPoint1 = _fireStartPos.position + direction1 * gunData.range;


        Vector3 direction2 = Quaternion.Euler(0, 0, -halfAngle) * _fireStartPos.up;
        Vector3 endPoint2 = _fireStartPos.position + direction2 * gunData.range;
        

        rangeLine.SetPosition(0, _fireStartPos.position);
        rangeLine.SetPosition(1, endPoint1);
        rangeLine.SetPosition(2, _fireStartPos.position);
        rangeLine.SetPosition(3, endPoint2);
        rangeLine.SetPosition(4, _fireStartPos.position);
    }

    //발사버튼 누를시
    public bool Fire()
    {
        if(gunState == GunState.Shootable && Time.time >= _lastFireTime + 1/ gunData.fireRate )
        {
            /*
             발사 코드 작성.
             총알을 발사하든 레이케스트로 충돌감지를 하든
             적중시 패킷을 서버에게 전달
             */

            //정규분포를 사용한 발사
            float halfAccuracyRange = gunData.accuracy / 2f;

            float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
            float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * _fireStartPos.up;

            //레이캐스트를 사용한 방법
            RaycastHit2D hit = Physics2D.Raycast(_fireStartPos.position, direction, gunData.range);

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
                Vector3 endPos = _fireStartPos.position + direction * gunData.range;
                //bulletLine.SetPosition(0, _fireStartPos.position);
                //bulletLine.SetPosition(1, endPos);
            }

            if (isBulletPrefShoot)
            {
                //총알을 사용한 방법
                Bullet bullet = gunData.bulletObj.GetComponent<Bullet>();
                if(hit.point != new Vector2(0,0))
                    bullet.EndPos = hit.point;
                else
                    bullet.EndPos = _fireStartPos.position + direction * gunData.range;
                bullet._damage = gunData.damage;
                bullet._range = gunData.range;
                bullet._dir = direction;
                Instantiate(bullet, _fireStartPos.position, _fireStartPos.rotation);
            }
            _lastFireTime = Time.time;//마지막 사격 시간 업데이트

            _curAmmo--; //현재 총알감소
            _curAmmo = Mathf.Max(_curAmmo, 0);
            if(_curAmmo == 0 )
            {
                gunState = GunState.Empty;
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
        if(_curAmmo < gunData.ammo || gunState != GunState.Reloading)
        {
            StartCoroutine(Reloading());
        }
    }

    //실질적인 재장전
    private IEnumerator Reloading()
    {
        gunState = GunState.Reloading;
        yield return new WaitForSeconds(gunData.reloadTime);

        _curAmmo = gunData.ammo;
        gunState = GunState.Shootable;
    }

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return gunData.fireRate; // GunStat 클래스에서 설정한 발사 속도를 반환
    }
}

[System.Serializable]
public class GunData
{
    public float range;       // 사거리.
    public float fireRate;    // 연사 속도. 값과 속도가 반비례
    public int ammo;          // 장탄수(보유장탄)
    public float accuracy;    // 발사 각도(정확도)
    public int damage;        // 데미지
    public GameObject bulletObj;
    public GunType gunType;
    public int reloadTime;
}
