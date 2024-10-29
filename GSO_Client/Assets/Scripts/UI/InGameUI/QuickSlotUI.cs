using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    //퀵슬롯 버튼 1, 2, 3의 컴포넌트에 부착
    public Sprite defaultSprite;

    public ItemData itemData;
    private Image itemImage;
    private TextMeshProUGUI itemAmountText;
    private RectTransform cooltimeRect;
    private Data_master_item_use consumeData;

    public bool isReady;
    private Coroutine cooltimer;

    private void Awake()
    {
        itemData = null;
        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemAmountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        cooltimeRect = transform.GetChild(2).GetComponent<RectTransform>();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => UseQuickSlot(itemData));
        ResetSlot();
    }


    /// <summary>
    /// 슬롯의 이미지와 수량 텍스트 설정
    /// </summary>
    public void SetSlot(ItemData item)
    {
        if(itemData!= null && itemData.itemId == item.itemId)
        {
            return;
        }

        itemData = item;
        GetComponent<Button>().interactable = true;

        Sprite itemSprite = ItemObject.FindItemSprtie(item);
        itemImage.sprite = itemSprite;
        itemAmountText.text = item.amount.ToString();
        consumeData = new Data_master_item_use();

        consumeData = Data_master_item_use.GetData(item.itemId);
        if (consumeData == null)
        {
            Debug.Log($"ConsumeDB에서 해당 아이템을 찾지 못함 {item.itemId}");
            return;
        }
        cooltimer = StartCoroutine(OnCooltime(consumeData.cool_time));
        itemData.OnAmountChanged += UpdateItemAmount;
    }

    public void UpdateItemAmount(int newAmount)
    {
        itemAmountText.text = newAmount.ToString();
        
        if (newAmount <= 0)
        {
            ResetSlot();
        }
    }

    /// <summary>
    /// 슬롯의 아이템 제거 -> 디폴트 이미지 및 텍스트로 변경
    /// </summary>
    public void ResetSlot()
    {
        if (itemData != null) {
            itemData.OnAmountChanged -= UpdateItemAmount;
        }
        itemData = null;
        itemImage.sprite = defaultSprite;
        itemAmountText.text = "x";
        consumeData = null;

        cooltimeRect.sizeDelta = Vector2.zero;
        isReady = false;
        if(cooltimer != null)
        {
            StopCoroutine(cooltimer);
            cooltimer = null;
        }
        
        GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 등록된 아이템을 사용. 이건 아이템의 기능이 나와야할듯
    /// </summary>
    public void UseQuickSlot(ItemData Item)
    {
        if (!isReady)
        {
            return;
        }

        if(Item == null )
        {
            Debug.Log("아이템이 등록되어있지 않음");
            return;
        }

        if (!UseConsume(consumeData))//아이템 사용
        {
            Debug.Log("아이템 사용 실패");
            return;
        }

        C_InputData inputPacket = new C_InputData();
        inputPacket.Item = Item.itemId;

        Managers.Network.Send(inputPacket);


        //추후 아이템 사용 패킷 생성후 교체
        //패킷을 통해 서버에 해당 아이템이 사용되었다는것을 알려주며 서버에있는 해당 아이템의 개수를 -1 시켜야함

        Item.amount -= 1; //아이템의 개수 감소
        if (Item.amount == 0) //개수가 0이되면 아이템 삭제 및 슬롯 리셋
        {
            ResetSlot();
        }
        else
        {
            itemAmountText.text = Item.amount.ToString();
        }
    }

    public bool UseConsume(Data_master_item_use consume)
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

        if (consume.effect == EEffect.immediate)
        {
            isReady = false;
            myPlayer.OnHealed(consume.energy); //체력 회복
            cooltimer = StartCoroutine(OnCooltime(consume.cool_time)); //쿨타임 코루틴 시작
        }
        else if (consume.effect == EEffect.buff)
        {
            isReady = false;
            StartCoroutine(myPlayer.OnBuffed(consume));
            cooltimer = StartCoroutine(OnCooltime(consume.cool_time)); //버프가 끝난뒤 쿨타임을 적용하고 싶다면 cooltime+효과 지속시간
        }

        return true;
    }

    

    private IEnumerator OnCooltime(double cooltime)
    {
        Button thisBtn = GetComponent<Button>();
        thisBtn.interactable = false;

        float elapseTime = (float)cooltime;

        Vector2 initialSize = thisBtn.GetComponent<RectTransform>().sizeDelta;
        cooltimeRect.sizeDelta = initialSize;

        // 쿨타임이 끝날 때까지 반복
        while (elapseTime > 0)
        {
            float remainingTimeRatio = elapseTime / (float)consumeData.cool_time;

            float newSize = initialSize.y * remainingTimeRatio;
            cooltimeRect.sizeDelta = new Vector2(initialSize.x, newSize);
            elapseTime -= Time.deltaTime;
            yield return null;
        }

        cooltimeRect.sizeDelta = Vector2.zero;
        thisBtn.interactable = true;
        cooltimer = null;
        isReady = true;
    }
}

