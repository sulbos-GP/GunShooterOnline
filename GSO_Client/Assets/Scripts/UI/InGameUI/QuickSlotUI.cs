using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class IQuickSlot : MonoBehaviour
{
    //퀵슬롯 버튼 1, 2, 3의 컴포넌트에 부착
    public int SlotId;

    public Sprite defaultSprite;

    //컴포넌트
    public Image itemImage;
    public TextMeshProUGUI itemAmountText;
    public Image coolTimeImage;

    public ItemData itemData;
    
    public bool isReady;
    private Coroutine cooltimer;

    private void Awake()
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemAmountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        coolTimeImage = transform.GetChild(2).GetComponent<Image>();
        
    }

    public void Init()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => UseQuickSlot(itemData));
        ResetSlot();
    }

    /// <summary>
    /// 슬롯의 이미지와 수량 텍스트 설정
    /// </summary>
    public void SetSlot(ItemData item)
    {
        if (itemData != null && itemData.itemId == item.itemId)
        {
            return;
        }
        itemData = item;
        GetComponent<Button>().interactable = true;

        Sprite itemSprite = ItemObject.FindItemSprtie(item);
        itemImage.sprite = itemSprite;
        UpdateItemAmount(item.amount);
        StartCoroutine(OnCooltime(Data_master_item_use.GetData(item.itemId).cool_time));
    }

    public void UpdateItemAmount(int newAmount)
    {
        if (newAmount <= 0)
        {
            ResetSlot();
        }
        else
        {
            itemAmountText.text = newAmount.ToString();
        }
    }

    /// <summary>
    /// 슬롯의 아이템 제거 -> 디폴트 이미지 및 텍스트로 변경
    /// </summary>
    public void ResetSlot()
    {
        itemImage.sprite = defaultSprite;
        itemAmountText.text = "x";

        coolTimeImage.fillAmount = 0;

        isReady = false;
        if(cooltimer != null)
        {
            StopCoroutine(cooltimer);
            cooltimer = null;
        }
        itemData = null;
        GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 등록된 아이템을 사용. 이건 아이템의 기능이 나와야할듯
    /// </summary>
    public void UseQuickSlot(ItemData item)
    {
        if (item == null)
        {
            Debug.Log("아이템이 등록되어있지 않음");
            return;
        }

        if (!CheckAbleToUse())//아이템 사용
        {
            Debug.Log("아이템 사용 실패");
            return;
        }

        C_InputData inputPacket = new C_InputData();
        inputPacket.ItemId = item.objectId;
        inputPacket.ItemSoltId = SlotId;

        Managers.Network.Send(inputPacket);

        item.amount -= 1; //아이템의 개수 감소
        UpdateItemAmount(item.amount);
        StartCoroutine(OnCooltime(Data_master_item_use.GetData(item.itemId).cool_time));
    }

    public bool CheckAbleToUse()
    {
        MyPlayerController myPlayer = Managers.Object.MyPlayer;

        if (myPlayer == null)
        {
            Debug.Log("아이템을 적용할 플레이어를 찾지 못함");
            return false;
        }

        if (!isReady)
        {
            Debug.Log("현재 비활성화 상태임");
            return false;
        }

        if (cooltimer != null)
        {
            Debug.Log("쿨타임이 돌아가는 중");
            return false;
        }
        return true;
    }



    private IEnumerator OnCooltime(double cooltime)
    {
        Button thisBtn = GetComponent<Button>();
        thisBtn.interactable = false;

        coolTimeImage.fillAmount = 1;
        float elapseTime = 0f;

        while (elapseTime < cooltime)
        {
            float remainingTimeRatio = 1 - (elapseTime / (float)cooltime);
            coolTimeImage.fillAmount = remainingTimeRatio;

            // 경과 시간 업데이트
            elapseTime += Time.deltaTime;
            yield return null;
        }

        // 쿨타임 종료 후 초기화
        coolTimeImage.fillAmount = 0;
        thisBtn.interactable = true;
        cooltimer = null;
        isReady = true;
    }

}

