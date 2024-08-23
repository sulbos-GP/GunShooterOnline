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


public partial class InventoryController
{
    /// <summary>
    /// C_MoveItem패킷 생성 및 전송
    /// </summary>
    private void SendMoveItemInGridPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.TargetObjectId = Managers.Object.MyPlayer.Id; //옮긴 인벤토리의 아이디
        packet.MoveItem = item.itemData.GetItemData();
        packet.GridX = pos.x;
        packet.GridY = pos.y;
            
        
        Managers.Network.Send(packet);
    }


    private void ItemDeletePacket()
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.DeleteItemId = SelectedItem.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log("C_DeleteItem");
    }

    private void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {

    }
}


