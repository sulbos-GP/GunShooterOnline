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
        Debug.Log($"C_SendLoadInven : {objectId}의 인벤토리 요청 전송");
    }

    public static void SendCloseInvenPacket(int objectId = 0)
    {
        C_CloseInventory packet = new C_CloseInventory();
        packet.SourceObjectId = objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SendCloseInven : {objectId}의 인벤토리 종료 행동 전송");
    }

    public static void SendSearchItemPacket(int objectId, ItemObject item)
    {
        C_SearchItem packet = new C_SearchItem();
        packet.SourceObjectId = objectId;
        packet.SourceItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_SearchInventory : {item.itemData.objectId}의 검색 행동 전송");
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

        Debug.Log($"C_MoveItem : {item.itemData.objectId}의 아이템 이동 요청 전송");
        Managers.Network.Send(packet);
    }


    public static void SendDeleteItemPacket(ItemObject item)
    {
        C_DeleteItem packet = new C_DeleteItem();
        packet.SourceObjectId = item.backUpParentId;
        packet.DeleteItemId = item.itemData.objectId;
        Managers.Network.Send(packet);
        Debug.Log($"C_DeleteItem : {item.itemData.objectId}아이템 삭제 요청 전송");
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
        Debug.Log($"C_MergeItem : {selectedItem.itemData.objectId}을 {overlapItem.itemData.objectId}에 합침 요청 전송");
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
        Debug.Log($"C_DevideItem : {devideItem.itemData.objectId}의 분리 요청 전송");
    }

    public static void SendUseItemPacket(ItemObject useItem)
    {
        //아이템을 사용한 경우 서버에 요청 전송
    }
}
