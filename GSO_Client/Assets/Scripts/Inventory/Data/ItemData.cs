using Google.Protobuf.Protocol;
using NPOI.HPSF;
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

    [Header("임시 사용변수")]
    //임시변수(아이템 코드를 통해 데이터베이스에서 불러오기가 가능할때까지)
    public string item_name;
    public float item_weight; //아이템의 무게
    public ItemType item_type;
    public int item_string_value;
    public int item_purchase_price;
    public int item_sell_price;
    public float item_searchTime;
    public int width;
    public int height;
    public bool isItemConsumeable; //임시(아이템 타입으로 유추가능, 아이템 머지에 소모품인지 판단함. 이후 코드를 통해 조회로 변경)
    //public Sprite itemSprite;
}
