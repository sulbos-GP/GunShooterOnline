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


public partial class InventoryController : MonoBehaviour
{
    /// <summary>
    /// C_MoveItem패킷 생성 및 전송
    /// </summary>
    private void SendMoveItemInGridPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ItemData = item.itemData.GetItemData(),
            TargetId = item.curItemGrid.ownInven.invenData.inventoryId,
            GridId = item.curItemGrid.gridData.gridId,

            LastItemPosX = item.backUpItemPos.x,
            LastItemPosY = item.backUpItemPos.y,
            LastItemRotate = item.backUpItemRotate,
            LastGridId = item.backUpItemGrid.gridData.gridId
        };
        Managers.Network.Send(packet);
    }
}


