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

    public int curGunEquipSlot; //사용중인 총의 데이터가 없을 경우 0 , 1슬롯 사용시 1 , 2슬롯 사용시 2

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
        curGunEquipSlot = 0; 
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
        _curAmmo = 0;
        CurGunState = GunState.Shootable;
        Reload();

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
        

        if(CurGunState == GunState.Shootable && Time.time >= _lastFireTime + CurGunData.attack_speed )
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
        //조건1 현재 총알이 최대개수보다 작아야함.
        //조건2 재장전 중이 아니어야함
        //조건3 인벤에 맞는 총알이 있어야함 (todo)
        if(_curAmmo < CurGunData.reload_round || CurGunState != GunState.Reloading)
        {
            //인벤에 해당 총알이 있는지 검색. -> 있다면 최대 장전량 만큼 있는지 확인. -> 그이상이 있다면 최대개수로 아니라면 해당 개수만큼 재장전
            StartCoroutine(ReloadCoroutine(CurGunData.reload_round));//현재는 임시로 최대 개수
        }
    }

    //실질적인 재장전
    private IEnumerator ReloadCoroutine(int reloadAmount)
    {
        CurGunState = GunState.Reloading;
        yield return new WaitForSeconds(CurGunData.reload_time);

        _curAmmo = reloadAmount;
        CurGunState = GunState.Shootable;

        //(TODO) 인벤에 총알의 양을 감소시킴
    }

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return CurGunData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
    }
}
