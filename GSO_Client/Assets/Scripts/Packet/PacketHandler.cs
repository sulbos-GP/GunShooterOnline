using Google.Protobuf;
using Google.Protobuf.Protocol;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using ServerCore;
using System;
using System.Collections.Generic;
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
        
        foreach (var info in spawnPacket.Objects)
        {
            Managers.Object.Add(info, false);

            var type = (info.ObjectId >> 24) & 0x7f;
            if ((GameObjectType)type != GameObjectType.Player)
                continue;
            var Stats = info.StatInfo;
            var player = Managers.Object.FindById(info.ObjectId).GetComponent<PlayerController>();
            player.Hp = Stats.Hp;
            player.MaxHp = Stats.MaxHp;
        }
        //Debug.Log("S_SpawnHandler");*/
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
        var movePacket = packet as S_Move;

        var go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        {
            return;
        }

        var cc = go.GetComponent<BaseController>();
        if (cc == null)
            return;

        var ec = cc.GetComponent<PlayerController>();
        ec.UpdatePosInfo(movePacket.PositionInfo);
        ec.UpdateMoving();
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
        /*
         * 플레이어가 인벤토리를 여는 키를 눌렀을대 전송되며 플레이어의 아이템데이터를 서버에 요청한다
         * sourceObjectId가 0이면 플레이어, id가 있으면 박스의 id
         * 각 타겟의 UI의 ItemList에 패킷으로 전달된 아이템들을 넣어줌
         * 
         * 인벤토리를 열고 InventorySet 함수를 실행시키면 각 인벤토리가 생성된다
         * 만약 박스가 열렸다면 박스의 bool변수를 변경하여 다른 플레이어가 루팅중인 박스는 접근이 불가하다
         */

        /*
        //해당 클라이언트만 해당 되며 
        S_LoadInventory packet = message as S_LoadInventory;
        Debug.Log("S_LoadInventory");

        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }

        //패킷의 인벤데이터를 클라의 인벤데이터로 변환
        InvenData newInvenData = new InvenData();
        newInvenData.SetInvenData(packet.InvenData);
        foreach (GridData grid in newInvenData.grid)
        {
            if (Managers.Object._gridDic.ContainsKey(grid.gridId) == true) { continue; }
            Managers.Object.AddGridDic(grid.gridId, grid);
            foreach (ItemData item in grid.itemList)
            {
                if (Managers.Object._itemDic.ContainsKey(item.objectId) == true) { continue; }
                Managers.Object.AddItemDic(item.objectId, item);
            }
        }

        GameObject invenObj = Managers.Object.FindById(packet.InventoryId); //패킷의 해당 인벤토리의 id로 인벤토리 오브젝트 검색
        if (Managers.Object.MyPlayer.Id == packet.InventoryId)
        {
            //플레이어 인벤토리에 패킷의 invenData 적용
            if (invenObj.GetComponent<PlayerInventory>() == null)
            {
                Debug.Log("적용할 오브젝트에 해당 스크립트가 없음(Player)");
                return;
            }

            invenObj.GetComponent<PlayerInventory>().InputInvenData = newInvenData;
        }
        else
        {
            //인벤토리 오브젝트 목록 : 박스, 다른 플레이어 객체
            //아더 인벤토리에 패킷의 invenData 적용
            if (invenObj.GetComponent<OtherInventory>() == null)
            {
                Debug.Log("적용할 오브젝트에 해당 스크립트가 없음(Other)");
                return;
            }

            invenObj.GetComponent<OtherInventory>().InputInvenData = newInvenData;
        }*/
    }

    internal static void S_CloseInventoryHandler(PacketSession session, IMessage message)
    {
        /*
         * 인벤토리를 닫을경우 패킷 -> 박스에 2명이 접근하는걸 막기 위함
         * isSuccess로 성공유무 판단
         * sourceObjectId로 0이 아니라면 박스이니 박스의 bool변수를 변경
         */
    }

    internal static void S_MoveItemHandler(PacketSession session, IMessage message)
    {
        /*
         * isSuccess가 true면 출발지 아이템을 검색하여 삭제 및 목적지 아이템을 새로 생성하여 배치
         */

        /* 서버에서 success를 true로 주면 해당 아이템의 배치를 완료함 (아이템의 오브젝트 위치 설정 및 해당 그리드의 배열에 아이템 추가)
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
            return;
        }

        
        ItemData moveItemData = null;
        Managers.Object._itemDic.TryGetValue(packet.ItemData.ItemId, out moveItemData);
        if (moveItemData == null)
        {
            //클라에 해당 아이템이 없는 경우(적플레이어가 자신의 인벤에 있던 아이템을 박스에 넣을 경우) -> 새 아이템을 생성
            moveItemData = new ItemData();
            moveItemData.SetItemData(packet.ItemData);
        }

        moveItemData.pos = new Vector2Int(packet.ItemData.ItemPosX, packet.ItemData.ItemPosY);
        moveItemData.rotate = packet.ItemData.ItemRotate;

        GridData movedGrid;
        Managers.Object._gridDic.TryGetValue(packet.GridId, out movedGrid);
        if (movedGrid != null)
        {
            //현재 그리드를 찾은 상태에서 
            foreach (ItemData itemData in movedGrid.itemList)
            {
                //아이템의 위치가 같다면 머지(혹시모르니 아이템 코드가 같다는 조건도 넣음)
                //이 핸들러가 도착했다는것은 아이템의 배치가 성공했음을 의미
                //같은 그리드상에서 아이템을 옮겼을때 의 경우 이미 그리드의 아이템 리스트에는 해당 아이템이 있어 중복되는 현상 수정(itemId가 달라야하는 조건 추가)
                if ((itemData.objectId != moveItemData.objectId) && (itemData.pos == moveItemData.pos) && (itemData.itemId == moveItemData.itemId))
                {
                    itemData.amount += moveItemData.amount;
                    return;
                }
            }
            movedGrid.itemList.Add(moveItemData);
        }

        //todo. 만약 플레이어 1,2가 같은 박스의 인벤토리를 보고있을때 1이 아이템을 옮기면 2의 인벤토리UI 에서도 해당 아이템 오브젝트를 옮김-> 딜리트에도 같음 
        //제대로 작동안함
        for (int i = 0; i < InventoryController.invenInstance.instantItemList.Count; i++)
        {
            if (InventoryController.invenInstance.instantItemList[i].itemData.objectId == moveItemData.objectId)
            {
                //이미 해당 오브젝트의 아이템 데이터가 업데이트 되어 자신의 데이터를 사용하여 위치와 회전만 조정하면 됨
                ItemObject itemObj = InventoryController.invenInstance.instantItemList[i];
                if(itemObj.curItemGrid.gridData.gridId == movedGrid.gridId) //TODO -> 인벤토리 단위로 바꿔야함(나중에 인벤토리 내에 여러개의 그리드가 존재할경우)
                {
                    itemObj.curItemGrid.UpdateItemPosition(itemObj, itemObj.itemData.pos.x, itemObj.itemData.pos.y, itemObj.GetComponent<RectTransform>());
                    itemObj.Rotate(itemObj.itemData.rotate);
                }
                else
                {
                    itemObj.DestroyItem();
                }

                break;
            }
        }

        GridData pastGrid;
        Managers.Object._gridDic.TryGetValue(packet.LastGridId, out pastGrid);
        if (pastGrid == null)
        {
            Debug.Log("옮기기전 그리드가 존재하지 않음(무브연산)");
            return;
        }

        pastGrid.itemList.Remove(moveItemData);*/
    }

    //아이템을 삭제할때. 플레이어가 아이템을 들었을때 다른 플레이어에게 전송할때도 좋을듯.
    internal static void S_DeleteItemHandler(PacketSession session, IMessage message)
    {
        /* 이것도 success에 여부에 따라 해당 아이템을 삭제
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

        ItemData deleteItemdata = null;
        Managers.Object._itemDic.TryGetValue(packet.ItemData.ItemId, out deleteItemdata);
        if (deleteItemdata == null)
        {
            //클라에 해당 아이템이 없는 경우(적플레이어가 자신의 인벤에 있던 아이템을 버릴경우)
            Debug.Log("클라에 해당 아이템 데이터가 없음(삭제 연산)");
            return;
        }

        GridData deleteItemGrid;
        Managers.Object._gridDic.TryGetValue(packet.GridId, out deleteItemGrid);
        if (deleteItemGrid == null)
        {
            Debug.Log("클라이언트에 아이템이 위치했던 그리드가 존재하지 않음(삭제연산)");
            return;
        }
        deleteItemGrid.itemList.Remove(deleteItemdata);


        //해당 데이터로 만들어진 아이템 오브젝트가 존재할경우 삭제
        //제대로 작동안함
        /*
        for (int i = 0; i < InventoryController.invenInstance.instantItemList.Count; i++)
        {
            if(InventoryController.invenInstance.instantItemList[i].itemData.objectId == deleteItemdata.objectId)
            {
                Managers.Object.RemoveItemDic(InventoryController.invenInstance.instantItemList[i].itemData.objectId);
                InventoryController.invenInstance.instantItemList[i].DestroyItem();
                break;
            }
        }
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
        //일단 지움 : Managers.Object._rayDic.Add(packet.RayId, packet);

        GameObject go = Managers.Object.FindById(packet.HitObjectId);
        if (go == null)
        {
            Debug.Log("Wall Hit bullet");
            return;
        }
        //Debug.Log(go.GetComponent<MyPlayerController>().Id);
        var cc = go.GetComponent<BaseController>();
        if (cc == null)
        {
            //TO - DO : 맞는지 모르겠음.
            Vector2 hitObj = new Vector2(packet.HitPointX,packet.HitPointY);

            //Debug.DrawLine(hitObj, UnitManager.Instance.CurrentPlayer.transform.position);


            //Debug.DrawLine(hitObj, hitObj);
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

        /* 다른 플레이어는 클라에서 인벤토리를 가지지 않음
        InvenData targetInvenData = player.GetComponent<PlayerInventory>().InputInvenData;
        if (targetInvenData == null) {
            Debug.Log("인벤데이터를 찾지 못함");
        }

        //해당 플레이어의 인벤토리의 그리드와 아이템을 오브젝트 매니저의 딕셔너리에서 제거
        foreach (GridData grid in targetInvenData.grid) {
            Managers.Object.RemoveGridDic(grid.gridId);
            foreach (ItemData item in grid.itemList)
            {
                Managers.Object.RemoveItemDic(item.objectId);
            }
        }*/

        Managers.Resource.Destroy(player);
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
