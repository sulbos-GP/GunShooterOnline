using Google.Protobuf.Protocol;
using Org.BouncyCastle.Utilities;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GunState gunState { get; private set; } //현재 총의 상태 : shootable, empty, reload

    [SerializeField]
    public Data_master_item_weapon UsingGunData { get; private set; } //현재 사용중인 총의 스텟 데이터
    private ItemData usingGunItemData; //사용중인 총의 아이템 데이터(총의 오브젝트 아이디, 총알 참조)
    private Transform _fireStartPos;
    public AimLine gunLine;
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public int currentIndex=0;
    public float nextTime = 0.0f;
    public int curGunEquipSlot; //사용중인 총의 데이터가 없을 경우 0 , 1슬롯 사용시 1 , 2슬롯 사용시 2
    public AnimationClip aniInfo;

    private Transform _fireParticlePos;
    private Animator _animator;

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
        _fireParticlePos = _fireStartPos.GetChild(0);
        _animator = _fireParticlePos.GetComponent<Animator>();
        ResetGun();
    }

    public void SetGunStat(ItemData itemData)
    {
        usingGunItemData = itemData;
        UsingGunData = Data_master_item_weapon.GetData(itemData.itemId);
        gunState = CurAmmo == 0 ? GunState.Empty: GunState.Shootable;

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
        gunState = GunState.Empty;
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
    public void Fire(Vector2 dir)
    {
        if (UsingGunData  == null) 
        {
            Debug.Log("현재 총을 들고 있지 않음");
            return;
        }
        

        if(gunState == GunState.Shootable)
        {
            var cRay = new C_RaycastShoot
            {
                StartPosX = _fireStartPos.position.x,
                StartPosY = _fireStartPos.position.y,
                DirX = dir.x,
                DirY = dir.y,
            };

            Managers.Network.Send(cRay);
        }
    }

    public void UseAmmo()
    {
        CurAmmo--; //현재 총알감소
        CurAmmo = Mathf.Max(CurAmmo, 0);
        if (CurAmmo == 0)
        {
            gunState = GunState.Empty;
        }
    }

    //재장전 버튼 누를시
    public void Reload()
    {
        //조건1 현재 총알이 최대개수보다 작아야함.
        //조건2 재장전 중이 아니어야함
        //조건3 인벤에 맞는 총알이 있어야함 (todo)
        if(UsingGunData == null)
        {
            Managers.SystemLog.Message("총을 들고 있지 않음");
            return;
        }
        if(CurAmmo < UsingGunData.reload_round || gunState != GunState.Reloading)
        {
            //인벤에 해당 총알이 있는지 검색. -> 있다면 최대 장전량 만큼 있는지 확인. -> 그이상이 있다면 최대개수로 아니라면 해당 개수만큼 재장전
            
            StartCoroutine(ReloadCoroutine(UsingGunData.reload_round));//현재는 임시로 최대 개수
        }
    }

    //실질적인 재장전
    private IEnumerator ReloadCoroutine(int reloadAmount)
    {
        //재장전 패킷 전송
        C_InputData packet = new C_InputData();
        packet.Reload = true;
        Managers.Network.Send(packet);

        gunState = GunState.Reloading;
        UIManager.Instance.SetActiveReloadBtn(false);
        UIManager.Instance.SetAmmoText();
        yield return new WaitForSeconds(UsingGunData.reload_time);

        CurAmmo = reloadAmount;
        gunState = GunState.Shootable;
        UIManager.Instance.SetActiveReloadBtn(true);
        UIManager.Instance.SetAmmoText();

        //(TODO) 인벤에 총알의 양을 감소시킴
    }

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return UsingGunData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
    }

    public void StartEffect()
    {
        audioSource.PlayOneShot(audioClips[currentIndex],0.7f);
        currentIndex = (currentIndex + 1) % audioClips.Length;
        _animator.SetTrigger("Fire");
    }
}
