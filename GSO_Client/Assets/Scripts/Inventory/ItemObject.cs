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
    /*
     * �� �ڵ�� item�����տ� �����Ǹ� �������� ��ü�� �����մϴ�. 
     * �������� ������ �� �������� ���� ����(��ġ? ,ȸ��?, ������? ��)
     * 
     * 1. Set �Լ��� ��Ʈ�ѷ����� ������ ���ÿ� �ҷ����� ��ϵ� �����͸� ���� ������ �����Ű��
     *    ȸ���� ���迩�θ� �ʱ�ȭ�մϴ�.
     *    
     * 2. SearchingItem �Լ��� ��Ʈ�ѷ����� �ش� ������ ������Ʈ�� Ŭ�������� isHide�� true���
     *    �� �Լ��� �����ŵ�ϴ�. �ڷ�ƾ�� ����Ͽ� ��ȸ�ð� �� isHide�� �����ϰ� �������� �̹���
     *    �� �ٲٰ� �˴ϴ�.
     *    
     * 3. RotateRight, RotateLeft �Լ��� ��Ʈ�ѷ����� ȸ�� ����� �ٶ� ����մϴ�.
     *    rotated������ �ٲٸ� ���� RectTransform�� �����ŵ�ϴ�.
    */
    public const int maxItemMergeAmount = 64;

    public int itemId;
    public RectTransform itemRect;

    public ItemData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ

    //������(���� ���� ������ �ﰢ Ž���Ͽ� �Լ�����)
    public int Width
    {
        get
        {
            if (curItemRotate % 2 == 0)
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
            if (curItemRotate % 2 == 0)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }
    
    public int ItemAmount
    {
        get { return itemAmount; }
        set
        {
            itemAmount = value;
            TextControl();
        }
    }

    public bool ishide; //������ ���� ������
    private bool isOnSearching; //������ ��ȸ��
    public List<int> searchedPlayerList = new List<int>(); //�� ����Ʈ�� ���Ե� �÷��̾�� �������� ������
    public int itemAmount;

    //���� �������� ��ġ, ȸ����, �׸���
    public InventoryGrid curItemGrid;
    public Vector2Int curItemPos; //�������� �⺻��ġ
    public int curItemRotate = 0; // 0 : 0 or 360, 1: 90, 2 : 180, 3 : 270

    
    //��� ����
    public InventoryGrid backUpItemGrid; //�������� ���� ������ �׸���
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����
    

    /// <summary>
    /// �������� �����͸� ����
    /// </summary>
    public void Set(ItemData itemData)
    {
        transform.GetComponent<Image>().raycastTarget = false;
        itemRect = GetComponent<RectTransform>();
        //������ ������ ������Ʈ
        this.itemData = itemData;

        //������ ������ ������Ʈ
        Vector2 size = new Vector2();
        size.X = Width * InventoryGrid.WidthOfTile;
        size.Y = Height * InventoryGrid.HeightOfTile;
        transform.GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(size.X, size.Y);

        if (transform.parent != null)
        {
            curItemGrid = transform.parent.GetComponent<InventoryGrid>();
        }
        
        curItemPos = itemData.itemPos;
        itemRect.localPosition = new UnityEngine.Vector2(itemData.itemPos.x * InventoryGrid.WidthOfTile+50, itemData.itemPos.y* InventoryGrid.HeightOfTile-50);

        curItemRotate= itemData.itemRotate;
        Rotate(curItemRotate);

        ItemAmount = itemData.itemAmount;

        foreach(int i in itemData.searchedPlayerId)
        {
            searchedPlayerList[i] = itemData.searchedPlayerId[i];
        }

        //��ȸ�÷��̾� ����Ʈ�� ���Ե� �÷��̾� ���ο� ���� ����
        if (searchedPlayerList.Contains(InventoryController.invenInstance.playerId) == false)
        {
            transform.GetComponent<Image>().sprite = hideSprite;
            ishide = true;
            isOnSearching = false;
        }
        else
        {
            transform.GetComponent<Image>().sprite = itemData.itemSprite;
            ishide = false;
            isOnSearching = true;
        }
    }

    /// <summary>
    /// ������ �������� Ŭ���� ��� ������ ��ȸ
    /// </summary>
    public void SearchingItem()
    {
        if(isOnSearching == true) { return; }
        isOnSearching = true;

        //�������� ��ȸ �ð����� ����� ������ ����
        StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ishide = false;
        transform.GetComponent<Image>().sprite = itemData.itemSprite;
        searchedPlayerList.Add(InventoryController.invenInstance.playerId);
        TextControl();
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateRight()
    {
        curItemRotate = (curItemRotate+1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// �������� ��ȸ��
    /// </summary>
    public void RotateLeft()
    {
        curItemRotate = (curItemRotate-1) % 4;
        Rotate(curItemRotate);
    }

    /// <summary>
    /// �������� ȸ��. rect�� ȸ���ϸ� �̹��� ���� ȸ����.
    /// </summary>
    public void Rotate(int rotateInt)
    {
        itemRect.rotation = Quaternion.Euler(0, 0, 90 * rotateInt);
    }

    public void MergeItem(ItemObject targetItem, int mergeAmount)
    {
        if(itemData.itemCode == targetItem.itemData.itemCode)
        {
            targetItem.ItemAmount += mergeAmount;
            ItemAmount -= mergeAmount;
        }
    }

    public void TextControl()
    {
        TextMeshProUGUI amountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        amountText.raycastTarget = false;
        if (itemAmount <= 1 || ishide) 
        {
            amountText.gameObject.SetActive(false);
            return; 
        }

        amountText.text = itemAmount.ToString();
        amountText.gameObject.SetActive(true);
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
