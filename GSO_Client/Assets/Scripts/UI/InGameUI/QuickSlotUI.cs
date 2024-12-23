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
    public int SlotId;

    [SerializeField]private Sprite defaultSprite;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private Image coolTimeImage;
    [SerializeField] private ItemData itemData;
    [SerializeField] private bool isReady;
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

    public void SetSlot(ItemData item)
    {
        if (itemData != null && itemData.itemId == item.itemId)
        {
            return;
        }
        itemData = item;
        GetComponent<Button>().interactable = true;

        Sprite itemSprite = ItemObject.GetItemSprite(item);
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

    public void UseQuickSlot(ItemData item)
    {
        if (item == null)
        {
            return;
        }

        if (!CheckAbleToUse())
        {
            return;
        }

        C_InputData inputPacket = new C_InputData();
        inputPacket.ItemId = item.objectId;
        inputPacket.ItemSoltId = SlotId;

        Managers.Network.Send(inputPacket);

        item.amount -= 1;

        UpdateItemAmount(item.amount);
        Data_master_item_use consumeData = Data_master_item_use.GetData(item.itemId);
        if(item.itemId == 402)
            AudioManager.instance.PlayOneShot("Bandage",gameObject.GetComponent<AudioSource>(),1.0f);
        else if(item.itemId == 404)
            AudioManager.instance.PlayOneShot("Cigarret",gameObject.GetComponent<AudioSource>(),1.0f);
        StartCoroutine(OnBuffMark(consumeData.duration));
        StartCoroutine(OnCooltime(consumeData.cool_time));
    }

    public bool CheckAbleToUse()
    {
        MyPlayerController myPlayer = Managers.Object.MyPlayer;

        if (myPlayer == null)
        {
            return false;
        }

        if (!isReady)
        {
            return false;
        }

        if (cooltimer != null)
        {
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

            elapseTime += Time.deltaTime;
            yield return null;
        }

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

