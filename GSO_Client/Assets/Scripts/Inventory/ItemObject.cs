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
     * 1. ItemDataSet �Լ��� ��Ʈ�ѷ����� ������ ���ÿ� �ҷ����� ��ϵ� �����͸� ���� ������ �����Ű��
     *    ȸ���� ���迩�θ� �ʱ�ȭ�մϴ�.
     *    
     * 2. UnhideItem �Լ��� ��Ʈ�ѷ����� �ش� ������ ������Ʈ�� Ŭ�������� isHide�� true���
     *    �� �Լ��� �����ŵ�ϴ�. �ڷ�ƾ�� ����Ͽ� ��ȸ�ð� �� isHide�� �����ϰ� �������� �̹���
     *    �� �ٲٰ� �˴ϴ�.
     *    
     * 3. RotateRight, RotateLeft �Լ��� ��Ʈ�ѷ����� ȸ�� ����� �ٶ� ����մϴ�.
     *    rotated������ �ٲٸ� ���� RectTransform�� �����ŵ�ϴ�.
    */
    public const int maxItemMergeAmount = 64;

    public RectTransform itemRect;
    public ItemData itemData; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public Sprite hideSprite; //��ȸ ���� ������ ��������Ʈ
    public List<Sprite> spriteList;

    //������(���� ���� ������ �ﰢ Ž���Ͽ� �Լ�����)
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

    public bool ishide; //������ ���� ������
    private bool isOnSearching; //������ ��ȸ��


    //���� �������� ��ġ, ȸ����, �׸���
    public InventoryGrid curItemGrid;
    
    //��� ����
    public InventoryGrid backUpItemGrid; //�������� ���� ������ �׸���
    public Vector2Int backUpItemPos; //�������� ���� ��ġ
    public int backUpItemRotate; //�������� ���� ȸ����
    private Sprite itemSprite;

    private void Awake()
    {
        transform.GetComponent<Image>().raycastTarget = false;
        itemRect = GetComponent<RectTransform>();
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
        //��ȸ�÷��̾� ����Ʈ�� ���Ե� �÷��̾� ���ο� ���� ����
        if (itemData.searchedPlayerId.Contains(InventoryController.invenInstance.playerId) == false)
        {
            transform.GetComponent<Image>().sprite = hideSprite;
            ishide = true;
            isOnSearching = false;
        }
        else
        {
            transform.GetComponent<Image>().sprite = itemSprite;
            ishide = false;
            isOnSearching = true;
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
        StartCoroutine(SearchingTimer(itemData.item_searchTime));
    }

    private IEnumerator SearchingTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ishide = false;

        
        transform.GetComponent<Image>().sprite = itemSprite;
        itemData.searchedPlayerId.Add(InventoryController.invenInstance.playerId);
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
        if (itemData.itemAmount <= 1 || ishide) 
        {
            amountText.gameObject.SetActive(false);
            return; 
        }

        amountText.text = itemData.itemAmount.ToString();
        amountText.gameObject.SetActive(true);
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
