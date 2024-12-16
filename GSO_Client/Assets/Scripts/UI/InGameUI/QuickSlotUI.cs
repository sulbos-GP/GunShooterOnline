using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IQuickSlot : MonoBehaviour
{
    //������ ��ư 1, 2, 3�� ������Ʈ�� ����
    public int SlotId;

    public Sprite defaultSprite;

    //������Ʈ
    public Image itemImage;
    public TextMeshProUGUI itemAmountText;
    public Image coolTimeImage;

    public ItemData itemData;
    
    public bool isReady;
    private Coroutine cooltimer;

    private void Awake()
    {
        itemImage = transform.GetChild(1).GetComponent<Image>();
        itemAmountText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        coolTimeImage = transform.GetChild(3).GetComponent<Image>();
        
    }

    public void Init()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => UseQuickSlot(itemData));
        ResetSlot();
    }

    /// <summary>
    /// ������ �̹����� ���� �ؽ�Ʈ ����
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
    /// ������ ������ ���� -> ����Ʈ �̹��� �� �ؽ�Ʈ�� ����
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
    /// ��ϵ� �������� ���. �̰� �������� ����� ���;��ҵ�
    /// </summary>
    public void UseQuickSlot(ItemData item)
    {
        if (item == null)
        {
            Debug.Log("�������� ��ϵǾ����� ����");
            return;
        }

        if (!CheckAbleToUse())//������ ���
        {
            Debug.Log("������ ��� ����");
            return;
        }

        C_InputData inputPacket = new C_InputData();
        inputPacket.ItemId = item.objectId;
        inputPacket.ItemSoltId = SlotId;

        Managers.Network.Send(inputPacket);

        item.amount -= 1; //�������� ���� ����

        UpdateItemAmount(item.amount);
        Data_master_item_use consumeData = Data_master_item_use.GetData(item.itemId);

        StartCoroutine(OnBuffMark(consumeData.duration)); //���ӽð� ���� �� ��ũ ����
        StartCoroutine(OnCooltime(consumeData.cool_time));
    }

    public bool CheckAbleToUse()
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

            // ��� �ð� ������Ʈ
            elapseTime += Time.deltaTime;
            yield return null;
        }

        // ��Ÿ�� ���� �� �ʱ�ȭ
        coolTimeImage.fillAmount = 0;
        thisBtn.interactable = true;
        cooltimer = null;
        isReady = true;
    }

    private IEnumerator OnBuffMark(float time)
    {
        UIManager.Instance.SetActiveHealImage(true);

        yield return new WaitForSeconds(time);

        UIManager.Instance.SetActiveHealImage(false);
    }

}

