using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

internal class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S_EnterGameHandler");
        var enterGamePacket = (S_EnterGame)packet;
        Debug.Log($"{enterGamePacket.Player}");

        Managers.Object.Add(enterGamePacket.Player, true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        var leaveGamePacket = (S_LeaveGame)packet;
    }

    /*public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        //게임에 접속이되면
        Debug.Log("S_ConnectedHandler");

        var lobbyInfo = new C_LobbyInfo();
        lobbyInfo.DeviceId = SystemInfo.deviceUniqueIdentifier;


        Managers.Network.Send(lobbyInfo);
    }*/

    /*public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        var spawnPacket = (S_Spawn)packet;
        foreach (var info in spawnPacket.Objects) Managers.Object.Add(info, false);
        //Debug.Log("S_SpawnHandler");
    }*/

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        var despawn = (S_Despawn)packet;

        foreach (var id in despawn.ObjcetIds) Managers.Object.Remove(id);
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        //전투중
        //Debug.Log("S_MoveHandler");
        Debug.Log("핸들러 수신");
        var movePacket = packet as S_Move;

        var go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        var cc = go.GetComponent<BaseController>();
        if (cc == null)
            return;

        cc.PosInfo = movePacket.PositionInfo;
        //cc.State = CreatureState.Moving;
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage message)
    {
        var changeHpPacket = message as S_ChangeHp;

        var go = Managers.Object.FindById(changeHpPacket.ObjectId);

        if (go != null)
            go.GetComponent<CreatureController>().Hp = changeHpPacket.Hp;
        else
            Debug.Log("아이디 없음");
    }

    public static void S_DieHandler(PacketSession session, IMessage message)
    {
        var diePacket = message as S_Die;
        var go = Managers.Object.FindById(diePacket.ObjectId);
        if (go != null)
            go.GetComponent<CreatureController>().OnDead(diePacket.AttackerId);
        else
            Debug.Log("아이디 없음");
    }


    internal static void S_RoomInfoHandler(PacketSession session, IMessage message)
    {
       /* var roomPacket = message as S_RoomInfo;

        if (roomPacket.RoomInfos.Count > 1)
            Managers.Map.MapRoomInfoInit(roomPacket);
        else if (roomPacket.RoomInfos.Count == 1)
            Managers.Map.MapRoomUpdate(roomPacket);*/
    }

    internal static void S_SpawnHandler(PacketSession session, IMessage message)
    {
        throw new NotImplementedException();
    }

    internal static void S_ConnectedHandler(PacketSession session, IMessage message)
    {
        S_Connected packet= message as S_Connected;
        Debug.Log("S_ConnectedHandler");

    }

    internal static void S_LoadInventoryHandler(PacketSession session, IMessage message)
    {
        S_LoadInventory packet  = message as S_LoadInventory;
        if(packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_LoadInventory");

        //인벤 데이터 생성 및 패킷의 InvenDataInfo를 InvenData로 변환
        InvenData newInvenData = new InvenData();
        newInvenData.SetInvenData(packet.InvenData);

        //플레이어의 인벤토리id와 패킷내의 인벤토리id 비교 -> 같으면 플레이어의 인벤토리에 반영 다르면 아더 인벤토리에 반영
        if (Managers.Object.MyPlayer.myPlayerInven.InvenId == packet.InventoryId)
        {
            //플레이어 인벤토리에 패킷의 invenData 적용
            Managers.Object.MyPlayer.myPlayerInven.invenData = newInvenData;
        }
        else
        {
            //아더 인벤토리에 패킷의 invenData 적용
            Managers.Object.MyPlayer.myOtherInven.invenData= newInvenData;
        }
    }

    internal static void S_MoveItemHandler(PacketSession session, IMessage message)
    {
        S_MoveItem packet = message as S_MoveItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_MoveItem");

        //패킷의 id를 아이템 오브젝트의 리스트에서 검색 해당 아이템의 ItemObject 스크립트 불러옴
        ItemObject moveItemObj;
        bool searchId = Managers.Object._itemDic.TryGetValue(packet.ItemId,out moveItemObj);
        if (searchId == false)
        {
            Debug.Log("옮기려는 아이템이 존재하지 않음(검색실패)");
            return;
        }

        //ItemObject.itemData에 변경된 내용들을 변경하고 ItemObject.Set을 통해 아이템의 위치 변경
        moveItemObj.curItemPos = new Vector2Int(packet.ItemPosX, packet.ItemPosY);
        moveItemObj.curItemRotate = packet.ItemRotate;
        Managers.Object._gridDic.TryGetValue(packet.gridId, out moveItemObj.curItemGrid);
        moveItemObj.backUpItemPos = new Vector2Int(packet.lastItemPosX, packet.lastItemPosY);
        moveItemObj.backUpItemRotate = packet.lastItemRotate;
        Managers.Object._gridDic.TryGetValue(packet.lastGridId, out moveItemObj.backUpItemGrid);

        //lastGridId를 통해 그리드를 검색하여 해당 그리드의 inventoryGrid.CleanItemSlot(item)으로 이전 그리드에서 해당 아이템을 지움
        moveItemObj.backUpItemGrid.CleanItemSlot(moveItemObj);

        //gridId를 통해 그리드를 검색하여 해당 그리드의 inventoryGrid.PlaceItem(item, item.posX, item.posY)로 현재 그리드에 해당 아이템 배치
        moveItemObj.curItemGrid.PlaceItem(moveItemObj, moveItemObj.curItemPos.x,moveItemObj.curItemPos.y);

        //클라이언트에서 해당 아이템을 배치 가능한지 체크해서 성공할 경우에만 패킷을 전달하기에 따로 성공 여부 체크는 필요 없을듯.
    }

    internal static void S_DeleteItemHandler(PacketSession session, IMessage message)
    {
        S_DeleteItem packet = message as S_DeleteItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_DeleteItem");
        //패킷의 ItmeId를 통해 해당 아이템을 검색

        ItemObject deleteItem;
        bool searchId = Managers.Object._itemDic.TryGetValue(packet.ItemId, out deleteItem);

        if (searchId == false) {
            Debug.Log("삭제하려는 아이템이 존재하지 않음(검색 실패)");
            return;
        }
        //item.curGrid를 통해 해당 아이템이 존재하는 그리드를 도출해냄
        InventoryGrid deleteItemGrid = deleteItem.curItemGrid;
        //item.curGrid.CleanItemSlot(item)으로 해당 그리드의 아이템 슬롯에서 해당 아이템 제거
        deleteItemGrid.CleanItemSlot(deleteItem);

        Managers.Object.RemoveItem(packet.ItemId);

    }


    /* internal static void S_SkillHandler(PacketSession session, IMessage message)
     {
         var skillPacket = message as S_Skill;
         var go = Managers.Object.FindById(skillPacket.ObjectId);
         if (go == null)
             return;

         var cc = go.GetComponent<CreatureController>();
         if (cc == null)
             return;
         Managers.Skill.UseSkill(cc, skillPacket); //이팩트 생성

         Debug.Log("S_SkillHandler");
     }*/

    /*internal static void S_StatChangeHandler(PacketSession session, IMessage message)
    {
        var statpacket = (S_StatChange)message;
        var go = Managers.Object.FindById(statpacket.ObjectId);
        if (go == null)
            return;

        var cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;
        
        Debug.Log($"previous : S_Stat {statpacket.ObjectId}{cc.Stat}");

        cc.Stat.MergeFrom(statpacket.StatInfo);

        #region IsPlayer
        MyPlayerController mc = cc as MyPlayerController;
        if (mc != null)
        {
            mc.CheakUpdateLevel();
        }
        #endregion
        
        
        
        
        Debug.Log($"Next : S_Stat {statpacket.StatInfo}");
    }*/

    /*internal static void S_LobbyPlayerInfoHandler(PacketSession session, IMessage message)
    {
        //서버에서 로비에관한 정보
        Debug.Log("S_LobbyPlayerInfoHandler");
        var lobbyPlayerInfo = (S_LobbyPlayerInfo)message;
        GameObject.Find("LobbyScene").GetComponent<LobbyScene>().DataUpdate(lobbyPlayerInfo);
    }*/
}