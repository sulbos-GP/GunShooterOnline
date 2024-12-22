using Google.Protobuf.Protocol;
using Org.BouncyCastle.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GunState gunState { get; private set; } //현재 총의 상태 : shootable, empty, reload

    [SerializeField]
    public Data_master_item_weapon WeaponData { get; private set; } //현재 사용중인 총의 스텟 데이터
    private ItemData itemData; //사용중인 총의 아이템 데이터(총의 오브젝트 아이디, 총알 참조)
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
        get => itemData.loadedAmmo; 
        set 
        {
            if (itemData == null)
            {
                return;
            }
            itemData.loadedAmmo = value;
        }
    } 

    public void Init()
    {
        curGunEquipSlot = 0;

        //Debug Line
        gunLine = GetComponent<AimLine>();
        if(gunLine != null)
        {
            gunLine.Init();

            //게임 시작시 실행할 루틴
            _fireStartPos = transform.GetChild(0);
            _fireParticlePos = _fireStartPos.GetChild(0);
            _animator = _fireParticlePos.GetComponent<Animator>();

        }

        ResetGun();
    }

    public void SetGunStat(ItemData _itemData)
    {
        itemData = _itemData;
        WeaponData = Data_master_item_weapon.GetData(itemData.itemId);
        gunState = CurAmmo == 0 ? GunState.Empty: GunState.Shootable;

        gunLine.OnAimLine();
        SetGunSprite(itemData.iconName);
        UIManager.Instance.SetAmmoText();
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

    /// <summary>
    /// 나중에 추가
    /// </summary>
    public void SetGunRoation(Vector2 dir)
    {
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //var gunTrn = transform.GetChild(0);
        //transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));


        //Debug.Log("Dir" + angle);

        C_InputData inputPacket = new C_InputData();
        inputPacket.GunRoation = new GunAppearacneInfo() { Roation = angle };
        Managers.Network.Send(inputPacket);

    }


    public void GunRoationHandle(float angle)
    {
        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

    }


    public void ResetGun()
    {
        WeaponData = null;
        itemData = null;
        CurAmmo = 0;
        gunState = GunState.Empty;
        GetComponent<SpriteRenderer>().sprite = null;
        gunLine?.OffAimLine();
        UIManager.Instance.SetAmmoText();
    }

    public Data_master_item_weapon GetGunStat()
    {
        return WeaponData;
    }

    private void Update()
    {
        if(WeaponData == null) return;
        SetAimLine();
    }

    private void SetAimLine()
    {
        //발사범위 선 2개 긋기
        if (_fireStartPos == null) return;

        float halfAngle = WeaponData.attack_range * 0.5f;
        Vector3 direction1 = Quaternion.Euler(0, 0, halfAngle) * _fireStartPos.up;
        Vector3 endPoint1 = _fireStartPos.position + direction1 * WeaponData.distance;
        Vector3 direction2 = Quaternion.Euler(0, 0, -halfAngle) * _fireStartPos.up;
        Vector3 endPoint2 = _fireStartPos.position + direction2 * WeaponData.distance;
        gunLine.SetAimLine(_fireStartPos.position, endPoint1, endPoint2);
    }

    //발사버튼 누를시
    public void Fire(Vector2 dir)
    {
        //#region 임시 무한
        //var cRay2 = new C_RaycastShoot //서버에서 이패킷을 받는다면 해당 아이템데이터의 총알을 감소
        //{
        //    StartPosX = _fireStartPos.position.x,
        //    StartPosY = _fireStartPos.position.y,
        //    DirX = dir.x,
        //    DirY = dir.y,
        //};

        //Managers.Network.Send(cRay2);
        //#endregion


        if (WeaponData  == null) 
        {
            Debug.Log("현재 총을 들고 있지 않음");
            return;
        }
        

        if(gunState == GunState.Shootable)
        {
            var cRay = new C_RaycastShoot //서버에서 이패킷을 받는다면 해당 아이템데이터의 총알을 감소
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
        if(WeaponData == null)
        {
            Managers.SystemLog.Message("총을 들고 있지 않음");
            return;
        }

        if (CurAmmo < WeaponData.reload_round || gunState != GunState.Reloading)
        {
            StartCoroutine(ReloadCoroutine());//현재는 임시로 최대 개수
        }
    }

    //실질적인 재장전
    private IEnumerator ReloadCoroutine()
    {
        //재장전 패킷 전송
        C_InputData packet = new C_InputData();
        packet.Reload = true;
        Managers.Network.Send(packet);

        gunState = GunState.Reloading;
        UIManager.Instance.SetActiveReloadBtn(false);
        UIManager.Instance.SetAmmoText();

        Image delayImage = UIManager.Instance.ReloadBtn.transform.GetChild(1).GetComponent<Image>();

        delayImage.fillAmount = 1;
        float elapseTime = 0f;

        int reloadTime = WeaponData.reload_time;
        while (elapseTime < reloadTime)
        {
            if(WeaponData == null)
            {
                StopCoroutine(ReloadCoroutine());
            }
            float remainingTimeRatio = 1 - (elapseTime / reloadTime);
            delayImage.fillAmount = remainingTimeRatio;

            // 경과 시간 업데이트
            elapseTime += Time.deltaTime;
            yield return null;
        }
        AudioManager.instance.PlaySound("Reload",gameObject.GetComponent<AudioSource>());
    }

    public void ReloadDone(int reloadAmount)
    {
        CurAmmo = reloadAmount;
        gunState = GunState.Shootable;
        UIManager.Instance.SetActiveReloadBtn(true);
        UIManager.Instance.SetAmmoText();
    }

    

    //FovPlayer의 코루틴에서 사용
    public float GetFireRate()
    {
        return WeaponData.attack_speed; // GunStat 클래스에서 설정한 발사 속도를 반환
    }

    public void StartEffect()
    {
        audioSource.PlayOneShot(audioClips[currentIndex],0.7f);
        currentIndex = (currentIndex + 1) % audioClips.Length;
        _animator.SetTrigger("Fire");
    }
}
