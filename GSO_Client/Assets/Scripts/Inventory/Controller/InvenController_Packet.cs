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



    /// <summary>
    /// 그리드 -> 그리드 혹은 장착슬롯 -> 그리드
    /// </summary>
    private void SendMoveItemPacket(ItemObject item , Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();
        
        packet.SourceObjectId = item.backUpItemGrid.objectId;  //출발 소유자 id
        packet.DestinationObjectId = item.curItemGrid.objectId;  //도착 소유자id
        packet.SourceMoveItemId = item.itemData.objectId; //옮긴 아이템 아이디
        packet.DestinationGridX = pos.x; //옮긴 위치
        packet.DestinationGridY = pos.y; //옮긴 위치
        packet.DestinationRotation = item.itemData.rotate; //옮겼을때의 회전

        Managers.Network.Send(packet);
    }


    private void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpItemGrid.objectId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log("C_DeleteItem");
    }

    private void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {
        //보류
    }

    private void SendMergeItemPacket(ItemObject item, ItemObject targetItem , int mergeAmount)
    {
        C_MergeItem packet = new C_MergeItem();
        packet.SourceObjectId = 0;
        packet.DestinationObjectId = 0;

        packet.MergedObjectId = item.itemData.objectId;
        packet.CombinedObjectId = targetItem.itemData.objectId;
        packet.MergeNumber = mergeAmount;

        Managers.Network.Send(packet);
        Debug.Log("C_MergeItem");
    }

    private void SendDevideItemPacket(ItemObject DevideItem, ItemObject newItem)
    {
        C_DevideItem packet = new C_DevideItem();
        packet.SourceObjectId = 0;
        packet.DestinationObjectId = 0;

        packet.SourceItemId = DevideItem.itemData.objectId;
        //분리된 새로운 아이템 오브젝트 생성후 그것을 배치했을때의 위치
        packet.DestinationGridX = newItem.itemData.pos.x;
        packet.DestinationGridY = newItem.itemData.pos.y;
        packet.DestinationRotation = newItem.itemData.rotate;
        packet.DevideNumber = 0; //임시

        Managers.Network.Send(packet);
        Debug.Log("C_DevideItem");
    }
}


