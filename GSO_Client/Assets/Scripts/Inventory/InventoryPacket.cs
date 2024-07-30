using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//그리드의 데이터
/*
public class SGrid
{
    public Vector2Int gridSize; // 해당 그리드의 크기 (x, y 축 셀 갯수)
    public Vector2 gridPos;// 해당 그리드의 인벤토리 내부 로컬 위치
    public List<SItem> itemList;// 해당 그리드안에 있는 아이템
}*/

//아이템의 데이터
/*
public class SItem
{
    public int itemId; // 해당 아이템의 고유한 아이디
    public int itemCode; // 해당 아이템을 DB에서 조회하기 위한 코드
    public Vector2Int itemPos; // 아이템의 그리드 안 좌표상의 위치
    public int itemRotate; // 아이템의 회전코드(rotate * 90)
    public int itemAmount; // 아이템의 개수(소모품만 64개까지)
    public List<int> searchedPlayerList; // 이 아이템을 조회한 플레이어의 아이디
}*/

/*
public class SInven
{
    public float limitWeight; //해당 인벤토리의 한계무게
    public List<GridData> gridList; //이 인벤토리에서 생성될 그리드의 데이터
}
*/


//클라에서 서버로의 패킷에는 시간 추가할것

//인벤토리 요청과 응답에 대한 패킷
/// <summary>
/// 클라이언트가 서버에게 해당 id의 인벤토리의 id를 요청
/// 1. 플레이어가 최초로 로드될때 플레이어의 인벤토리 id로 요청
/// 2. 플레이어가 인벤토리를 소유한 오브젝트와 인터렉트 할때 해당 인벤토리 아이디로 요청
/// </summary>
public class C_LoadInventory 
{
    //인터렉션한 플레이어와 불러올 인벤토리 아이디
    public int playerId;
    public int inventoryId;
}

/// <summary>
/// 서버가 요청을 보낸 플레이어에게 해당 인벤토리의 정보를 전송함
/// 클라는 이 내용을 바탕으로 해당 인벤토리에 다음 내용을 적용시킨후 SET
/// </summary>
public class S_LoadInventory
{

    // 클라이언트의 요청에 서버에서 보내주는 인벤토리의 내용
    public int inventoryId; // 해당 인벤토리의 아이디

    public InvenData inventoryData;

    /* 보류(충분히 그리드 리스트 조회만으로 유추 가능
    public float curWeight; // 현재 무게(모든 그리드들의 무게의 합, 솔직히 그리드 리스트만 주면 나오긴 함)
                            // 그리드는 아이템의 데이터를 담고 아이템의 코드를 통해 데이터베이스에서 무게를 조회할수 있기때문
    */
}


//플레이어가 인벤토리의 내용을 옮길경우
/// <summary>
/// 플레이어가 아이템을 다른 위치에 배치했을때의 아이템의 정보를 서버에 전송
/// </summary>
public class C_MoveItem
{
    public int playerId; //옮긴 플레이어의 id
    public int itemId; //옮긴 아이템의 데이터
    public Vector2Int itemPos; //아이템의 옮긴 좌표
    public int itemRotate;
    public int inventoryId; // 아이템을 옮긴  인벤토리의 id
    public int gridNum; //인벤토리의 몇번째 그리드인지(혹은 그리드의 id)
    
    //문제가 있을시 백업용. 필요하나?
    public Vector2Int lastItemPos; //이전 아이템의 좌표
    public int lastItemRotate;
    public int lastInventoryId; // 이전 인벤토리의 id
    public int lastGridNum; //이전 인벤토리의 몇번재 그리드인지
}

/// <summary>
/// 서버가 다른 클라이언트에게 해당 아이템이 어디에 배치되어 있는지를 전송
/// 클라는 이 아이템을 해당 그리드의 자식으로 배치하고 위치와 회전을 변경
/// </summary>
public class S_MoveItem
{
    //어떤 아이템이 어느위치에 어느 회전으로 어디에 배치되었는지
    public int itemId;
    public Vector2Int itemPos; //아이템의 옮긴 좌표
    public int itemRotate;
    public int inventoryId; // 아이템을 옮긴  인벤토리의 id
    public int gridNum; //인벤토리의 몇번째 그리드인지(혹은 그리드의 id)
}


//플레이어가 아이템을 삭제한 경우
/// <summary>
/// 클라에서 서버로 삭제한 아이템의 정보 전송
/// </summary>
public class C_DeleteItem
{
    //삭제한 플레이어의 Id와 삭제한 item의 id 전송
    public int playerId;
    public int itemId;
}

/// <summary>
/// 서버에서 모든 클라이언트에 해당 아이템이 삭제되었다는 정보 전송
/// 클라는 해당 아이디를 가진 아이템을 검색해 오브젝트와 슬롯 내에서 제거
/// </summary>
public class S_DeleteItem
{
    //아이템id로 검색하여 해당 아이템 삭제
    public int itemId;
}