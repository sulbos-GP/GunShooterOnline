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

//������Ʈ id�� myPlayer�� �κ��丮�� ������ 0
public partial class InventoryController
{
    //��Ŷ�� ���� �뵵�� �ٲ���⿡ ������ �ʿ�
    private void SendMoveItemPacket(ItemObject item, Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.SourceObjectId = 0; 
        packet.DestinationObjectId = 0; 
        packet.SourceMoveItemId = item.itemData.objectId;
        packet.DestinationGridX = pos.x;
        packet.DestinationGridY = pos.y;
        packet.DestinationRotation = item.itemData.rotate;

        Managers.Network.Send(packet);
    }


    private void SendDelteItemPacket()
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = 0;
        packet.DeleteItemId = SelectedItem.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log("C_DeleteItem");
    }

    private void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {
        //����
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
        //�и��� ���ο� ������ ������Ʈ ������ �װ��� ��ġ�������� ��ġ
        packet.DestinationGridX = newItem.itemData.pos.x;
        packet.DestinationGridY = newItem.itemData.pos.y;
        packet.DestinationRotation = newItem.itemData.rotate;
        packet.DevideNumber = 0; //�ӽ�

        Managers.Network.Send(packet);
        Debug.Log("C_DevideItem");
    }
}


