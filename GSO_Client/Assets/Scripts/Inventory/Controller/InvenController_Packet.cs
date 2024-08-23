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
    //��Ŷ�� ���� �뵵�� �ٲ���⿡ ������ �ʿ�
    private void SendMoveItemInGridPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.SourceObjectId = Managers.Object.MyPlayer.Id; //�ű�� �� ��ġ
        packet.DestinationObjectId = Managers.Object.MyPlayer.Id; //�ű� �� ��ġ
        packet.SourceMoveItem = item.itemData.GetItemData();
        packet.DestinationGridX = pos.x;
        packet.DestinationGridY = pos.y;

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


