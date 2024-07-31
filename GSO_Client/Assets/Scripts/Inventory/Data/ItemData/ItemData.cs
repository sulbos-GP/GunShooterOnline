using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[CreateAssetMenu(fileName = "new ItemData", menuName = "InvenData/ItemData")]
public class ItemData : ScriptableObject
{
    /*
     * ��ũ���ͺ� ������Ʈ�� �������� �����͸� �����մϴ�.
     * itemCode(������ ������ ���� �ڵ�) , �������� �̸�, �������� �˻��ϴ� �ð�, ũ��, �̹���
     * �� �����Ͽ� ������ �����տ� �����˴ϴ�.
     * 
     * �����δ� ��Ʈ�ѷ��� ����Ʈ�� �־� ��Ʈ�ѷ����� �����Ҷ� ����Ʈ ���� �������� �ϳ��� 
     * ������ŵ�ϴ�.
     */

    /// <summary>
    /// ItemDataInfo�� �ش� ��ũ��Ʈ�� ������ ����
    /// </summary>
    public void SetItemData(ItemDataInfo itemDataInfo)
    {
        itemId = itemDataInfo.ItemId;
        itemCode = itemDataInfo.ItemCode;
        itemPos = new Vector2Int(itemDataInfo.ItemPosX, itemDataInfo.ItemPosY);
        itemRotate = itemDataInfo.ItemRotate;
        itemAmount = itemDataInfo.ItemAmount;

        foreach(int id in itemDataInfo.SearchedPlayerId)
        {
            searchedPlayerId.Add(id);
        }

        //�ӽ�
        item_name = itemDataInfo.ItemName;
        item_weight = itemDataInfo.ItemWeight; //�������� ����
        item_type = itemDataInfo.ItemType;
        item_string_value = itemDataInfo.ItemStringValue;
        item_purchase_price = itemDataInfo.ItemPurchasePrice;
        item_sell_price= itemDataInfo.ItemSellPrice;
        item_searchTime = itemDataInfo.ItemSearchTime;
        width = itemDataInfo.Width;
        height = itemDataInfo.Height;
        isItemConsumeable = itemDataInfo.IsItemConsumeable; //�ӽ�(������ Ÿ������ ���߰���, ������ ������ �Ҹ�ǰ���� �Ǵ���. ���� �ڵ带 ���� ��ȸ�� ����)
        //itemSprite = itemDataInfo.ItemSprite;
    }

    /// <summary>
    /// ���� ��ũ��Ʈ�� ������ ItemDataInfo�� ��ȯ
    /// </summary>
    public ItemDataInfo GetItemData()
    {
        ItemDataInfo itemDataInfo = new ItemDataInfo();
        itemDataInfo.ItemId = itemId;
        itemDataInfo.ItemCode = itemCode;
        itemDataInfo.ItemPosX = itemPos.x;
        itemDataInfo.ItemPosY = itemPos.y;
        itemDataInfo.ItemRotate = itemRotate;
        itemDataInfo.ItemAmount = itemAmount;

        foreach (int id in searchedPlayerId)
        {
            itemDataInfo.SearchedPlayerId.Add(id);
        }

        //�ӽ�

        itemDataInfo.ItemName = item_name;
        itemDataInfo.ItemWeight = item_weight; //�������� ����
        itemDataInfo.ItemType = item_type;
        itemDataInfo.ItemStringValue = item_string_value;
        itemDataInfo.ItemPurchasePrice = item_purchase_price;
        itemDataInfo.ItemSellPrice = item_sell_price;
        itemDataInfo.ItemSearchTime = item_searchTime;
        itemDataInfo.Width = width;
        itemDataInfo.Height = height;
        itemDataInfo.IsItemConsumeable = isItemConsumeable;  //�ӽ�(������ Ÿ������ ���߰���, ������ ������ �Ҹ�ǰ���� �Ǵ���. ���� �ڵ带 ���� ��ȸ�� ����)
        //itemSprite = itemDataInfo.ItemSprite;

        return itemDataInfo;
    }


    [Header("������ �����ͺ��̽� ����")]
    public int itemId; // �ش� �������� ������ ���̵�
    public int itemCode; //�������� ����(�ش� �������� DB���� ��ȸ�ϱ� ���� �ڵ�)
    public Vector2Int itemPos; // �������� �׸��� �� ��ǥ���� ��ġ
    public int itemRotate; // �������� ȸ���ڵ�(rotate * 90)
    public int itemAmount = 1; // �������� ����(�Ҹ�ǰ�� 64������)
    public List<int> searchedPlayerId; // �� �������� ��ȸ�� �÷��̾��� ���̵�

    [Header("�ӽ� ��뺯��")]
    //�ӽú���(������ �ڵ带 ���� �����ͺ��̽����� �ҷ����Ⱑ �����Ҷ�����)
    public string item_name;
    public float item_weight; //�������� ����
    public int item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width = 1;
    public int height = 1;
    public bool isItemConsumeable; //�ӽ�(������ Ÿ������ ���߰���, ������ ������ �Ҹ�ǰ���� �Ǵ���. ���� �ڵ带 ���� ��ȸ�� ����)
    public Sprite itemSprite;
}
