using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Eval;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

//오브젝트 id가 myPlayer의 인벤토리면 무조건 0
public partial class InventoryController
{
    public void SendLoadInvenPacket(int objectId)
    {
        C_LoadInventory packet = new C_LoadInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendLoadInven : {objectId}의 아이템 요청 ");
    }

    public void SendCloseInvenPacket(int objectId = 0)
    {
        C_CloseInventory packet = new C_CloseInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendLoadInven : {objectId}의 인벤토리 닫음 ");
    }

    public void SendSearchItemPacket(int objectId, ItemObject item)
    {
        C_SearchItem packet = new C_SearchItem();
        packet.SourceObjectId = objectId;
        packet.SourceItemId = item.itemData.objectId; 
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId} 아이템 검색 ");
    }


    /// <summary>
    /// 아이템을 그리드 혹은 장착 슬롯에 배치
    /// </summary>
    public void SendMoveItemPacket(ItemObject item , Vector2Int pos = default)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.SourceObjectId = item.backUpParentId;
        packet.DestinationObjectId = item.parentObjId;
        packet.DestinationGridX = pos.x; //옮긴 위치
        packet.DestinationGridY = pos.y; //옮긴 위치
        packet.DestinationRotation = item.itemData.rotate; //옮겼을때의 회전
        
        packet.SourceMoveItemId = item.itemData.objectId; 

        Debug.Log($"C_MoveItem : {item.itemData.objectId} 아이템 옮김 ");
        Managers.Network.Send(packet);
    }


    public void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpParentId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_DeleteItem : {item.itemData.objectId} 아이템 삭제");
    }

    
    public void SendMergeItemPacket(ItemObject selectedItem, ItemObject overlapItem,  int itemAmount)
    {
        C_MergeItem packet = new C_MergeItem(); // merge에 combined 수량을 갯수만큼 더한다
        packet.SourceObjectId = overlapItem.backUpParentId; //combined가 있는 그리드
        packet.DestinationObjectId = selectedItem.backUpParentId;  //selectedㅇ가 있던 그리드

        packet.MergedObjectId = overlapItem.itemData.objectId; //감소하는 아이템
        packet.CombinedObjectId = selectedItem.itemData.objectId; 
        packet.MergeNumber = itemAmount;

        Managers.Network.Send(packet);
        Debug.Log("C_MergeItem");
    }

    public void SendDivideItemPacket(ItemObject devideItem, Vector2Int pos, int amount)
    {
        C_DevideItem packet = new C_DevideItem();
        packet.SourceObjectId = devideItem.backUpParentId;
        packet.DestinationObjectId = devideItem.backUpParentId;

        packet.SourceItemId = devideItem.itemData.objectId;
        packet.DestinationGridX = pos.x;
        packet.DestinationGridY = pos.y;
        packet.DestinationRotation = devideItem.itemData.rotate;
        packet.DevideNumber = amount;
        

        Managers.Network.Send(packet);
        Debug.Log("C_DevideItem");
    }

    public void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {
        //보류
    }

}


