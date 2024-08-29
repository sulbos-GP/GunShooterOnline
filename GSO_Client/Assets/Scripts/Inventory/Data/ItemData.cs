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
     * 스크립터블 오브젝트로 아이템의 데이터를 정의합니다.
     * itemId(아이템 종류에 따른 코드) , 아이템의 이름, 아이템을 검색하는 시간, 크기, 이미지
     * 를 설정하여 아이템 프리팹에 부착됩니다.
     * 
     * 실제로는 컨트롤러의 리스트에 넣어 컨트롤러에서 생성할때 리스트 안의 데이터중 하나를 
     * 부착시킵니다.
     */

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
        Data_Item itemDB = Data_Item.GetData(itemId);
        item_name = itemDB.name;
        item_weight = Math.Round(itemDB.weight,2); //아이템의 무게
        
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
        return itemInfo;
    }

    [Header("아이템 데이터베이스 변수")]
    public int objectId; // 해당 아이템의 고유한 아이디  -> 오브젝트 아이디
    public int itemId; //아이템의 종류(해당 아이템을 DB에서 조회하기 위한 코드) -> 아이템 아이디
    public Vector2Int pos; // 아이템의 그리드 안 좌표상의 위치
    public int rotate; // 아이템의 회전코드(rotate * 90)
    public int amount; // 아이템의 개수(소모품만 64개까지)
    public bool isSearched; // 이 아이템을 조회한 플레이어의 아이디

    public string item_name;
    public double item_weight;
    public ItemType item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width;
    public int height;

    public bool isItemConsumeable; //임시(아이템 타입으로 유추가능, 아이템 머지에 소모품인지 판단함. 이후 코드를 통해 조회로 변경)
    public string icon;
}
