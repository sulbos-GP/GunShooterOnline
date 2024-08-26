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
        
        
        //해당 클라이언트만 해당 되며 
        S_LoadInventory packet = message as S_LoadInventory;
        if (packet == null)
        {
            Debug.Log("S_LoadInventory 패킷이 없음");
            return;
        }

        if (!packet.IsSuccess)
        {
            Debug.Log("S_LoadInventory 불러오기에 실패함");
            if (InventoryController.invenInstance.isActive)
            {
                InventoryController.invenInstance.invenUIControl();
            }
            return;
        }

        //패킷으로 받은 아이템데이터 리스트를 클라이언트 형식으로 변경
        List<ItemData> packetItemList = new List<ItemData>();
        foreach (PS_ItemInfo packetItem in packet.ItemInfos)
        {
            ItemData convertItem = new ItemData();
            convertItem.SetItemData(packetItem);
            packetItemList.Add(convertItem);
        }

        Debug.Log($"{packet.SourceObjectId}의 아이템 설정");

        if(packet.SourceObjectId == 0)
        {
            //플레이어의 인벤토리
            PlayerInventoryUI playerInvenUI = InventoryController.invenInstance.playerInvenUI;
            playerInvenUI.InventorySet(); //그리드 생성됨

            GridObject playerGrid = playerInvenUI.instantGrid;
            playerGrid.objectId = packet.SourceObjectId;
            playerGrid.PlaceItemInGrid(packetItemList);
            playerInvenUI.WeightTextSet(playerGrid.GridWeight, playerGrid.limitWeight);
            playerGrid.PrintInvenContents();
        }
        else
        {
            GameObject target = Managers.Object.FindById(packet.SourceObjectId);
            target.GetComponent<Box>().interactable = false;

            OtherInventoryUI otherInvenUI = InventoryController.invenInstance.otherInvenUI;
            otherInvenUI.InventorySet();
            GridObject boxGrid = otherInvenUI.instantGrid;

            boxGrid.objectId = packet.SourceObjectId;
            boxGrid.PlaceItemInGrid(packetItemList);
            boxGrid.PrintInvenContents();
        }

        if (!InventoryController.invenInstance.isActive)
        {
            InventoryController.invenInstance.invenUIControl();
        }
    }

    internal static void S_CloseInventoryHandler(PacketSession session, IMessage message)
    {
        /*
         * 인벤토리를 닫을경우 패킷 -> 박스에 2명이 접근하는걸 막기 위함
         * isSuccess로 성공유무 판단
         * sourceObjectId로 0이 아니라면 박스이니 박스의 bool변수를 변경
         */
        
        S_CloseInventory packet = message as S_CloseInventory;
        if (packet == null)
        {
            Debug.Log("S_CloseInventory 패킷이 없음");
            return;
        }
        Debug.Log("패킷받음");
        if (!packet.IsSuccess)
        {
            Debug.Log("서버에서 거부함");
            return;
        }

        if(packet.SourceObjectId != 0)
        {
            GameObject target = Managers.Object.FindById(packet.SourceObjectId);
            target.GetComponent<Box>().interactable = true;
        }

        

        if (InventoryController.invenInstance.isActive)
        {
            InventoryController.invenInstance.instantItemDic.Clear();
            InventoryController.invenInstance.invenUIControl();
        }
    }

    internal static void S_SearchItemHandler(PacketSession session, IMessage message)
    {
        /*
         * 아이템을 검색 시작할때 보낸패킷의 답장
         * isSuccess로 성공유무 판단
         * 실패하면 검색 코루틴을 중지하거나 isHide가 풀려있으면 다시 잠금
         */

        S_SearchItem packet = message as S_SearchItem;
        if (packet == null)
        {
            Debug.Log("S_SearchInventory 패킷이 없음");
            return;
        }

        Debug.Log("패킷받음");
        Debug.Log($"패킷의 아이템id = {packet.SourceItem.ObjectId}");
        if (!packet.IsSuccess)
        {
            ItemObject targetItem = null;
            InventoryController.invenInstance.instantItemDic.TryGetValue(packet.SourceItem.ObjectId, out targetItem);
            if (!targetItem.isHide || targetItem.searchingCoroutine != null)
            {
                targetItem.HideItem();
            }
        }
    }


    private static void ChangeItemObjectId(ItemObject targetItem, int newId)
    {
        InventoryController.invenInstance.instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.itemData.objectId = newId;
        InventoryController.invenInstance.instantItemDic.Add(targetItem.itemData.objectId, targetItem);
    }

    internal static void S_MoveItemHandler(PacketSession session, IMessage message)
    {
        /*
         * isSuccess가 true면 출발지 아이템을 검색하여 삭제 및 목적지 아이템을 새로 생성하여 배치
         * false면 출발지 아이템을 원래 위치로 Undo
         */
        S_MoveItem packet = message as S_MoveItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_MoveItem");
        
        ItemObject targetItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.SourceMoveItem.ObjectId, out targetItem);
        if (targetItem == null)
        {
            Debug.Log("해당 아이템 검색 실패");
            return;
        }

        if (packet.IsSuccess)
        {
            Debug.Log("S_MoveItem 성공");
            Debug.Log($"패킷의 아이템id\n옮기기전 : {packet.SourceMoveItem.ObjectId}\n옮긴후 : {packet.DestinationMoveItem.ObjectId}");
            GridObject sourceGrid;
            if (packet.SourceObjectId == 0) {
                sourceGrid = InventoryController.invenInstance.playerInvenUI.instantGrid;
            }
            else
            {
                sourceGrid = InventoryController.invenInstance.otherInvenUI.instantGrid;
            }

            GridObject destinationGrid;
            if (packet.DestinationObjectId == 0)
            {
                destinationGrid = InventoryController.invenInstance.playerInvenUI.instantGrid;
            }
            else
            {
                destinationGrid = InventoryController.invenInstance.otherInvenUI.instantGrid;
            }

            //sourceGrid.CleanItemSlot(sourceItem); 이미 아이템을 집을때 슬롯에서 삭제되어 있음
            destinationGrid.PlaceItem(targetItem, packet.DestinationMoveItem.X, packet.DestinationMoveItem.Y);
            targetItem.Rotate(packet.DestinationMoveItem.Rotate);

            ChangeItemObjectId(targetItem, packet.DestinationMoveItem.ObjectId);

            targetItem.curItemGrid.PrintInvenContents();
            targetItem.backUpItemGrid.PrintInvenContents();
            InventoryController.invenInstance.BackUpGridSlot(targetItem);
            InventoryController.invenInstance.BackUpItem(targetItem);

            InventoryController.invenInstance.playerInvenUI.WeightTextSet(
                InventoryController.invenInstance.playerInvenUI.instantGrid.GridWeight, 
                InventoryController.invenInstance.playerInvenUI.instantGrid.limitWeight);
        }

        else
        {
            Debug.Log("S_MoveItem 실패");
            Debug.Log($"패킷의 아이템id\n옮기기전 : {packet.SourceMoveItem.ObjectId}\n옮긴후 : {packet.DestinationMoveItem.ObjectId}");
            targetItem.curItemGrid.PrintInvenContents();
            targetItem.backUpItemGrid.PrintInvenContents();

            ChangeItemObjectId(targetItem, packet.DestinationMoveItem.ObjectId);
            InventoryController.invenInstance.UndoGridSlot(targetItem);
            InventoryController.invenInstance.UndoItem(targetItem);

            InventoryController.invenInstance.playerInvenUI.WeightTextSet(
                InventoryController.invenInstance.playerInvenUI.instantGrid.GridWeight,
                InventoryController.invenInstance.playerInvenUI.instantGrid.limitWeight);

            targetItem.curItemGrid.PrintInvenContents();
            targetItem.backUpItemGrid.PrintInvenContents();
        }

    }

    //아이템을 삭제할때. 플레이어가 아이템을 들었을때 다른 플레이어에게 전송할때도 좋을듯.
    internal static void S_DeleteItemHandler(PacketSession session, IMessage message)
    {
        /*
         * isSuccess 가 성공하면 출발지 저장소 아이디를 통해 그리드의 내용을 삭제하고 delete아이템을 삭제함
         * false면 해당 아이템을 Undo하여 원래 위치로
         */
        S_DeleteItem packet = message as S_DeleteItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_DeleteItem");
        Debug.Log($"패킷의 아이템id = {packet.DeleteItem.ObjectId}");

        ItemObject targetItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.DeleteItem.ObjectId, out targetItem);
        if (targetItem == null)
        {
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            return;
        }

        if (packet.IsSuccess) {

            Debug.Log("S_DeleteItem 성공");
            GridObject sourceGrid;
            if (packet.SourceObjectId == 0)
            {
                sourceGrid = InventoryController.invenInstance.playerInvenUI.instantGrid;
            }
            else
            {
                sourceGrid = InventoryController.invenInstance.otherInvenUI.instantGrid;
            }

            sourceGrid.CleanItemSlot(targetItem);
            InventoryController.invenInstance.DestroyItem(targetItem);

            InventoryController.invenInstance.playerInvenUI.WeightTextSet(
                InventoryController.invenInstance.playerInvenUI.instantGrid.GridWeight,
                InventoryController.invenInstance.playerInvenUI.instantGrid.limitWeight);
        }
        else
        {
            Debug.Log("S_DeleteItem 실패");
            InventoryController.invenInstance.UndoGridSlot(targetItem);
            InventoryController.invenInstance.UndoItem(targetItem);

            InventoryController.invenInstance.playerInvenUI.WeightTextSet(
                InventoryController.invenInstance.playerInvenUI.instantGrid.GridWeight,
                InventoryController.invenInstance.playerInvenUI.instantGrid.limitWeight);
        }

    }

    internal static void S_MergeItemHandler(PacketSession session, IMessage message)
    {
        S_MergeItem packet = message as S_MergeItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_MergeItem");
        Debug.Log($"합쳐지는 아이템 아이디 = {packet.MergedItem.ObjectId}, 합치기 위한 아이디 = {packet.CombinedItem.ObjectId}");

        ItemObject mergedItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.MergedItem.ObjectId, out mergedItem);
        ItemObject combinedItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.CombinedItem.ObjectId, out combinedItem);

        if (mergedItem == null || combinedItem == null)
        {
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            return;
        }

        if (packet.IsSuccess)
        {
            //성공할경우 로직
            /*
             * 패킷의 combinedItem의 수량이 0이라면 combinedItem의 삭제하고 아니라면 combinedItem의 원래 위치로 돌아감
             * 
             * mergedItme 수량을 패킷의 수량으로 업데이트
             */


        }
        else
        {
            //실패할경우 로직
            Debug.Log("S_MergeItem 실패");
            InventoryController.invenInstance.UndoGridSlot(combinedItem);
            InventoryController.invenInstance.UndoItem(combinedItem);

            InventoryController.invenInstance.playerInvenUI.WeightTextSet(
                InventoryController.invenInstance.playerInvenUI.instantGrid.GridWeight,
                InventoryController.invenInstance.playerInvenUI.instantGrid.limitWeight);
            
        }
    }

    internal static void S_DevideItemHandler(PacketSession session, IMessage message)
    {
        S_DevideItem packet = message as S_DevideItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        Debug.Log("S_DevideItem");
        Debug.Log($"합쳐지는 아이템 아이디 = {packet.SourceItem.ObjectId}, 합치기 위한 아이디 = {packet.DestinationItem.ObjectId}");

        ItemObject sourceItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.SourceItem.ObjectId, out sourceItem);
        ItemObject destinationItem = null;
        InventoryController.invenInstance.instantItemDic.TryGetValue(packet.DestinationItem.ObjectId, out destinationItem);

        if (sourceItem == null || destinationItem == null)
        {
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            return;
        }

        if (packet.IsSuccess)
        {
            //성공할경우 로직
            /*
             * 
             */


        }
        else
        {
            //실패할경우 로직
           

        }
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
