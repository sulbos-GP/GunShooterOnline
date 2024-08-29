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
    public void SendLoadInvenPacket(int objectId)
    {
        C_LoadInventory packet = new C_LoadInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendLoadInven : {objectId}�� ������ ��û ");
    }

    public void SendCloseInvenPacket(int objectId = 0)
    {
        C_CloseInventory packet = new C_CloseInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendLoadInven : {objectId}�� �κ��丮 ���� ");
    }

    public void SendSearchItemPacket(int objectId, ItemObject item)
    {
        C_SearchItem packet = new C_SearchItem();
        packet.SourceObjectId = objectId;
        packet.SourceItemId = item.itemData.objectId; 
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId} ������ �˻� ");
    }


    /// <summary>
    /// �������� �׸��� Ȥ�� ���� ���Կ� ��ġ
    /// </summary>
    public void SendMoveItemPacket(ItemObject item , Vector2Int pos = default)
    {
        C_MoveItem packet = new C_MoveItem();

        if (item.backUpItemGrid != null) 
        {
            packet.SourceObjectId = item.backUpItemGrid.objectId;
        }
        else if(item.backUpEquipSlot != null)
        {
            packet.SourceObjectId = item.backUpEquipSlot.slotId;
        }

        if (item.curItemGrid != null)
        {
            packet.DestinationObjectId = item.curItemGrid.objectId;
            packet.DestinationGridX = pos.x; //�ű� ��ġ
            packet.DestinationGridY = pos.y; //�ű� ��ġ
            packet.DestinationRotation = item.itemData.rotate; //�Ű������� ȸ��
        }
        else if (item.curEquipSlot != null) 
        {
            packet.DestinationObjectId = item.curEquipSlot.slotId;
            packet.DestinationGridX = 0; //�ű� ��ġ
            packet.DestinationGridY = 0; //�ű� ��ġ
            packet.DestinationRotation = 0; //�Ű������� ȸ��
        }

        packet.SourceMoveItemId = item.itemData.objectId; 

        
        Debug.Log($"C_MoveItem : {item.itemData.objectId} ������ �ű� ");
        Managers.Network.Send(packet);
    }


    public void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpItemGrid.objectId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_DeleteItem : {item.itemData.objectId} ������ ����");
    }

    
    public void SendMergeItemPacket(ItemObject selectedItem, ItemObject overlapItem,  int itemAmount)
    {
        C_MergeItem packet = new C_MergeItem(); // merge�� combined ������ ������ŭ ���Ѵ�
        packet.SourceObjectId = overlapItem.backUpItemGrid.objectId; //combined�� �ִ� �׸���
        packet.DestinationObjectId = selectedItem.backUpItemGrid.objectId;  //selected���� �ִ� �׸���

        packet.MergedObjectId = overlapItem.itemData.objectId; //�����ϴ� ������
        packet.CombinedObjectId = selectedItem.itemData.objectId; 
        packet.MergeNumber = itemAmount;

        Managers.Network.Send(packet);
        Debug.Log("C_MergeItem");
    }

    public void SendDivideItemPacket(ItemObject devideItem, Vector2Int pos, int amount)
    {
        C_DevideItem packet = new C_DevideItem();
        packet.SourceObjectId = devideItem.backUpItemGrid.objectId;
        packet.DestinationObjectId = devideItem.curItemGrid.objectId;

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
        //����
    }

}


