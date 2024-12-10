using Google.Protobuf.Protocol;
using NPOI.HPSF;
using NPOI.OpenXmlFormats.Dml.Chart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;



[System.Serializable]
public class ItemData
{
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
        Data_master_item_base itemDB = Data_master_item_base.GetData(itemId);
        item_name = itemDB.name;
        item_weight = Math.Round(itemDB.weight,2); //�������� ����

        switch (itemDB.type) {
            case eITEM_TYPE.Weapone: item_type = ItemType.Weapon; break;
            case eITEM_TYPE.Defensive: item_type = ItemType.Defensive; break;
            case eITEM_TYPE.Bag: item_type = ItemType.Bag; break;
            case eITEM_TYPE.Recovery: item_type = ItemType.Recovery; break;
            case eITEM_TYPE.Bullet: item_type = ItemType.Bullet; break;
            case eITEM_TYPE.Spoil: item_type = ItemType.Spoil; break;
        }

        item_string_value = itemDB.description;
        item_purchase_price = itemDB.purchase_price;
        item_sell_price = itemDB.sell_price;
        item_searchTime = itemDB.inquiry_time;
        width = itemDB.scale_x;
        height = itemDB.scale_y;

        PS_ItemAttributes attribute = itemInfo.Attributes;
        durability = attribute.Durability == 0 ? 0 : attribute.Durability;
        loadedAmmo = attribute.LoadedAmmo == 0 ? 0 : attribute.LoadedAmmo;

        isItemConsumeable = itemDB.type == eITEM_TYPE.Recovery || itemDB.type == eITEM_TYPE.Bullet;
        iconName = itemDB.icon;
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

        PS_ItemAttributes attribute = new PS_ItemAttributes();
        attribute.Durability = durability;
        attribute.LoadedAmmo = loadedAmmo;
        itemInfo.Attributes = attribute;

        return itemInfo;
    }

    public int objectId;    // �ش� �������� �����ϸ� ������ ���̵�
    public int itemId;      // �������� ������ �����ϴ� �ڵ�
    public Vector2Int pos;  // �������� �׸��� �� ��ǥ���� ��ġ
    public int rotate;      // �������� ȸ���ڵ�(rotate * 90)
    public int amount;

    public bool isSearched; //Ŭ�����忡�� �� �������� �˻��Ǿ����� Ȯ��

    public string item_name;
    public double item_weight;
    public ItemType item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width;
    public int height;
    public bool isItemConsumeable;
    public string iconName;

    //PS_ItemAttributes
    public int durability; //�ش� �������� ���� ��� ���� ������
    public int loadedAmmo; //�ش� �������� ���� ��� �ش� �����ۿ� ������ �Ѿ� ��
}
