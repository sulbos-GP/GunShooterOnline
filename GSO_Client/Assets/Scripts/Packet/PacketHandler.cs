using Google.Protobuf;
using Google.Protobuf.Protocol;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using ServerCore;
using System;
using System.Collections.Generic;

using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
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

        //enterGamePacket.ItemInfos //총알을 반영하기 위함. 실제로 아이템을 생성해내지는 않음

        //enterGamePacket.GearInfos //장착을 통해 장비 반영. 실제로 아이템을 생성하지 않고 장비변경만
        foreach(PS_GearInfo gear in enterGamePacket.GearInfos)
        {
            EquipSlot targetSlot = InventoryController.equipSlotDic[(int)gear.Part];

            ItemData data = new ItemData();
            data.SetItemData(gear.Item);

            targetSlot.ApplyItemEffects(data);
        }
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
        // 1번째 : 적 Spawn 정보 2번째 : box 정보 3번째 : 로컬 플레이어 정보?
        var spawnPacket = (S_Spawn)packet;
        
        foreach (var info in spawnPacket.Objects)
        {
            Managers.Object.Add(info, false);

            var type = (info.ObjectId >> 24) & 0x7f;
            if ((GameObjectType)type != GameObjectType.Player)
                continue;

            //체력 STAT 주입
            var Stats = info.StatInfo;
            var player = Managers.Object.FindById(info.ObjectId).GetComponent<PlayerController>();
            player.Hp = Stats.Hp;
            player.MaxHp = Stats.MaxHp;

            //Collider Line 작성
            player.SetDrawLine(info.Shape.Width,info.Shape.Height);


            //Spawn Player
            Vector2 vec2 = new Vector2(info.PositionInfo.PosX, info.PositionInfo.PosY);
            player.SpawnPlayer(vec2);
            Debug.Log("spawnID : " + info.ObjectId);
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

        //타 플레이어의 움직임을 조정

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
        {
            go.GetComponent<CreatureController>().Hp = changeHpPacket.Hp;
            go.GetComponent<PlayerController>().Hit();
        }
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

        //먼저 켜야함. 그래야 장비창의 awake로 장비칸의 id가 적용됨 -> 컨트롤러에서 Init으로 설정하도록 변경 옮겨도 됨
        if (!InventoryController.invenInstance.isActive) //인벤토리가 꺼져있으면 킴
        {
            InventoryController.invenInstance.invenUIControl();
        }

        //패킷으로 받은 아이템데이터 리스트를 클라이언트 형식으로 변경
        List<ItemData> packetItemList = new List<ItemData>();
        foreach (PS_ItemInfo packetItem in packet.ItemInfos)
        {
            ItemData convertItem = new ItemData();
            convertItem.SetItemData(packetItem);
            packetItemList.Add(convertItem);
        }

        //인벤토리 불러오기
        if(packet.SourceObjectId == 0)
        {
            //장착칸 불러오기 및 수정
            foreach (PS_GearInfo packetItem in packet.GearInfos)
            {
                ItemData convertItem = new ItemData();
                convertItem.SetItemData(packetItem.Item);

                EquipSlot targetSlot = InventoryController.equipSlotDic[(int)packetItem.Part];

                ItemObject newItem = ItemObject.CreateNewItem(convertItem, targetSlot.transform);

                if (targetSlot.equippedItem == null)
                {
                    targetSlot.EquipItem(newItem);
                }
                else if(targetSlot.equippedItem != newItem)
                {
                    //아이템 교체 -> todo 서버와 함께 구현해야함
                    targetSlot.UnEquipItem();
                    targetSlot.EquipItem(newItem);
                }
                
            }

            //플레이어의 인벤토리
            PlayerInventoryUI playerInvenUI = InventoryController.invenInstance.playerInvenUI;
            playerInvenUI.InventorySet(); //그리드 생성됨

            GridObject playerGrid = playerInvenUI.instantGrid;
            playerGrid.objectId = packet.SourceObjectId;
            playerGrid.PlaceItemInGrid(packetItemList);
            InventoryController.UpdatePlayerWeight();
            playerGrid.PrintInvenContents();

            
        }
        else
        {
            GameObject target = Managers.Object.FindById(packet.SourceObjectId);
            Box box = target.GetComponent<Box>();
            box.interactable = false;

            OtherInventoryUI otherInvenUI = InventoryController.invenInstance.otherInvenUI;
            otherInvenUI.InventorySet();
            otherInvenUI.instantGrid.InitializeGrid(box.size, box.weight);
            GridObject boxGrid = otherInvenUI.instantGrid;

            boxGrid.objectId = packet.SourceObjectId;
            //boxGrid.UpdateGridWeight(); //필요한가? 박스에는 무게가 의미 없음
            boxGrid.PlaceItemInGrid(packetItemList);
            boxGrid.PrintInvenContents();
        }
        InventoryController.invenInstance.DebugDic();
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
            if(target == null)
            {
                Debug.Log("타겟박스의 검색 실패");
                return;
            }
            target.GetComponent<Box>().interactable = true;
        }

        if (InventoryController.invenInstance.isActive)//인벤토리가 켜져 있으면 끔
        {
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

        Debug.Log($"패킷의 아이템id = {packet.SourceItem.ObjectId}");

        if (!packet.IsSuccess) //실패시 아이템을 다시 숨김상태로 전환
        {
            ItemObject targetItem = null;
            InventoryController.instantItemDic.TryGetValue(packet.SourceItem.ObjectId, out targetItem);
            if (!targetItem.isHide || targetItem.searchingCoroutine != null)
            {
                targetItem.HideItem();
            }
        }
    }

    /// <summary>
    /// moveItem에선 이동전 아이템과 이동후 아이템을 따로 주는데 이동전 아이템의 아이디와 이동후 아이템의 아이디가 다룰수 있어 이동후 아이디로 교체
    /// </summary>
    private static void ChangeItemObjectId(ItemObject targetItem, int newId)
    {
        InventoryController.instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.itemData.objectId = newId;
        if (!InventoryController.instantItemDic.ContainsKey(targetItem.itemData.objectId))
        {
            InventoryController.instantItemDic.Add(targetItem.itemData.objectId, targetItem);
        }
        
    }

    private static void AssignEquipOrGrid(int objectId, ref EquipSlot equipSlot, ref GridObject gridObject)
    {

        if (objectId > 0 && objectId <= 7)
        {
            equipSlot = InventoryController.equipSlotDic[objectId];
        }
        else
        {
            gridObject = GetGridObject(objectId);
        }
    }

    private static GridObject GetGridObject(int objectId)
    {

        return objectId == 0 ? InventoryController.invenInstance.playerInvenUI.instantGrid : InventoryController.invenInstance.otherInvenUI.instantGrid;
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

        InventoryController invenInstance = InventoryController.invenInstance;

        Debug.Log("S_MoveItem");
        
        ItemObject targetItem = null;
        InventoryController.instantItemDic.TryGetValue(packet.SourceMoveItem.ObjectId, out targetItem);
        if (targetItem == null)
        {
            Debug.Log($"해당 아이템 검색 실패 {packet.SourceMoveItem.ObjectId}");
            return;
        }

        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlot sourceEquip = null;
        EquipSlot destinationEquip = null;

        AssignEquipOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        AssignEquipOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);

        //Debug.Log($"패킷의 아이템id\n옮기기전 : {packet.SourceMoveItem.ObjectId}\n옮긴후 : {packet.DestinationMoveItem.ObjectId}");

        if (packet.IsSuccess)
        {
            if (packet.DestinationObjectId > 0 && packet.DestinationObjectId <= 7)
            {
                //도착지점이 장착칸 -> 해당 아이템을 장착칸에 장착
                if (!destinationEquip.EquipItem(targetItem)) //장착 실패시 원위치로
                {
                    invenInstance.UndoSlot(targetItem);
                    invenInstance.UndoItem(targetItem);
                }
            }
            else
            {
                //도착지점이 인벤칸 -> 해당 아이템을 인벤토리에 배치
                destinationGrid.PlaceItem(targetItem, packet.DestinationMoveItem.X, packet.DestinationMoveItem.Y);
                targetItem.Rotate(packet.DestinationMoveItem.Rotate); //주어진 회전도로 회전
            }

            invenInstance.BackUpSlot(targetItem);
            invenInstance.BackUpItem(targetItem);
        }
        else
        {
            invenInstance.UndoSlot(targetItem);
            invenInstance.UndoItem(targetItem);
        }

        ChangeItemObjectId(targetItem, packet.DestinationMoveItem.ObjectId);
        InventoryController.UpdatePlayerWeight();

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
        Debug.Log($"패킷의 아이템id = {packet.DeleteItem.ObjectId}");

        InventoryController invenInstance = InventoryController.invenInstance;

        ItemObject targetItem = null;
        InventoryController.instantItemDic.TryGetValue(packet.DeleteItem.ObjectId, out targetItem);
        if (targetItem == null)
        {
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            return;
        }

        GridObject sourceGrid = null;
        EquipSlot sourceEquip = null;
        AssignEquipOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);


        if (packet.IsSuccess) {
            Debug.Log("S_DeleteItem 성공");
            invenInstance.DestroyItem(targetItem);
        }
        else
        {
            Debug.Log("S_DeleteItem 실패");
            invenInstance.UndoSlot(targetItem);
            invenInstance.UndoItem(targetItem);
        }
        
        InventoryController.UpdatePlayerWeight();
    }

    internal static void S_MergeItemHandler(PacketSession session, IMessage message)
    {
        S_MergeItem packet = message as S_MergeItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }

        Debug.Log($"합쳐지는 아이템 아이디 = {packet.MergedItem.ObjectId}, 합치기 위한 아이디 = {packet.CombinedItem.ObjectId}");
        InventoryController invenInstance = InventoryController.invenInstance;

        ItemObject mergedItem = null;
        InventoryController.instantItemDic.TryGetValue(packet.MergedItem.ObjectId, out mergedItem);
        ItemObject combinedItem = null;
        InventoryController.instantItemDic.TryGetValue(packet.CombinedItem.ObjectId, out combinedItem);

        if (mergedItem == null || combinedItem == null)
        {
            invenInstance.DebugDic();
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            Debug.Log($"packet.mergeItem = {packet.MergedItem.ObjectId}");
            Debug.Log($"packet.combinedItem = {packet.CombinedItem.ObjectId}");
            return;
        }

        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlot sourceEquip = null;
        EquipSlot destinationEquip = null;

        AssignEquipOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        AssignEquipOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);


        if (packet.IsSuccess)
        {
            //각각 변화한 아이템 양 적용
            mergedItem.ItemAmount = packet.MergedItem.Amount;
            combinedItem.ItemAmount = packet.CombinedItem.Amount;

            if (sourceEquip != null)
            {
                sourceEquip.GetComponent<RecoverySlot>().UpdateQuickSlotAmount(mergedItem.ItemAmount);
            }

            if (destinationEquip != null)
            {
                destinationEquip.GetComponent<RecoverySlot>().UpdateQuickSlotAmount(combinedItem.ItemAmount);
            }

            //옮긴 아이템의 양이 0개가 되면 파괴 아니면 원래 위치로 이동
            if (packet.CombinedItem.Amount == 0)
            {
                invenInstance.DestroyItem(combinedItem);
            }
            else
            {
                invenInstance.UndoSlot(combinedItem);
                invenInstance.UndoItem(combinedItem);
            }
        }
        else
        {
            Debug.Log("S_MergeItem 실패");
            InventoryController.invenInstance.UndoSlot(combinedItem);
            InventoryController.invenInstance.UndoItem(combinedItem);
        }

        InventoryController.UpdatePlayerWeight();
    }


    internal static void S_DevideItemHandler(PacketSession session, IMessage message)
    {
        S_DevideItem packet = message as S_DevideItem;
        if (packet == null)
        {
            Debug.Log("패킷이 없음");
            return;
        }
        
        InventoryController invenInstance = InventoryController.invenInstance;

        ItemObject sourceItem = null; //원래 있던 아이템
        InventoryController.instantItemDic.TryGetValue(packet.SourceItem.ObjectId, out sourceItem);

        if (sourceItem == null )
        {
            Debug.Log("해당 아이디의 아이템 오브젝트가 존재하지 않음");
            return;
        }
        Debug.Log("S_Divide");
        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlot sourceEquip = null;
        EquipSlot destinationEquip = null;

        AssignEquipOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        AssignEquipOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);

        if (packet.IsSuccess)
        {
            Debug.Log($"원래 아이디 = {packet.SourceItem.ObjectId}, 새로 생성된 아이디 = {packet.DestinationItem.ObjectId}");
            invenInstance.UndoSlot(sourceItem); //원래 아이템은 원위치로 이동후 나눈 만큼 아이템 감소
            invenInstance.UndoItem(sourceItem);
            sourceItem.ItemAmount = packet.SourceItem.Amount;

            if (packet.DestinationObjectId > 0 && packet.DestinationObjectId <= 7)
            {
                //도착지점이 장착칸 -> 이 경우는 소모품의 경우 
                ItemData itemData = new ItemData();
                itemData.SetItemData(packet.DestinationItem);

                ItemObject newItem = ItemObject.CreateNewItem(itemData, destinationEquip.transform);
                    
                if (!destinationEquip.EquipItem(newItem))
                {
                    invenInstance.UndoSlot(newItem);
                    invenInstance.UndoItem(newItem);
                }
            }
            else
            {
                //도착지점이 인벤칸 -> 새로운 아이템을 생성하여 나눈 아이템 생성
                ItemData newData = new ItemData();
                newData.SetItemData(packet.DestinationItem);

                destinationGrid.CreateItemObjAndPlace(newData);
            }

            invenInstance.BackUpSlot(sourceItem);
            invenInstance.BackUpItem(sourceItem);
        }
        else
        {
            //실패할경우 로직
            Debug.Log("S_Divide 실패");
            invenInstance.UndoSlot(sourceItem);
            invenInstance.UndoItem(sourceItem);
        }

        ChangeItemObjectId(sourceItem, packet.SourceItem.ObjectId);
        InventoryController.UpdatePlayerWeight();
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
            //플레이어를 찾을 수 없는 곳.
            //Vector2 hitObj = new Vector2(packet.HitPointX,packet.HitPointY);
            
            //Debug.DrawLine(hitObj, UnitManager.Instance.CurrentPlayer.transform.position);


            //Debug.DrawLine(hitObj, hitObj);
            return;
        }
        Vector2 hitPoint = new Vector2(packet.HitPointX, packet.HitPointY);
        Vector2 startPoint = new Vector2(packet.StartPosX, packet.StartPosY);
        Managers.Object.MyPlayer.gun.bulletLine.SetPosition(0, startPoint);
        Managers.Object.MyPlayer.gun.bulletLine.SetPosition(1, hitPoint);
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
            //플레이어는 자신의 장착칸과 인벤토리의 내용을 서버에 전송?
            return;
        }

        var player = Managers.Object.FindById(packet.PlayerId);

        Managers.Resource.Destroy(player);
        Managers.Object.Remove(packet.PlayerId);
        Managers.Object.DebugDics();
    }

    internal static void S_JoinServerHandler(PacketSession session, IMessage message)
    {
        S_JoinServer packet = message as S_JoinServer;

        if (!packet.Connected)
            return;

        //접속 완료

        Debug.Log("서버에 접속 완료");
    }

    internal static void S_WaitingStatusHandler(PacketSession session, IMessage message)
    {
        S_WaitingStatus packet = message as S_WaitingStatus;

        if (packet == null)
            return;

        //참가시 인원 증감시 호출

        Debug.Log($"접속 인원 : {packet.CurrentPlayers} / {packet.RequiredPlayers}");
    }

    internal static void S_GameStartHandler(PacketSession session, IMessage message)
    {
        S_GameStart packet = message as S_GameStart;

        if (packet == null)
            return;



        //자신의 플레이어 외에 다른 플레이어와 오브젝트의 객체 생성

        foreach(ObjectInfo obj in packet.Objects)
        {
            Managers.Object.Add(obj);
        }

        //obj가 플레이어인 경우 장착칸 1번 확인해서 

        //Managers.Object.Add(enterGamePacket.Player, true);

        ////Use Stat
        //var Stats = enterGamePacket.Player.StatInfo;
        //Managers.Object.MyPlayer.Hp = Stats.Hp;
        //Managers.Object.MyPlayer.MaxHp = Stats.MaxHp;

        ////enterGamePacket.ItemInfos //총알을 반영하기 위함. 실제로 아이템을 생성해내지는 않음

        ////enterGamePacket.GearInfos //장착을 통해 장비 반영. 실제로 아이템을 생성하지 않고 장비변경만
        //foreach (PS_GearInfo gear in enterGamePacket.GearInfos)
        //{
        //    EquipSlot targetSlot = InventoryController.equipSlotDic[(int)gear.Part];

        //    ItemData data = new ItemData();
        //    data.SetItemData(gear.Item);

        //    targetSlot.ApplyItemEffects(data);
        //}
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
