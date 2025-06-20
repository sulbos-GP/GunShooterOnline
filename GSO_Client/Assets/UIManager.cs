﻿using Google.Protobuf.Protocol;
using MathNet.Numerics.Providers.SparseSolver;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject DieUI { get; private set; }
    public UI_Quest QuestUI { get; private set; }

    public TMP_Text HealthText { get; private set; }
    public Image CureImage { get; private set; }

    public TMP_Text AmmoText { get; private set; }
    public TMP_Text TimeText { get; private set; }
    public float leftTime;

    public Button ReloadBtn { get; private set; }
    public Button InteractBtn { get; private set; }
    public Button OptionBtn { get; private set; }
    public Button InventoryBtn { get; private set; }

    public Transform WQuickSlot { get; private set; }
    public Button MainWeaponBtn { get; private set; }
    public Button SubWeaponBtn { get; private set; }

    public Transform IQuickSlot { get; private set; }

    public Image HitImage{get; private set;}
    
    public Image FadeImage { get; private set; }
    public TextMeshProUGUI LoadingText { get; private set; }


    public float Duration = 2.0f;
    private bool isDie;
    private bool _init = false;
    private float _alphaTime = 3f;
    
    //Fade Var
    public float _fadeDuration = 1.0f;
    public float _textDuration = 0.5f; // 텍스트 변경 간격
    private bool isLoading = true;

    
    private MyPlayerController myPlayer => Managers.Object.MyPlayer;

    

    private void Awake() 
    {
        Instance = this;
        Init();
    }

    void Start()
    {
        isDie = false;
        StartCoroutine(AnimateLoadingText());
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

        UpdateTime();

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
                    AmmoText.text = "Not Setted";
                    break;
                case "TimeUI":
                    TimeText = child.GetComponentInChildren<TMP_Text>();
                    TimeText.text = "Not Setted";
                    break;
                case "Health":
                    HealthText = child.GetChild(0).GetComponent<TMP_Text>();
                    CureImage = child.GetChild(1).GetComponent<Image>();
                    HealthText.text = "Not Setted";
                    CureImage.gameObject.SetActive(false);
                    break;
                case "InteractBtn":
                    InteractBtn = child.GetComponent<Button>();
                    InteractBtn.interactable = false;
                    break;
                case "ReloadBtn":
                    ReloadBtn = child.GetComponent<Button>();
                    ReloadBtn.interactable = false;
                    ReloadBtn.transform.GetChild(1).GetComponent<Image>().fillAmount = 0;
                    break;
                /*case "OptionBtn":
                    OptionBtn = child.GetComponent<Button>();
                    break;*/
                case "InventoryBtn":
                    InventoryBtn = child.GetComponent<Button>();
                    break;
                case "DieUI":
                    DieUI = child.gameObject;
                    DieUI.SetActive(false);
                    break;
                case "IQuickSlot":
                    IQuickSlot = child;
                    IQuickSlot[] slots = IQuickSlot.GetComponentsInChildren<IQuickSlot>();
                    foreach(IQuickSlot slot in slots)
                    {
                        slot.Init();
                    }
                    break;
                case "WQuickSlot":
                    WQuickSlot = child;
                    MainWeaponBtn = child.GetChild(0).GetComponent<Button>();
                    SubWeaponBtn = child.GetChild(1).GetComponent<Button>();
                    SetWQuickSlot(1); //초기화
                    SetWQuickSlot(2);
                    break;
                case "HitImage":
                    HitImage = child.GetComponent<Image>();
                    HitImage.color = Color.clear;
                    break;
                case "FadeImage":
                    FadeImage = child.GetComponent<Image>();
                    LoadingText = child.GetChild(0).GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        DieUI.gameObject.SetActive(false);
    }
    
    //=======FadeIn/Out Code//
    public void StartFadeIn()
    {
        FadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public void StartFadeOut()
    {
        FadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator AnimateLoadingText()
    {
        while (isLoading) // 로딩 상태가 true인 동안 반복
        {
            // "Loading.", "Loading..", "Loading..." 순으로 변경
            LoadingText.text = "Loading.";
            yield return new WaitForSeconds(_textDuration);

            LoadingText.text = "Loading..";
            yield return new WaitForSeconds(_textDuration);

            LoadingText.text = "Loading...";
            yield return new WaitForSeconds(_textDuration);
        }

        // 조건 만족 시 페이드 아웃 실행
        StartCoroutine(FadeOut());
    }
    public void StopLoading() // 조건 만족 시 호출
    {
        isLoading = false; // 로딩 상태 종료
    }
    
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color imageColor = FadeImage.color;
        Color textColor = LoadingText.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 동일한 Alpha 값 적용
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeDuration);

            // 이미지와 텍스트의 Alpha를 같은 비율로 조정
            imageColor.a = alpha;
            FadeImage.color = imageColor;

            textColor.a = alpha;
            LoadingText.color = textColor;

            yield return null;
        }

        // 최종 Alpha 값 설정 (완전 불투명)
        imageColor.a = 1f;
        FadeImage.color = imageColor;

        textColor.a = 1f;
        LoadingText.color = textColor;
        FadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        float dotCount = PlayerPrefs.GetInt("dotCount");
        LoadingText.text = "Loading";
        for(int i=0; i<dotCount; i++)
            LoadingText.text += ".";
        float elapsedTime = 0f;

        Color imageColor = FadeImage.color;
        Color textColor = LoadingText.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 동일한 Alpha 값 적용
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);

            // 이미지와 텍스트의 Alpha를 같은 비율로 조정
            imageColor.a = alpha;
            FadeImage.color = imageColor;

            textColor.a = alpha;
            LoadingText.color = textColor;
            
            yield return null;
        }

        // 최종 Alpha 값 설정 (완전 투명)
        imageColor.a = 0f;
        FadeImage.color = imageColor;

        textColor.a = 0f;
        LoadingText.color = textColor;
        FadeImage.gameObject.SetActive(false);
    }
    
    //=====================//
    

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

    public void SetDieMessage(string attackerName)
    {
        DieUI.SetActive(true);
        Transform dieContent = DieUI.transform.GetChild(2).GetComponent<ScrollRect>().content;
        dieContent.Find("KillName").GetComponent<TextMeshProUGUI>().text = "Killer : ";
        dieContent.Find("KillName").GetComponent<TextMeshProUGUI>().text += attackerName;
        StartCoroutine(TextAlpha(DieUI.GetComponent<CanvasGroup>(), Duration));
    }

    public IEnumerator SetHitEffect()
    {
        HitImage.color = new Color(1f, 0f, 0f, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        HitImage.color = Color.clear;
    }

    public void SetActiveHealImage(bool tf)
    {
        CureImage.gameObject.SetActive(tf);
    }

    public void SetTimeUI(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        TimeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    private void UpdateTime()
    {
        leftTime -= Time.deltaTime;
        SetTimeUI(leftTime);
    }


    public void SetHpText()
    {
        float MaxHP = myPlayer.MaxHp;
        float HP = myPlayer.Hp;
        HealthText.text = $"HP : {(HP / MaxHP)*100}% ";
    }

    public void SetAmmoText()
    {
        if (myPlayer.usingGun.gunState == GunState.Reloading)
            AmmoText.text = "Reloading";
        else
        {
            if (myPlayer.usingGun.WeaponData == null)
            {
                AmmoText.text = "No Gun";
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

    public bool SetWQuickSlot(int slotId, int itemId = 0)
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
        Image targetImageUI = targetBtn.transform.GetChild(1).GetComponent<Image>();
        Sprite targetSprite;
        if (itemId == 0) {
            targetBtn.interactable = false;
            targetSprite = null;
            targetImageUI.gameObject.SetActive(false);
            return true;
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

        targetImageUI.gameObject.SetActive(true);
        targetImageUI.sprite = targetSprite;
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
        else
        {
            targetSlot.SetSlot(itemdata);
        }
        
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
