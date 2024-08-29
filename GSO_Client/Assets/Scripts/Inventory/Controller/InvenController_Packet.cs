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

        packet.SourceObjectId = item.backUpParentId;
        packet.DestinationObjectId = item.parentObjId;
        packet.DestinationGridX = pos.x; //�ű� ��ġ
        packet.DestinationGridY = pos.y; //�ű� ��ġ
        packet.DestinationRotation = item.itemData.rotate; //�Ű������� ȸ��
        
        packet.SourceMoveItemId = item.itemData.objectId; 

        Debug.Log($"C_MoveItem : {item.itemData.objectId} ������ �ű� ");
        Managers.Network.Send(packet);
    }


    public void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpParentId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_DeleteItem : {item.itemData.objectId} ������ ����");
    }

    
    public void SendMergeItemPacket(ItemObject selectedItem, ItemObject overlapItem,  int itemAmount)
    {
        C_MergeItem packet = new C_MergeItem(); // merge�� combined ������ ������ŭ ���Ѵ�
        packet.SourceObjectId = overlapItem.backUpParentId; //combined�� �ִ� �׸���
        packet.DestinationObjectId = selectedItem.backUpParentId;  //selected���� �ִ� �׸���

        packet.MergedObjectId = overlapItem.itemData.objectId; //�����ϴ� ������
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
        //����
    }

}


