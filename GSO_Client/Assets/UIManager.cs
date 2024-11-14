using Google.Protobuf.Protocol;
using MathNet.Numerics.Providers.SparseSolver;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DieUI { get; private set; }
    public UI_Quest QuestUI { get; private set; }

    public TMP_Text HealthText { get; private set; }
    public Image CureImage { get; private set; }

    public TMP_Text AmmoText { get; private set; }
    public TMP_Text TimeText { get; private set; }
     
    public Button ReloadBtn { get; private set; }
    public Button InteractBtn { get; private set; }
    public Button OptionBtn { get; private set; }
    public Button InventoryBtn { get; private set; }

    public Transform WQuickSlot { get; private set; }
    public Button MainWeaponBtn { get; private set; }
    public Button SubWeaponBtn { get; private set; }

    public Transform IQuickSlot { get; private set; }


    public float Duration = 2.0f;
    private bool isDie;
    private bool _init;
    private float _alphaTime = 3f;

    
    private MyPlayerController myPlayer => Managers.Object.MyPlayer;

    

    private void Awake() 
    {
        Instance = this;
        Init();
    }

    void Start()
    {
        isDie = false;
        _init = false;
    }


    void Update()
    {
        if (Managers.Object.MyPlayer == null || isDie)
            return;

        if(!_init)
            Init();

        if(myPlayer.Hp<=0)
        {
            isDie = true;
        }

    }

    public void DieMessage(string attackerName)
    {
        DieUI.SetActive(true);
        DieUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text += attackerName;
        StartCoroutine(TextAlpha(DieUI.GetComponent<CanvasGroup>(), Duration));
    }

    private void Init()
    {
        _init = true;

        foreach (Transform child in transform)
        {
            switch(child.name)
            {
                case "Quests":
                    QuestUI = child.GetComponentInChildren<UI_Quest>();
                    break;
                case "GunAmmo":
                    AmmoText = child.GetComponentInChildren<TMP_Text>();
                    break;
                case "TimeUI":
                    TimeText = child.GetComponentInChildren<TMP_Text>();
                    break;
                case "Health":
                    HealthText = child.GetComponentInChildren<TMP_Text>();
                    CureImage = child.GetChild(2).GetComponent<Image>();
                    break;
                case "InteractBtn":
                    InteractBtn = child.GetComponent<Button>();
                    break;
                case "ReloadBtn":
                    ReloadBtn = child.GetComponent<Button>();
                    break;
                case "OptionBtn":
                    OptionBtn = child.GetComponent<Button>();
                    break;
                case "InventoryBtn":
                    InventoryBtn = child.GetComponent<Button>();
                    break;
                case "DieUI":
                    child.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Killer : ";
                    break;
                case "IQuickSlot":
                    IQuickSlot = child;
                    break;
                case "WQuickSlot":
                    WQuickSlot = child;
                    MainWeaponBtn = child.GetChild(0).GetComponent<Button>();
                    SubWeaponBtn = child.GetChild(1).GetComponent<Button>();
                    break;
            }
        }
    }

    
    private IEnumerator TextAlpha(CanvasGroup group,float duration)
    {
        float elapsedTime = 0f;

        while(elapsedTime<duration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return null;
        }

        group.alpha = 1f;
    }

    public void SetActiveHealImage(bool tf)
    {
        CureImage.gameObject.SetActive(tf);
    }

    public void SetHpText()
    {
        float MaxHP = myPlayer.MaxHp;
        float HP = myPlayer.Hp;
        HealthText.text = HP + " / " + MaxHP;
    }

    public void SetAmmoText()
    {
        if (myPlayer.usingGun.gunState == GunState.Reloading)
            AmmoText.text = "ReloadCoroutine Gun";
        else
        {
            if (myPlayer.usingGun.UsingGunData == null)
            {
                AmmoText.text = "Gun is not Equipped";
            }
            else
            {
                AmmoText.text = myPlayer.usingGun.CurAmmo.ToString() + " / " + myPlayer.usingGun.GetGunStat().reload_round.ToString();
            }
        }
    }

    public void SetReloadBtnListener(Gun usingGun)
    {
        ReloadBtn.onClick.RemoveAllListeners();
        ReloadBtn.onClick.AddListener(usingGun.Reload);
    }

    public void SetActiveReloadBtn(bool tf)
    {
        ReloadBtn.interactable = tf;
    }

    public bool SetWeaponBtn(int slotId, int itemId)
    {
        Button targetBtn;
        switch (slotId)
        {
            case 1:
                targetBtn = MainWeaponBtn; break;
            case 2:
                targetBtn = SubWeaponBtn; break;
            default:
                Debug.Log($"무기 슬롯의 코드가 적절하지 않음 slotId : {slotId}");
                return false;
        }

        Sprite targetSprite;
        if (itemId == 0) {
            targetSprite = null;
            targetBtn.interactable = false;
        }
        else
        {
            string path = Data_master_item_base.GetData(itemId).icon;
            if (path == null)
            {
                Debug.Log($"db에서 아이템을 찾을수 없음 itemId : {itemId}");
                return false;
            }
            targetSprite = Resources.Load<Sprite>($"Sprite/Item/{path}");
            if (targetSprite == null)
            {
                Debug.Log($"이미지 경로가 적절하지 않음 path : {targetSprite}");
                return false;
            }
        }

        targetBtn.transform.GetChild(1).GetComponent<Image>().sprite = targetSprite;
        targetBtn.interactable = true;
        return true;
    }

    public bool SetIQuickSlot(int slotId, ItemData itemdata)
    {
        IQuickSlot targetSlot = IQuickSlot.GetChild(slotId - 5).GetComponent<IQuickSlot>();
        if (targetSlot == null)
        {
            Debug.Log("타겟 슬롯을 찾지못함");
            return false;
        }

        if (itemdata == null)
        {
            targetSlot.ResetSlot();
        }
        targetSlot.SetSlot(itemdata);
        return true;
    }

    public void LobbyScene()
    {
        C_ExitGame cExit = new C_ExitGame
        {
            PlayerId = myPlayer.Id,
            ExitId = 1
        };
        Managers.Network.Send(cExit);

#if UNITY_EDITOR
        Managers.Scene.LoadScene(Define.Scene.Lobby);
#else
        Managers.Scene.LoadScene(Define.Scene.Shelter);
#endif

    }
}
