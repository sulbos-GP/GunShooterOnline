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

    private void SendSearchItemPacket(int objectId, ItemObject item)
    {
        C_SearchItem packet = new C_SearchItem();
        packet.SourceObjectId = objectId;
        packet.SourceItemId = item.itemData.objectId; 
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId} ������ �˻� ");
    }


    /// <summary>
    /// �׸��� -> �׸��� Ȥ�� �������� -> �׸���
    /// </summary>
    private void SendMoveItemPacket(ItemObject item , Vector2Int pos)
    {
        C_MoveItem packet = new C_MoveItem();
        
        packet.SourceObjectId = item.backUpItemGrid.objectId;  //��� ������ id
        packet.DestinationObjectId = item.curItemGrid.objectId;  //���� ������id
        packet.SourceMoveItemId = item.itemData.objectId; //�ű� ������ ���̵�
        packet.DestinationGridX = pos.x; //�ű� ��ġ
        packet.DestinationGridY = pos.y; //�ű� ��ġ
        packet.DestinationRotation = item.itemData.rotate; //�Ű������� ȸ��
        Debug.Log($"C_SearchInventory : {item.itemData.objectId} ������ �ű� ");
        Managers.Network.Send(packet);
    }


    private void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpItemGrid.objectId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId} ������ ����");
    }

    private void SendEquipItemPacket(ItemObject item, int equipSlotCode)
    {
        //����
    }

    private void SendMergeItemPacket(ItemObject selectedItem, ItemObject overlapItem,  int itemAmount)
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
}


