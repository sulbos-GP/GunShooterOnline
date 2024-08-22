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
    /// C_MoveItem��Ŷ ���� �� ����
    /// </summary>
    private void SendMoveItemInGridPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.PlayerId = Managers.Object.MyPlayer.Id;
        packet.ItemData = item.itemData.GetItemData();
        packet.TargetId = item.curItemGrid.ownInven.invenData.inventoryId;
        packet.GridId = item.curItemGrid.gridData.gridId;
            
        if(item.backUpItemGrid != null)
        { //����ĭ���� �׸���� �ű��쿡�� ��� �׸��尡 ����
            packet.LastItemPosX = item.backUpItemPos.x;
            packet.LastItemPosY = item.backUpItemPos.y;
            packet.LastItemRotate = item.backUpItemRotate;
            packet.LastGridId = item.backUpItemGrid.gridData.gridId;
        }
            
        Managers.Network.Send(packet);
    }

    private void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {

    }
}


