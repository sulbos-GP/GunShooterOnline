using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    //퀵슬롯 버튼 1, 2, 3에 넣을거임
    public Sprite defaultSprite;

    public ItemData itemData;
    private Image itemImage;
    private TextMeshProUGUI itemAmount;
    private RectTransform cooltimeRect;
    private Consume consumeData;

    public bool isReady;
    private Coroutine cooltimer;

    private void Awake()
    {
        itemData = null;
        itemImage = transform.GetChild(0).GetComponent<Image>();
        itemAmount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
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
        itemAmount.text = item.amount.ToString();
        consumeData = new Consume();
        if (!ConsumeDB.consumeDB.TryGetValue(item.itemId, out consumeData))
        {
            Debug.Log($"ConsumeDB에서 해당 아이템을 찾지 못함 {item.itemId}");
            return;
        }
        cooltimer = StartCoroutine(OnCooltime(consumeData.cooltime));
        itemData.OnAmountChanged += UpdateItemAmount;
    }

    public void UpdateItemAmount(int newAmount)
    {
        itemAmount.text = newAmount.ToString();
        
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
        itemAmount.text = "x";
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
        
        //추후 아이템 사용 패킷 생성후 교체
        //패킷을 통해 서버에 해당 아이템이 사용되었다는것을 알려주며 서버에있는 해당 아이템의 개수를 -1 시켜야함

        Item.amount -= 1; //아이템의 개수 감소
        if (Item.amount == 0) //개수가 0이되면 아이템 삭제 및 슬롯 리셋
        {
            ResetSlot();
        }
        else
        {
            itemAmount.text = Item.amount.ToString();
        }
    }

    public bool UseConsume(Consume consume)
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


        if (consume.effect == EffectType.Immediate)
        {
            isReady = false;
            myPlayer.OnHealed(consume.energe);
            cooltimer = StartCoroutine(OnCooltime(consume.cooltime));
        }
        else if (consume.effect == EffectType.Buff)
        {
            isReady = false;
            myPlayer.OnHealed(consume.energe);
            StartCoroutine(OnBuff(myPlayer, consume));
        }

        return true;
    }

    private IEnumerator OnBuff(CreatureController target, Consume consume)
    {
        float elapsedTime = 0f;

        while (elapsedTime < consume.duration)
        {
            if (target.Hp >= target.MaxHp)
            {
                target.OnHealed(consume.energe);
            }

            yield return new WaitForSeconds((float)consume.active_time);

            elapsedTime += (float)consume.active_time;
        }

        cooltimer = StartCoroutine(OnCooltime(consume.cooltime));
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
            float remainingTimeRatio = elapseTime / (float)consumeData.cooltime;

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

