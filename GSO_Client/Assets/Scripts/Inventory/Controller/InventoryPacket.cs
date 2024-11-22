using Google.Protobuf.Protocol;
using Org.BouncyCastle.Bcpg;
using UnityEngine;

public class InventoryPacket
{
    public static void SendLoadInvenPacket(int objectId = 0)
    {
        C_LoadInventory packet = new C_LoadInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendLoadInven : {objectId}�� �κ��丮 ��û ����");
    }

    public static void SendCloseInvenPacket(int objectId = 0)
    {
        C_CloseInventory packet = new C_CloseInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendCloseInven : {objectId}�� �κ��丮 ���� �ൿ ����");
    }

    public static void SendSearchItemPacket(int objectId, ItemObject item)
    {
        C_SearchItem packet = new C_SearchItem();
        packet.SourceObjectId = objectId;
        packet.SourceItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId}�� �˻� �ൿ ����");
    }


    public static void SendMoveItemPacket(ItemObject item, Vector2Int pos = default)
    {
        C_MoveItem packet = new C_MoveItem();

        packet.SourceObjectId = item.backUpParentId;
        packet.DestinationObjectId = item.parentObjId;
        packet.DestinationGridX = pos.x;
        packet.DestinationGridY = pos.y;
        packet.DestinationRotation = item.itemData.rotate;

        packet.SourceMoveItemId = item.itemData.objectId;

        Debug.Log($"C_MoveItem : {item.itemData.objectId}�� ������ �̵� ��û ����");
        Managers.Network.Send(packet);
    }


    public static void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpParentId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_DeleteItem : {item.itemData.objectId}������ ���� ��û ����");
    }


    public static void SendMergeItemPacket(ItemObject selectedItem, ItemObject overlapItem, int itemAmount)
    {
        C_MergeItem packet = new C_MergeItem();
        packet.SourceObjectId = overlapItem.backUpParentId;
        packet.DestinationObjectId = selectedItem.backUpParentId;

        packet.MergedObjectId = overlapItem.itemData.objectId;
        packet.CombinedObjectId = selectedItem.itemData.objectId;
        packet.MergeNumber = itemAmount;

        Managers.Network.Send(packet);
        Debug.Log($"C_MergeItem : {selectedItem.itemData.objectId}�� {overlapItem.itemData.objectId}�� ��ħ ��û ����");
    }

    public static void SendDivideItemPacket(ItemObject devideItem, Vector2Int pos, int amount)
    {
        C_DevideItem packet = new C_DevideItem();
        packet.SourceObjectId = devideItem.backUpParentId;
        packet.DestinationObjectId = devideItem.parentObjId;

        packet.SourceItemId = devideItem.itemData.objectId;
        packet.DestinationGridX = pos.x;
        packet.DestinationGridY = pos.y;
        packet.DestinationRotation = devideItem.itemData.rotate;
        packet.DevideNumber = amount;

        Managers.Network.Send(packet);
        Debug.Log($"C_DevideItem : {devideItem.itemData.objectId}�� �и� ��û ����");
    }

    public static void SendUseItemPacket(ItemObject useItem)
    {
        //�������� ����� ��� ������ ��û ����
    }
}
