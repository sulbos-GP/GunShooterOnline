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
    /// ItemDataInfo를 해당 스크립트의 변수에 적용
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
        item_weight = Math.Round(itemDB.weight,2); //아이템의 무게

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
    /// 현재 스크립트의 변수를 ItemDataInfo로 변환
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

    public int objectId;    // 해당 데이터의 고유하며 유일한 아이디
    public int itemId;      // 아이템의 종류를 명시하는 코드
    public Vector2Int pos;  // 아이템의 그리드 안 좌표상의 위치
    public int rotate;      // 아이템의 회전코드(rotate * 90)
    public int amount;

    public bool isSearched; //클라입장에서 이 아이템이 검색되었는지 확인

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
    public int durability; //해당 아이템이 방어구인 경우 남은 내구도
    public int loadedAmmo; //해당 아이템이 총인 경우 해당 아이템에 장전된 총알 수
}
