using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    //������ ��ư 1, 2, 3�� ��������
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
    /// ������ �̹����� ���� �ؽ�Ʈ ����
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
            Debug.Log($"ConsumeDB���� �ش� �������� ã�� ���� {item.itemId}");
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
    /// ������ ������ ���� -> ����Ʈ �̹��� �� �ؽ�Ʈ�� ����
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
    /// ��ϵ� �������� ���. �̰� �������� ����� ���;��ҵ�
    /// </summary>
    public void UseQuickSlot(ItemData Item)
    {
        if (!isReady)
        {
            return;
        }

        if(Item == null )
        {
            Debug.Log("�������� ��ϵǾ����� ����");
            return;
        }

        if (!UseConsume(consumeData))//������ ���
        {
            Debug.Log("������ ��� ����");
            return;
        }
        
        //���� ������ ��� ��Ŷ ������ ��ü
        //��Ŷ�� ���� ������ �ش� �������� ���Ǿ��ٴ°��� �˷��ָ� �������ִ� �ش� �������� ������ -1 ���Ѿ���

        Item.amount -= 1; //�������� ���� ����
        if (Item.amount == 0) //������ 0�̵Ǹ� ������ ���� �� ���� ����
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
            Debug.Log("�������� ������ �÷��̾ ã�� ����");
            return false;
        }

        if (!isReady)
        {
            Debug.Log("���� ��Ȱ��ȭ ������");
            return false;
        }

        if (cooltimer != null)
        {
            Debug.Log("��Ÿ���� ���ư��� ��");
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

        // ��Ÿ���� ���� ������ �ݺ�
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

