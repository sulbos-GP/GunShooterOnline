using Google.Protobuf.Protocol;
using NPOI.HPSF;
using NPOI.OpenXmlFormats.Dml.Chart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using WebCommonLibrary.Models.GameDB;



[System.Serializable]
public class ItemData
{
    #region DBGearUnit
    public void SetItemData(DB_GearUnit gear)
    {
        //objectId = itemInfo.ObjectId;
        itemId = gear.attributes.item_id;
        amount = gear.attributes.amount;
        isSearched = true;

        Data_master_item_base itemDB = Data_master_item_base.GetData(itemId);
        item_name = itemDB.name;
        item_weight = Math.Round(itemDB.weight, 2); //�������� ����

        switch (itemDB.type)
        {
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

        durability = gear.attributes.durability == 0 ? 0 : gear.attributes.durability;
        loadedAmmo = gear.attributes.loaded_ammo == 0 ? 0 : gear.attributes.loaded_ammo;

        isItemConsumeable = itemDB.type == eITEM_TYPE.Recovery || itemDB.type == eITEM_TYPE.Bullet;
        iconName = itemDB.icon;

    }
    #endregion

    #region DB_ItemUnit
    public void SetItemData(DB_ItemUnit item)
    {
        //objectId = itemInfo.ObjectId;
        itemId = item.attributes.item_id;
        pos = new Vector2Int(item.storage.grid_x, item.storage.grid_y);
        rotate = item.storage.rotation;
        amount = item.attributes.amount;

        Data_master_item_base itemDB = Data_master_item_base.GetData(itemId);
        item_name = itemDB.name;
        item_weight = Math.Round(itemDB.weight, 2);

        switch (itemDB.type)
        {
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

        durability = item.attributes.durability == 0 ? 0 : item.attributes.durability;
        loadedAmmo = item.attributes.loaded_ammo == 0 ? 0 : item.attributes.loaded_ammo;

        isItemConsumeable = itemDB.type == eITEM_TYPE.Recovery || itemDB.type == eITEM_TYPE.Bullet;
        iconName = itemDB.icon;

    }

    #endregion

    #region Ps_ItemInfo
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
    #endregion


    public int objectId;  
    public int itemId;   
    public Vector2Int pos; 
    public int rotate;      
    public int amount;

    public bool isSearched;

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
    public int durability;
    public int loadedAmmo;
}
