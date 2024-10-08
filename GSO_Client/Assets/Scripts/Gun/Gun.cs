using Google.Protobuf.Protocol;
using Org.BouncyCastle.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GunState UsingGunState { get; private set; }

    [SerializeField]
    public Data_master_item_weapon UsingGunData { get; private set; }
    private ItemData usingGunItemData;
    public int curGunEquipSlot; //사용중인 총의 데이터가 없을 경우 0 , 1슬롯 사용시 1 , 2슬롯 사용시 2

    [SerializeField]
    public int CurAmmo 
    {
        get => usingGunItemData.loadedAmmo; 
        set 
        {
            if (usingGunItemData == null)
            {
                return;
            }
            usingGunItemData.loadedAmmo = value;
        }
    } 

    private float _lastFireTime;
    public bool isBulletPrefShoot = true;

    private Transform _fireStartPos;
    public AimLine gunLine;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        curGunEquipSlot = 0;
        //Debug Line
        gunLine = GetComponent<AimLine>();
        gunLine.Init();

        //게임 시작시 실행할 루틴
        _fireStartPos = transform.GetChild(0);
        ResetGun();
    }

    public void SetGunStat(ItemData itemData)
    {
        usingGunItemData = itemData;
        UsingGunData = Data_master_item_weapon.GetData(itemData.itemId);
        UsingGunState = CurAmmo == 0 ? GunState.Empty: GunState.Shootable;

        gunLine.OnAimLine();
        SetGunSprite(itemData.iconName);
    }

    private void SetGunSprite(string iconName)
    {
        Sprite gunSprite = Resources.Load<Sprite>($"Sprite/Item/{iconName}");

        if (gunSprite != null) 
        { 
            GetComponent<SpriteRenderer>().sprite = gunSprite;
            Debug.Log("스프라이트 적용");
        }
        else
        {
            Debug.Log("스프라이트 찾지못함");
        }
       
    }

    public void ResetGun()
    {
        UsingGunData = null;
        usingGunItemData = null;
        CurAmmo = 0;
        UsingGunState = GunState.Empty;
        GetComponent<SpriteRenderer>().sprite = null;
        gunLine.OffAimLine();
        //총 제거 패킷 전송
    }

    public Data_master_item_weapon GetGunStat()
    {
        return UsingGunData;
    }

    private void Update()
    {
        if(UsingGunData == null) return;
        SetAimLine();
    }

    private void SetAimLine()
    {
        //발사범위 선 2개 긋기
        if (_fireStartPos == null) return;

        float halfAngle = UsingGunData.attack_range * 0.5f;
        Vector3 direction1 = Quaternion.Euler(0, 0, halfAngle) * _fireStartPos.up;
        Vector3 endPoint1 = _fireStartPos.position + direction1 * UsingGunData.distance;
        Vector3 direction2 = Quaternion.Euler(0, 0, -halfAngle) * _fireStartPos.up;
        Vector3 endPoint2 = _fireStartPos.position + direction2 * UsingGunData.distance;
        gunLine.SetAimLine(_fireStartPos.position, endPoint1, endPoint2);
    }

    //발사버튼 누를시
    public bool Fire()
    {
        if (UsingGunData == null) 
        {
            Debug.Log("현재 총을 들고 있지 않음");
            return false;
        }
        

        if(UsingGunState == GunState.Shootable && Time.time >= _lastFireTime + UsingGunData.attack_speed )
        {
            /*
             발사 코드 작성.
             총알을 발사하든 레이케스트로 충돌감지를 하든
             적중시 패킷을 서버에게 전달
             */

            //정규분포를 사용한 발사
            float halfAccuracyRange = UsingGunData.attack_range / 2f;

            float meanAngle = 0f;  // 발사 각도의 평균 (중앙)
            float standardDeviation = halfAccuracyRange / 3f;  // 발사 각도의 표준편차 (정확도 기반)
            float randomAngle = GetRandomNormalDistribution(meanAngle, standardDeviation);
            Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * _fireStartPos.up;

            //레이캐스트를 사용한 방법
            //10.06 박성훈 : 레이케스트가 자신의 플레이어의 콜라이더에 막히는 문제가 발생
            // -> 모든 히트들을 받은후 히트가 자신의 플레이어면 그다음으로 넘기고 아니면 hit에 해당 히트를 넣음(나중에 관통 총알 등을 넣기에도 용이)
            RaycastHit2D[] hits = Physics2D.RaycastAll(_fireStartPos.position, direction, UsingGunData.distance);
            RaycastHit2D hit = default;
            foreach (RaycastHit2D _hit in hits)
            {
                if (_hit.collider.gameObject != Managers.Object.MyPlayer.gameObject)
                {
                    Debug.Log($"hit Object : {_hit.collider.gameObject.name}");
                    hit = _hit;
                    break; 
                }
            }
           
            if (hit.collider != null )
            {
                //도중에 충돌할 경우 충돌 위치까지 LineRenderer 설정
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
            

            if (isBulletPrefShoot)
            {
                //총알을 사용한 방법
                Bullet bullet = Managers.Resource.Instantiate($"Objects/BulletObjPref/{UsingGunData.bullet}").GetComponent<Bullet>();
                if (bullet == null)
                {
                    Debug.Log("리소스에서 총알 로드 실패");
                    return false;
                }

                if (hit.point != new Vector2(0, 0))
                    bullet.EndPos = hit.point;
                else
                    bullet.EndPos = _fireStartPos.position + direction * UsingGunData.distance;
                bullet._damage = UsingGunData.damage;
                bullet._range = UsingGunData.distance;
                bullet._dir = direction;
                bullet.transform.position = _fireStartPos.position;
                bullet.transform.rotation = _fireStartPos.rotation;
            }

            _lastFireTime = Time.time;//마지막 사격 시간 업데이트
            CurAmmo--; //현재 총알감소
            CurAmmo = Mathf.Max(CurAmmo, 0);
            if(CurAmmo == 0)
            {
                UsingGunState = GunState.Empty;
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
        if(CurAmmo < UsingGunData.reload_round || UsingGunState != GunState.Reloading)
        {
            //인벤에 해당 총알이 있는지 검색. -> 있다면 최대 장전량 만큼 있는지 확인. -> 그이상이 있다면 최대개수로 아니라면 해당 개수만큼 재장전
            StartCoroutine(ReloadCoroutine(UsingGunData.reload_round));//현재는 임시로 최대 개수
        }
    }

    //실질적인 재장전
    private IEnumerator ReloadCoroutine(int reloadAmount)
    {
        UsingGunState = GunState.Reloading;
        yield return new WaitForSeconds(UsingGunData.reload_time);

        CurAmmo = reloadAmount;
        UsingGunState = GunState.Shootable;

        //(TODO) 인벤에 총알의 양을 감소시킴
    }

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return UsingGunData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
    }
}
