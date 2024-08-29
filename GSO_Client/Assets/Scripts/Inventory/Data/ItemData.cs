using Google.Protobuf.Protocol;
using NPOI.HPSF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;



[System.Serializable]
public class ItemData
{

    /*
     * ��ũ���ͺ� ������Ʈ�� �������� �����͸� �����մϴ�.
     * itemId(������ ������ ���� �ڵ�) , �������� �̸�, �������� �˻��ϴ� �ð�, ũ��, �̹���
     * �� �����Ͽ� ������ �����տ� �����˴ϴ�.
     * 
     * �����δ� ��Ʈ�ѷ��� ����Ʈ�� �־� ��Ʈ�ѷ����� �����Ҷ� ����Ʈ ���� �������� �ϳ��� 
     * ������ŵ�ϴ�.
     */

    /// <summary>
    /// ItemDataInfo�� �ش� ��ũ��Ʈ�� ������ ����
    /// </summary>
    public void SetItemData(PS_ItemInfo itemInfo)
    {
        objectId = itemInfo.ObjectId;
        itemId = itemInfo.ItemId;
        pos = new Vector2Int(itemInfo.X, itemInfo.Y);
        rotate = itemInfo.Rotate;
        amount = itemInfo.Amount;

        isSearched = itemInfo.IsSearched;
        Data_Item itemDB = Data_Item.GetData(itemId);
        item_name = itemDB.name;
        item_weight = Math.Round(itemDB.weight,2); //�������� ����
        
        switch (itemDB.type) {
            case eITEM_TYPE.weapone: item_type = ItemType.Weapon; break;
            case eITEM_TYPE.defensive: item_type = ItemType.Defensive; break;
            case eITEM_TYPE.bag: item_type = ItemType.Bag; break;
            case eITEM_TYPE.recovery: item_type = ItemType.Recovery; break;
            case eITEM_TYPE.bullet: item_type = ItemType.Bullet; break;
            case eITEM_TYPE.spoil: item_type = ItemType.Spoil; break;
        }

        item_string_value = itemDB.description;
        item_purchase_price = itemDB.purchase_price;
        item_sell_price = itemDB.sell_price;
        item_searchTime = itemDB.inquiry_time;
        width = itemDB.scale_x;
        height = itemDB.scale_y;
        isItemConsumeable = itemDB.type == eITEM_TYPE.recovery || itemDB.type == eITEM_TYPE.bullet;
        icon = itemDB.icon;
    }

    /// <summary>
    /// ���� ��ũ��Ʈ�� ������ ItemDataInfo�� ��ȯ
    /// </summary>
    public PS_ItemInfo GetItemData()
    {
        PS_ItemInfo itemInfo = new PS_ItemInfo();
        itemInfo.ObjectId = objectId;
        itemInfo.ItemId = itemId;
        itemInfo.X = pos.x;
        itemInfo.Y = pos.y;
        itemInfo.Rotate = rotate;
        itemInfo.Amount = amount;
        itemInfo.IsSearched = isSearched;
        return itemInfo;
    }

    [Header("������ �����ͺ��̽� ����")]
    public int objectId; // �ش� �������� ������ ���̵�  -> ������Ʈ ���̵�
    public int itemId; //�������� ����(�ش� �������� DB���� ��ȸ�ϱ� ���� �ڵ�) -> ������ ���̵�
    public Vector2Int pos; // �������� �׸��� �� ��ǥ���� ��ġ
    public int rotate; // �������� ȸ���ڵ�(rotate * 90)
    public int amount; // �������� ����(�Ҹ�ǰ�� 64������)
    public bool isSearched; // �� �������� ��ȸ�� �÷��̾��� ���̵�

    public string item_name;
    public double item_weight;
    public ItemType item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width;
    public int height;

    public bool isItemConsumeable; //�ӽ�(������ Ÿ������ ���߰���, ������ ������ �Ҹ�ǰ���� �Ǵ���. ���� �ڵ带 ���� ��ȸ�� ����)
    public string icon;
}
