using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public class ItemObject : MonoBehaviour
{
    public const int maxItemMergeAmount = 64;

    //�ڽİ�ü
    public Image imageUI;
    public TextMeshProUGUI amountText; //������ ���� �ؽ�Ʈ
    public TextMeshProUGUI unhideTimer; //������ ���� Ÿ�̸� �ؽ�Ʈ

    //������Ʈ
    public RectTransform itemRect;

    //�������� ��������Ʈ
    public List<Sprite> spriteList;
    private Sprite itemSprite;
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ

    private Coroutine searchingCoroutine;

    //������ ������ ���
    public ItemData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public int Width
    {
        get
        {
            if (itemData.itemRotate % 2 == 0)
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
            if (itemData.itemRotate % 2 == 0)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    public int ItemAmount
    {
        get { return itemData.itemAmount; }
        set
        {
            itemData.itemAmount = value;
            TextControl();
        }
    }
    public InventoryGrid curItemGrid; 

    //�� ����
    public bool isHide; //������ ���� ������
    private bool isOnSearching; //������ ��ȸ��

    //��� ����
    public InventoryGrid backUpItemGrid; //�������� ���� ������ �׸���
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����
    

    private void Awake()
    {
        itemRect = GetComponent<RectTransform>();
        imageUI = transform.GetChild(0).GetComponent<Image>();
        imageUI.raycastTarget = false;

        unhideTimer = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        unhideTimer.raycastTarget = false;
        unhideTimer.gameObject.SetActive(false);
        
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        amountText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if(searchingCoroutine != null)
        {
            StopCoroutine(searchingCoroutine);
            unhideTimer.gameObject.SetActive(false);
            searchingCoroutine = null;
            isOnSearching = false;
        }
    }

    /// <summary>
    /// �������� �����͸� ����
    /// </summary>
    public void ItemDataSet(ItemData itemData)
    {
        //������ ������ ������Ʈ
        this.itemData = itemData;

        //������Ʈ�� ��Ʈ�� ������ ������Ʈ
        Vector2 size = new Vector2();
        size.X = Width * InventoryGrid.WidthOfTile;
        size.Y = Height * InventoryGrid.HeightOfTile;
        transform.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(size.X, size.Y);

        //�������� ũ��� ȸ�� ����
        itemRect.localPosition = new UnityEngine.Vector2(itemData.itemPos.x * InventoryGrid.WidthOfTile+50, itemData.itemPos.y* InventoryGrid.HeightOfTile-50);
        Rotate(itemData.itemRotate);
        ItemAmount = itemData.itemAmount;
        itemSprite = spriteList[itemData.itemCode - 1];
        isOnSearching = false;

        //��ȸ�÷��̾� ����Ʈ�� ���Ե� �÷��̾� ���ο� ���� ����
        if (itemData.searchedPlayerId.Contains(Managers.Object.MyPlayer.Id) == false)
        {
            imageUI.sprite = hideSprite;
            isHide = true;
        }
        else
        {
            imageUI.sprite = itemSprite;
            isHide = false;
        }
    }

    /// <summary>
    /// ������ �������� Ŭ���� ��� ������ ��ȸ
    /// </summary>
    public void UnhideItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //�������� ��ȸ �ð����� ����� ������ ����
        searchingCoroutine = StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        float timeRemaining = duration;

        unhideTimer.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(timeRemaining);
            int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 10); // One decimal place

            unhideTimer.text = string.Format("{0}:{1}", seconds, milliseconds);
            yield return null;
        }

        // Hide the timer text after the countdown is complete
        unhideTimer.gameObject.SetActive(false);

        isHide = false;
        imageUI.sprite = itemSprite;
        itemData.searchedPlayerId.Add(Managers.Object.MyPlayer.Id);
        TextControl();
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateRight()
    {
        itemData.itemRotate = (itemData.itemRotate + 1) % 4;
        Rotate(itemData.itemRotate);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateLeft()
    {
        itemData.itemRotate = (itemData.itemRotate - 1) % 4;
        Rotate(itemData.itemRotate);
    }

    /// <summary>
    /// �������� ȸ��. rect�� ȸ���ϸ� �̹��� ���� ȸ����.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        //itemRect.rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
        imageUI.GetComponent< RectTransform >().rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        if (itemData.itemCode == targetItem.itemData.itemCode)
        {
            targetItem.ItemAmount += mergeAmount;
            ItemAmount -= mergeAmount;
        }

        if(itemData.itemAmount <= 0)
        {
            backUpItemGrid.gridData.itemList.Remove(itemData);
            Managers.Object.RemoveItemDic(itemData.itemId);
        }
    }

    public void TextControl()
    {
        amountText.text = itemData.itemAmount.ToString();

        //������ ���� �ؽ�Ʈ�� ��Ȱ��ȭ �����̰� 2 �̻��̸� Ȱ��ȭ
        if (itemData.itemAmount > 1 && amountText.gameObject.activeSelf == false && !isHide) 
        {
            amountText.gameObject.SetActive(true);
        }
    }

    public void DestroyItem()
    {
        curItemGrid.gridData.itemList.Remove(itemData);
        Managers.Object.RemoveItemDic(itemData.itemId);
        Destroy(gameObject);
    }
}
