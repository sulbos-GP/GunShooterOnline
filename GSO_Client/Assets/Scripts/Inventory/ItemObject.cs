using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using static System.TimeZoneInfo;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    public const int maxItemMergeAmount = 64;

    /// <summary>
    /// ������ �����ͷ� ���ο� ������ UI�� ������
    /// </summary>
    public static ItemObject CreateNewItemObj(ItemData data, Transform parent = null)
    {
        ItemObject newItem = Managers.Resource.Instantiate("UI/InvenUI/ItemUI", parent).GetComponent<ItemObject>();
        newItem.SetItem(data);
        if (!InventoryController.instantItemDic.ContainsKey(data.objectId))
        {
            InventoryController.instantItemDic.Add(data.objectId, newItem);
        }
        return newItem;
    }

    /// <summary>
    /// �ش� �������� ���� ��ġ, ȸ��, �θ��� ���̵� ���
    /// </summary>
    public static void BackUpItem(ItemObject item)
    {
        item.backUpItemPos = item.itemData.pos; //���� ��ġ
        item.backUpItemRotate = item.itemData.rotate; //���� ȸ��
        item.backUpParentId = item.parentObjId; //���� �θ� ������Ʈ
    }


    //�ڽ� ������Ʈ
    public Image imageUI;
    public TextMeshProUGUI amountText; //������ ���� �ؽ�Ʈ
    public TextMeshProUGUI searchTimerUI; //������ ���� Ÿ�̸� �ؽ�Ʈ

    //������Ʈ
    public RectTransform itemRect;

    //�������� ��������Ʈ
    public Sprite itemSprite;
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ

    public Coroutine searchingCoroutine;

    //������ ������ ���
    public ItemData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����

    public int Width
    {
        get
        {
            if (itemData.rotate % 2 == 0)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }
    public int Height
    {
        get
        {
            if (itemData.rotate % 2 == 0)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    public int ItemAmount
    {
        get { return itemData.amount; }
        set
        {
            itemData.amount = value;

            itemWeight = itemData.item_weight * value;
            itemWeight = Math.Round(itemWeight,2);
            if (itemData.isSearched)
            {
                TextControl();
            }
        }
    }

    //�� ����
    public bool isHide; //������ ���� ������
    public bool isOnSearching; //������ ��ȸ��

    public double itemWeight; //���ݱ��� ���� itemData.itme_weight��  �̺����� �ٲܰ�

    public int parentObjId;
    public int backUpParentId;

    private void Init()
    {
        itemRect = GetComponent<RectTransform>();
        imageUI = transform.GetChild(0).GetComponent<Image>(); 
        imageUI.raycastTarget = false;

        searchTimerUI = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        searchTimerUI.raycastTarget = false;
        searchTimerUI.gameObject.SetActive(false);
        
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        amountText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            searchTimerUI.gameObject.SetActive(false);
            searchingCoroutine = null;
            isOnSearching = false;
        }
    }

    /// <summary>
    /// �������� �����͸� ����
    /// </summary>
    public void SetItem(ItemData itemData)
    {
        Init();

        //������ ������ ������Ʈ
        this.itemData = itemData;
        itemRect = GetComponent<RectTransform>();
        itemSprite = FindItemSprtie(itemData);
        isOnSearching = false;
        ItemAmount = itemData.amount;

        //Ŭ�� ���忡�� �̹� ��ȸ�� �����Ͷ�� �ش� �������� �̹����� �ƴϸ� ���� �̹�����
        if (itemData.isSearched == false)
        {
            imageUI.sprite = hideSprite;
            isHide = true;
        }
        else
        {
            imageUI.sprite = itemSprite != null ?  itemSprite : hideSprite;
            isHide = false;
        }

        //������ ������Ʈ�� �ʱ� ũ�⸦ ����
        //�������� rotate�� 0�϶��� ���� �����ϰ� �� �Ŀ� rotate�� ����
        Vector2 size = new Vector2();
        size.X = itemData.width * GridObject.WidthOfTile; //�׸��� 1ĭ�� ũ�� * ����,�ʺ�
        size.Y = itemData.height * GridObject.HeightOfTile;
        itemRect.sizeDelta = new UnityEngine.Vector2(size.X, size.Y);
        imageUI.GetComponent<RectTransform>().sizeDelta = itemRect.sizeDelta;

        Rotate(itemData.rotate);

        //�������� ��ġ�� ����. ���� ��Ŀ�� �»�� ������
        itemRect.localPosition = new UnityEngine.Vector2(itemData.width * GridObject.WidthOfTile + 50, itemData.height * GridObject.HeightOfTile - 50);
    }

    public static Sprite FindItemSprtie(ItemData itemData)
    {
        Sprite itemSprite = Resources.Load<Sprite>($"Sprite/Item/{itemData.iconName}");

        if (itemSprite == null)
        {
            Debug.Log("��������Ʈ �˻� ����");
            return null;
        }

        return itemSprite;
    }

    /// <summary>
    /// ������ �������� Ŭ���� ��� ������ ��ȸ
    /// </summary>
    public void SearchItemHandler()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //�������� ��ȸ �ð����� ����� ������ ����
        searchingCoroutine = StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        float timeRemaining = duration;

        searchTimerUI.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 10); // One decimal place

            searchTimerUI.text = string.Format("{0}:{1}", seconds, milliseconds);
            yield return null;
        }

        RevealItem();

        searchTimerUI.gameObject.SetActive(false);
        searchingCoroutine = null;
    }

    public void RevealItem()
    {
        isHide = false;
        itemData.isSearched = true;

        imageUI.sprite = itemSprite;
        TextControl();
    }

    public void HideItem()
    {
        isHide = true;
        itemData.isSearched = false;
        if (searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            isOnSearching = false;
            searchingCoroutine = null;
        }

        imageUI.sprite = hideSprite;
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateRight()
    {
        itemData.rotate = (itemData.rotate + 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateLeft()
    {
        itemData.rotate = (itemData.rotate - 1) % 4;
        Rotate(itemData.rotate);
    }

    /// <summary>
    /// �������� ȸ��. rect�� ȸ���ϸ� �̹��� ���� ȸ����.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        itemRect.sizeDelta = new UnityEngine.Vector2(Width* GridObject.WidthOfTile, Height*GridObject.HeightOfTile);
       
        imageUI.GetComponent< RectTransform >().rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
        
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        //�̹� ��ȿ�� üũ�� ������.
        targetItem.ItemAmount += mergeAmount;
        ItemAmount -= mergeAmount;

    }

    public void TextControl()
    {
        amountText.text = ItemAmount.ToString();

        //������ ���� �ؽ�Ʈ�� ��Ȱ��ȭ �����̰� 2 �̻��̸� Ȱ��ȭ
        if (ItemAmount > 1  && !isHide) 
        {
            amountText.gameObject.SetActive(true);
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    public Coroutine blinkCoroutine = null;

    public void StartBlink()
    {
        if(blinkCoroutine != null)
        {
            StopBlink();
        }

        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    public void StopBlink()
    {
        StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }
    public IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;
        float transitionTime = 1f;
        imageUI.color = Color.red;
        while (elapsedTime < transitionTime)
        {
            // ��� �ð��� ���� ���� ��ȭ
            imageUI.color = Color.Lerp(Color.red, Color.white, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        imageUI.color = Color.white;
    }

    public void DestroyItem()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
