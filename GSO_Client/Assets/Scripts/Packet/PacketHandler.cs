using Google.Protobuf;
using Google.Protobuf.Protocol;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
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
        
        //Use Stat
        var Stats = enterGamePacket.Player.StatInfo;
        Managers.Object.MyPlayer.Hp = Stats.Hp;
        Managers.Object.MyPlayer.MaxHp = Stats.MaxHp;
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

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        var spawnPacket = (S_Spawn)packet;
        
        foreach (var info in spawnPacket.Objects) Managers.Object.Add(info, false);
        //Debug.Log("S_SpawnHandler");
    }

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

 

    internal static void S_ConnectedHandler(PacketSession session, IMessage message)
    {
        S_Connected packet = message as S_Connected;
        Debug.Log("S_ConnectedHandler");

    }
    
    

    internal static void S_LoadInventoryHandler(PacketSession session, IMessage message)
    {
        S_LoadInventory packet = message as S_LoadInventory;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_LoadInventory");

        if(packet.InvenData == null)
        {
            Debug.Log("인벤데이터가 비어있음");
        }

        //인벤 데이터 생성 및 패킷의 InvenDataInfo를 InvenData로 변환
        InvenData newInvenData = new InvenData();
        newInvenData.SetInvenData(packet.InvenData);
        

        /*
        //생성된 인벤토리와 그리드, 아이템 데이터를 오브젝트 매니저의 딕셔너리들에 추가
        Managers.Object._inventoryDic.Add(packet.InventoryId, newInvenData);*/

        
        foreach(GridData grid in newInvenData.gridList)
        {
            if(Managers.Object._gridDic.ContainsKey(grid.gridId) == true) { continue; }
            Managers.Object.AddGridDic(grid.gridId, grid);
            foreach(ItemData item in grid.itemList)
            {
                if (Managers.Object._itemDic.ContainsKey(item.itemId) == true) { continue; }
                Managers.Object.AddItemDic(item.itemId, item);
            }
        }
        
        GameObject invenObj = Managers.Object.FindById(packet.InventoryId); //데이터를 적용할 대상 검색'

        Managers.Object.DebugDics();
        //플레이어의 인벤토리id와 패킷내의 인벤토리id 비교 -> 같으면 플레이어의 인벤토리에 반영 다르면 아더 인벤토리에 반영
        if (Managers.Object.MyPlayer.Id == packet.InventoryId)
        {
            //플레이어 인벤토리에 패킷의 invenData 적용
            if(invenObj.GetComponent<PlayerInventory>() == null)
            {
                Debug.Log("적용할 오브젝트에 해당 스크립트가 없음(Player)");
            }
            
            invenObj.GetComponent<PlayerInventory>().InputInvenData = newInvenData;
        }
        else
        {
            //아더 인벤토리에 패킷의 invenData 적용
            if (invenObj.GetComponent<OtherInventory>() == null)
            {
                Debug.Log("적용할 오브젝트에 해당 스크립트가 없음(Other)");
            }
            
            invenObj.GetComponent<OtherInventory>().InputInvenData = newInvenData;
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

        if (packet.PlayerId == Managers.Object.MyPlayer.Id)
        {
            //옮긴 플레이어를 제외한 다른 플레이어에게 적용되는 핸들러
            Debug.Log("myplayer");
            return;
        }


        ItemData moveItemData;
        bool success = Managers.Object._itemDic.TryGetValue(packet.ItemId, out moveItemData);
        if (success == false)
        {
            Debug.Log("옮기려는 아이템이 존재하지 않음(검색실패)");
            return;
        }

        moveItemData.itemPos = new Vector2Int(packet.ItemPosX, packet.ItemPosY);
        moveItemData.itemRotate = packet.ItemRotate;

        GridData curItemGrid;
        success = Managers.Object._gridDic.TryGetValue(packet.GridId, out curItemGrid);
        if (success == false)
        {
            Debug.Log("옮긴 후 그리드가 존재하지 않음(검색실패)");
            return;
        }
        curItemGrid.itemList.Add(moveItemData);

        GridData backUpItemGrid;
        success = Managers.Object._gridDic.TryGetValue(packet.LastGridId, out backUpItemGrid);
        if (success == false)
        {
            Debug.Log("옮기기전 그리드가 존재하지 않음(검색실패)");
            return;
        }
        backUpItemGrid.itemList.Remove(moveItemData);

        /*
        if (InventoryController.invenInstance.isActive)
        {
            //플레이어의 인벤UI가 켜져 있는 경우
            foreach (InventoryGrid grid in InventoryController.invenInstance.otherInvenUI.instantGridList) {
                if (grid.gridData.gridId == packet.GridId) { }
            }

        }
        */

        //프로토콜 업데이트 시 주석해제

        //클라이언트에서 해당 아이템을 배치 가능한지 체크해서 성공할 경우에만 패킷을 전달하기에 따로 성공 여부 체크는 필요 없을듯.

    }

    //아이템을 삭제할때. 플레이어가 아이템을 들었을때 다른 플레이어에게 전송할때도 좋을듯.
    internal static void S_DeleteItemHandler(PacketSession session, IMessage message)
    {
        S_DeleteItem packet = message as S_DeleteItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_DeleteItem");

        if (packet.PlayerId == Managers.Object.MyPlayer.Id)
        {
            //옮긴 플레이어를 제외한 다른 플레이어에게 전송됨
            return;
        }
        /*
        //패킷의 ItmeId를 통해 해당 아이템을 검색
        ItemObject deleteItem;
        bool success = Managers.Object._itemDic.TryGetValue(packet.ItemId, out deleteItem);

        if (success == false)
        {
            Debug.Log("삭제하려는 아이템이 존재하지 않음(검색 실패)");
            return;
        }
        //item.curGrid를 통해 해당 아이템이 존재하는 그리드를 도출해냄
        InventoryGrid deleteItemGrid = deleteItem.curItemGrid;
        if (deleteItemGrid == null)
        {
            Debug.Log("아이템이 위치한 그리드가 존재하지 않음");
            return;
        }

        //item.curGrid.CleanItemSlot(item)으로 해당 그리드의 아이템 슬롯에서 해당 아이템 제거
        deleteItemGrid.CleanItemSlot(deleteItem);

        //해당 오브젝트가 딕셔너리에 존재하면 해당 아이템을 삭제함
        Managers.Object.RemoveItem(packet.ItemId);
        */
    }

    internal static void S_RaycastHitHandler(PacketSession session, IMessage message)
    {
        S_RaycastHit packet = message as S_RaycastHit;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_RaycastHit");
        //레이의 아이디를 키로 해당 패킷을 저장하는 딕셔너리
        Managers.Object._rayDic.Add(packet.RayId, packet);

        GameObject go = Managers.Object.FindById(packet.HitObjectId);
        if (go == null)
        {
            Debug.Log("Wall Hit bullet");
            return;
        }
        Debug.Log(go.GetComponent<MyPlayerController>().Id);
        var cc = go.GetComponent<BaseController>();
        if (cc == null)
        {
            //TO - DO : 맞는지 모르겠음.
            Vector2 hitObj = new Vector2(packet.HitPointX,packet.HitPointY);
            Debug.DrawLine(hitObj, UnitManager.Instance.CurrentPlayer.transform.position);
            return;
        }
 

        //cc에서 피격 표시?

        //hit ID가 없으면 벽 맞는 거라         packet.HitPointX , Y이용하여 렌더링 및 이펙트 표시!! 


    }

    internal static void S_ExitGameHandler(PacketSession session, IMessage message)
    {
        S_ExitGame packet = message as S_ExitGame;

        if(Managers.Object.MyPlayer == null)
        {
            return;
        }
        //나간 플레이어는 이미 디스트로이 된 상태이며 그 외의 플레이어에게서 처리될 패킷
        if (packet.PlayerId == Managers.Object.MyPlayer.Id)
        {
            return;
        }
        //플레이어와 해당 플레이어 가진 아이템 그리드 데이터 삭제할것\
        var player = Managers.Object.FindById(packet.PlayerId);
        InvenData targetInvenData = player.GetComponent<PlayerInventory>().InputInvenData;
        if (targetInvenData == null) {
            Debug.Log("인벤데이터를 찾지 못함");
        }

        //해당 플레이어의 인벤토리의 그리드와 아이템을 오브젝트 매니저의 딕셔너리에서 제거
        foreach (GridData grid in targetInvenData.gridList) {
            Managers.Object.RemoveGridDic(grid.gridId);
            foreach (ItemData item in grid.itemList)
            {
                Managers.Object.RemoveItemDic(item.itemId);
            }
        }

        Managers.Object.Remove(packet.PlayerId);
        Managers.Object.DebugDics();
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