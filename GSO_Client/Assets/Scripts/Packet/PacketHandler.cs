using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using MathNet.Numerics.LinearAlgebra.Factorization;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.PlayerSettings;


internal class PacketHandler
{
    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ GAMESYSTEM PACKET START ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        Managers.SystemLog.Message("S_EnterGameHandler");
        var enterGamePacket = (S_EnterGame)packet;

        UIManager.Instance.leftTime = enterGamePacket.GameData.LeftTime* 60;

        Managers.SystemLog.Message($"{enterGamePacket.Player}");
        Managers.Object.Add(enterGamePacket.Player, true);

        var Stats = enterGamePacket.Player.StatInfo;
        Managers.Object.MyPlayer.Hp = Stats.Hp;
        Managers.Object.MyPlayer.MaxHp = Stats.MaxHp;
        UIManager.Instance.SetHpText();

        //장착데이터 딕셔너리에 저장 -> UI에 반영되지 않음
        foreach(PS_GearInfo gear in enterGamePacket.GearInfos)
        {
            ItemData data = new ItemData();
            data.SetItemData(gear.Item);
            InventoryController.Instance.SetEquipItem((int)gear.Part, data);
        }

        Managers.Object.MyPlayer._Quest = UIManager.Instance.QuestUI;
        Managers.Object.MyPlayer._Quest.InitQuest(enterGamePacket.Quests.ToList());
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        Managers.SystemLog.Message("S_LeaveGameHandler");
        var leaveGamePacket = (S_LeaveGame)packet;
    }

    /*public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        //게임에 접속이되면
        Managers.SystemLog.Message("S_ConnectedHandler");

        var lobbyInfo = new C_LobbyInfo();
        lobbyInfo.DeviceId = SystemInfo.deviceUniqueIdentifier;


        Managers.Network.Send(lobbyInfo);
    }*/

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        Managers.SystemLog.Message("S_SpawnHandler");
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

            player.SetDrawLine(info.Shape.Width,info.Shape.Height);
            

            //Spawn Player
            Vector2 vec2 = new Vector2(info.PositionInfo.PosX, info.PositionInfo.PosY);
            player.SpawnPlayer(vec2);
            Managers.SystemLog.Message("S_SpawnHandler : spawnID : " + info.ObjectId);
        }
        //Managers.SystemLog.Message("S_SpawnHandler");*/
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        Managers.SystemLog.Message("S_DespawnHandler");
        var despawn = (S_Despawn)packet;

        foreach (var id in despawn.ObjcetIds) Managers.Object.Remove(id);
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        var movePacket = packet as S_Move;
        var go = Managers.Object.FindById(movePacket.ObjectId);
        
        if (go == null)
        {
            return;
        }

        Managers.SystemLog.Message("S_MoveHandler : " + go.name);
        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        {
            
            return;
        }

        //타 플레이어의 움직임을 조정
       

        var cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.UpdatePosInfo(movePacket.PositionInfo);
        cc.UpdateMoving();
        cc.PosInfo = movePacket.PositionInfo;

        //cc.State = CreatureState.Moving;
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage message)
    {
        Managers.SystemLog.Message("S_ChangeHpHandler");
        var changeHpPacket = message as S_ChangeHp;

        var go = Managers.Object.FindById(changeHpPacket.ObjectId);
        

        if (go != null) 
        {
            go.GetComponent<CreatureController>().Hp = changeHpPacket.Hp;
            if(go.GetComponent<CreatureController>().Hp > changeHpPacket.Hp)
            {
                //HP가 줄었을때 
                //기존의 HP보다 패킷의 HP가 작을때만 Hit 판정
                go.GetComponent<PlayerController>().Hit();
                go.GetComponent<CreatureController>().Hp = changeHpPacket.Hp;
            }
            else if(go.GetComponent<CreatureController>().Hp < changeHpPacket.Hp)
            {
                //HP가 증가했을때 (회복 등)
                //회복 이펙트?
                go.GetComponent<CreatureController>().Hp = Mathf.Min(changeHpPacket.Hp, go.GetComponent<CreatureController>().MaxHp); //과치료 방지

            }

            //go가 나 자신이라면 UI변화
            if (go == Managers.Object.MyPlayer)
            {
                UIManager.Instance.SetHpText();
            }
        }
        else
            Managers.SystemLog.Message("S_ChangeHpHandler : can't find ObjectId");
    }

    public static void S_DieHandler(PacketSession session, IMessage message)
    {
        var diePacket = message as S_Die;
        var go = Managers.Object.FindById(diePacket.ObjectId);
        if (go != null)
            go.GetComponent<CreatureController>().OnDead(diePacket.AttackerId);
        else
            Managers.SystemLog.Message("S_ChangeHpHandler : can't find ObjectId");
    }


    internal static void S_RoomInfoHandler(PacketSession session, IMessage message)
    {
        Managers.SystemLog.Message("S_RoomInfoHandler");
        /* var roomPacket = message as S_RoomInfo;

         if (roomPacket.RoomInfos.Count > 1)
             Managers.Map.MapRoomInfoInit(roomPacket);
         else if (roomPacket.RoomInfos.Count == 1)
             Managers.Map.MapRoomUpdate(roomPacket);*/
    }



    internal static void S_ConnectedHandler(PacketSession session, IMessage message)
    {
        S_Connected packet = message as S_Connected;
        Managers.SystemLog.Message("S_ConnectedHandler");

    }

    internal static void S_ExitGameHandler(PacketSession session, IMessage message)
    {
        S_ExitGame packet = message as S_ExitGame;
        Managers.SystemLog.Message("S_ExitGame");
        if (Managers.Object.MyPlayer == null)
        {
            return;
        }

        if(packet.IsSuccess == true)
        {
            var player = Managers.Object.FindById(packet.PlayerId);

            //나간 플레이어는 이미 디스트로이 된 상태이며 그 외의 플레이어에게서 처리될 패킷
            if (packet.PlayerId == Managers.Object.MyPlayer.Id)
            {
                //플레이어는 자신의 장착칸과 인벤토리의 내용을 서버에 전송?
                //return;
            }

            //클라이언트의 모든 오브젝트의 내용 클리어
            Managers.Object.Clear();
            Managers.Object.DebugDics();

            // 게임 씬을 로비로
            var evniorment = Managers.EnvConfig.GetEnvironmentConfig();
            if (evniorment.LobbyName == "Shelter")
            {
                Managers.Scene.LoadScene(Define.Scene.Shelter);
            }
            else
            {
                Managers.Scene.LoadScene(Define.Scene.Lobby);
            }

            //Managers.Resource.Destroy(player);
            //Managers.Object.Remove(packet.PlayerId);
        }
        else
        {
            ExitZone exitZone = Managers.Object.FindById(packet.ExitId).GetComponent<ExitZone>();
            if (exitZone != null)
            {
                exitZone.CancelExit(packet.RetryTime / 1000.0f);
            }
        }

    }

    internal static void S_JoinServerHandler(PacketSession session, IMessage message)
    {
        S_JoinServer packet = message as S_JoinServer;
        Managers.SystemLog.Message("S_JoinServer");
        if (!packet.Connected)
        {
            Managers.SystemLog.Message("S_JoinServer : fail");
            return;
        }


        //접속 완료
        Managers.Scene.LoadScene(Define.Scene.Forest);


        Managers.SystemLog.Message("S_JoinServer : success");

    }

    internal static void S_WaitingStatusHandler(PacketSession session, IMessage message)
    {
        S_WaitingStatus packet = message as S_WaitingStatus;
        Managers.SystemLog.Message("S_WaitingStatus");
        if (packet == null)
            return;

        //참가시 인원 증감시 호출

        Managers.SystemLog.Message($"S_WaitingStatus : curPlayer : {packet.CurrentPlayers} / {packet.RequiredPlayers}");
    }

    internal static void S_GameStartHandler(PacketSession session, IMessage message)
    {
        Managers.SystemLog.Message("S_GameStartHandler");
        S_GameStart packet = message as S_GameStart;

        //자신의 플레이어 외에 다른 플레이어와 오브젝트의 객체 생성

        foreach (ObjectInfo obj in packet.Objects)
        {
            Managers.Object.Add(obj, false);
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
        //    EquipSlotBase targetSlot = InventoryController.equipSlotDic[(int)gear.Part];

        //    ItemData data = new ItemData();
        //    data.SetItemData(gear.Item);

        //    targetSlot.ApplyItemEffects(data);
        //}
    }
    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ GAMESYSTEM PACKET END ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/

    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ INVENTORY PACKET STARTㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    /// <summary>
    /// 인벤토리를 열고 송신받은 아이템 데이터를 기반으로 아이템을 구성
    /// </summary>
    internal static void S_LoadInventoryHandler(PacketSession session, IMessage message)
    {
        Managers.SystemLog.Message("S_LoadInventory");
        S_LoadInventory packet = message as S_LoadInventory;
        InventoryController inventory = InventoryController.Instance;

        //불러오기에 실패할경우 인벤토리가 off상태로 보장
        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_LoadInventory : fail to load");
            if (inventory.isActive)
            {
                inventory.invenUIControl(false);
            }
            return;
        }

        //인벤토리가 꺼져있으면 킴
        if (!inventory.isActive) 
        {
            inventory.invenUIControl(true);
        }

        //인벤토리에 존재하는 아이템의 데이터 생성
        List<ItemData> packetItemList = new List<ItemData>();
        foreach (PS_ItemInfo packetItem in packet.ItemInfos)
        {
            ItemData convertItem = new ItemData();
            convertItem.SetItemData(packetItem);
            packetItemList.Add(convertItem);
        }

        //플레이어의 인벤토리의 경우
        if(packet.SourceObjectId == 0)
        {
            //장착칸 설정
            foreach (PS_GearInfo packetItem in packet.GearInfos)
            {
                EquipSlotBase targetSlot = InventoryController.equipSlotDic[(int)packetItem.Part];
                ItemData targetItem;
                
                ItemData gearedItem = inventory.GetItemInDictByGearCode((int)packetItem.Part);

                if(gearedItem != null)
                {
                    gearedItem.SetItemData(packetItem.Item);
                    targetItem = gearedItem;
                }
                else
                {
                    targetItem = new ItemData();
                    targetItem.SetItemData(packetItem.Item);
                }

                ItemObject newItem = ItemObject.CreateNewItemObj(targetItem, targetSlot.transform);
                targetSlot.SetItemEquip(newItem);
            }

            //플레이어의 인벤토리
            inventory.playerInvenUI.InventorySet(); //그리드 생성됨

            GridObject playerGrid = inventory.playerInvenUI.instantGrid;
            playerGrid.objectId = packet.SourceObjectId;
            playerGrid.PlaceItemInGrid(packetItemList);
            InventoryController.UpdatePlayerWeight();

            playerGrid.PrintInvenContents();
        }
        //타인의 인벤토리의 경우
        else
        {
            Box box = Managers.Object.FindById(packet.SourceObjectId).GetComponent<Box>();
            //box.interactable = false;

            OtherInventoryUI otherInvenUI = inventory.otherInvenUI;
            otherInvenUI.InventorySet();
            otherInvenUI.instantGrid.InstantGrid(box.size, box.weight);
            GridObject boxGrid = otherInvenUI.instantGrid;

            boxGrid.objectId = packet.SourceObjectId;
            boxGrid.PlaceItemInGrid(packetItemList);

            boxGrid.PrintInvenContents();
        }

        inventory.DebugDic();
    }

    /// <summary>
    /// 인벤토리를 닫기
    /// </summary>
    internal static void S_CloseInventoryHandler(PacketSession session, IMessage message)
    {
        S_CloseInventory packet = message as S_CloseInventory;
        Managers.SystemLog.Message("S_CloseInventory");
        InventoryController inventory = InventoryController.Instance;

        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_CloseInventory : Didn't accepted by Server");
            return;
        }

        //if(packet.SourceObjectId != 0) 서버에서도 확인하니 생략
        //{
        //    //박스의 interactable을 
        //    GameObject target = Managers.Object.FindById(packet.SourceObjectId);
        //    if(target == null)
        //    {
        //        Managers.SystemLog.Message($"S_CloseInventory : fail to find with ObjectId {packet.SourceObjectId}");
        //        return;
        //    }
        //    target.GetComponent<Box>().interactable = true;
        //}

        if (InventoryController.Instance.isActive)//인벤토리가 켜져 있으면 끔
        {
            InventoryController.Instance.invenUIControl(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal static void S_SearchItemHandler(PacketSession session, IMessage message)
    {
        S_SearchItem packet = message as S_SearchItem;
        
        Managers.SystemLog.Message($"S_SearchItem : target = {packet.SourceItem.ObjectId}");

        ItemObject targetItem = InventoryController.instantItemDic.GetValueOrDefault(packet.SourceItem.ObjectId, null);
        if (targetItem == null)
        {
            Managers.SystemLog.Message($"S_SearchItem : can't find itemObject");
            return;
        }

        if (packet.IsSuccess)
        {
            targetItem.RevealItem();
        }
        else
        {
            targetItem.HideItem();
        }
    }

    //나중에 서버의 인벤토리 방식을 바꿔 변경 시 아이템 오브젝트의 아이디가 바뀌는 현상이 제거되면 없앨 예정
    private static void ChangeItemObjectId(ItemObject targetItem, int newId)
    {
        int id = targetItem.itemData.objectId;
        InventoryController.instantItemDic.Remove(targetItem.itemData.objectId);
        targetItem.itemData.objectId = newId;
        if (!InventoryController.instantItemDic.ContainsKey(targetItem.itemData.objectId))
        {
            InventoryController.instantItemDic.Add(targetItem.itemData.objectId, targetItem);
        }
        Managers.SystemLog.Message($"change ObjectId : OldId = {id} NewId = {newId}");
    }

    private static void IsGearSlotOrGrid(int objectId, ref EquipSlotBase equipSlot, ref GridObject gridObject)
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
        return objectId == 0 ? InventoryController.Instance.playerInvenUI.instantGrid : InventoryController.Instance.otherInvenUI.instantGrid;
    }

    internal static void S_MoveItemHandler(PacketSession session, IMessage message)
    {
        S_MoveItem packet = message as S_MoveItem;
        Managers.SystemLog.Message("S_MoveItem");
        InventoryController inventory = InventoryController.Instance;

        ItemObject targetItem = InventoryController.instantItemDic.GetValueOrDefault(packet.SourceMoveItem.ObjectId, null);
        if (targetItem == null)
        {
            Managers.SystemLog.Message($"S_MoveItem : can't find with this ObjectId : {packet.SourceMoveItem.ObjectId}");
            return;
        }

        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlotBase sourceEquip = null;
        EquipSlotBase destinationEquip = null;

        IsGearSlotOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        IsGearSlotOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);

        if (packet.IsSuccess)
        {
            if (packet.DestinationObjectId > 0 && packet.DestinationObjectId <= 7)
            {
                //도착지점이 장착칸 -> 해당 아이템을 장착칸에 장착
                if (!destinationEquip.SetItemEquip(targetItem)) //장착 실패시 원위치로
                {
                    inventory.UndoSlot(targetItem);
                    inventory.UndoItem(targetItem);
                }
            }
            else
            {
                //도착지점이 인벤칸 -> 해당 아이템을 인벤토리에 배치
                destinationGrid.PlaceItem(targetItem, packet.DestinationMoveItem.X, packet.DestinationMoveItem.Y);
                targetItem.Rotate(packet.DestinationMoveItem.Rotate); //주어진 회전도로 회전
            }

            inventory.BackUpSlot(targetItem);
            inventory.BackUpItem(targetItem);
        }
        else
        {
            inventory.UndoSlot(targetItem);
            inventory.UndoItem(targetItem);
        }

        ChangeItemObjectId(targetItem, packet.DestinationMoveItem.ObjectId);
        InventoryController.UpdatePlayerWeight();
    }

    internal static void S_DeleteItemHandler(PacketSession session, IMessage message)
    {
        S_DeleteItem packet = message as S_DeleteItem;
        Managers.SystemLog.Message($"S_DeleteItem : targetId = {packet.DeleteItem.ObjectId}");

        InventoryController inventory = InventoryController.Instance;

        ItemObject targetItem = InventoryController.instantItemDic.GetValueOrDefault(packet.DeleteItem.ObjectId, null);
        if (targetItem == null)
        {
            Managers.SystemLog.Message($"S_DeleteItem : can't find object with {packet.DeleteItem.ObjectId}");
            return;
        }

        GridObject sourceGrid = null;
        EquipSlotBase sourceEquip = null;
        IsGearSlotOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);


        if (!packet.IsSuccess) {
            Managers.SystemLog.Message("S_DeleteItem : failed");
            inventory.UndoSlot(targetItem);
            inventory.UndoItem(targetItem);
            return;
        }

        inventory.DestroyItem(targetItem);
        InventoryController.UpdatePlayerWeight();
    }

    internal static void S_MergeItemHandler(PacketSession session, IMessage message)
    {
        S_MergeItem packet = message as S_MergeItem;
        Managers.SystemLog.Message($"S_MergeItem : 합쳐지는 아이템 아이디 = {packet.MergedItem.ObjectId}, 합치기 위한 아이디 = {packet.CombinedItem.ObjectId}");


        InventoryController inventory = InventoryController.Instance;

        ItemObject mergedItem = InventoryController.instantItemDic.GetValueOrDefault(packet.MergedItem.ObjectId, null);
        ItemObject combinedItem = InventoryController.instantItemDic.GetValueOrDefault(packet.CombinedItem.ObjectId, null);

        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_MergeItem failed");
            InventoryController.Instance.UndoSlot(combinedItem);
            InventoryController.Instance.UndoItem(combinedItem);
            return;
        }

        if (mergedItem == null || combinedItem == null)
        {
            inventory.DebugDic();
            Managers.SystemLog.Message("S_MergeItem : cant find with those item");
            Managers.SystemLog.Message($"packet.mergeItem = {packet.MergedItem.ObjectId}");
            Managers.SystemLog.Message($"packet.combinedItem = {packet.CombinedItem.ObjectId}");
            return;
        }

        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlotBase sourceEquip = null;
        EquipSlotBase destinationEquip = null;

        IsGearSlotOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        IsGearSlotOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);

        //각각 변화한 아이템 양 적용
        mergedItem.ItemAmount = packet.MergedItem.Amount; //개수 증량
        combinedItem.ItemAmount = packet.CombinedItem.Amount; //개수 감소 혹은 삭제

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
            inventory.DestroyItem(combinedItem);
        }
        else
        {
            inventory.UndoSlot(combinedItem);
            inventory.UndoItem(combinedItem);
        }

        InventoryController.UpdatePlayerWeight();
    }


    internal static void S_DevideItemHandler(PacketSession session, IMessage message)
    {
        S_DevideItem packet = message as S_DevideItem;
        Managers.SystemLog.Message("S_Divide");
        InventoryController inventory = InventoryController.Instance;

        ItemObject sourceItem = InventoryController.instantItemDic.GetValueOrDefault(packet.SourceItem.ObjectId, null); //원래 있던 아이템
        if (sourceItem == null )
        {
            Managers.SystemLog.Message($"S_Divide : can't find object with ObjectId {packet.SourceItem.ObjectId}");
            return;
        }

        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_Divide failed");
            inventory.UndoSlot(sourceItem);
            inventory.UndoItem(sourceItem);
        }

        GridObject sourceGrid = null;
        GridObject destinationGrid = null;
        EquipSlotBase sourceEquip = null;
        EquipSlotBase destinationEquip = null;

        IsGearSlotOrGrid(packet.SourceObjectId, ref sourceEquip, ref sourceGrid);
        IsGearSlotOrGrid(packet.DestinationObjectId, ref destinationEquip, ref destinationGrid);

        Managers.SystemLog.Message($"S_Divide : oldId = {packet.SourceItem.ObjectId}, newId = {packet.DestinationItem.ObjectId}");
        inventory.UndoSlot(sourceItem); //원래 아이템은 원위치로 이동후 나눈 만큼 아이템 감소
        inventory.UndoItem(sourceItem);
        sourceItem.ItemAmount = packet.SourceItem.Amount;

        if (packet.DestinationObjectId > 0 && packet.DestinationObjectId <= 7)
        {
            //도착지점이 장착칸 -> 이 경우는 소모품의 경우 
            ItemData itemData = new ItemData();
            itemData.SetItemData(packet.DestinationItem);

            ItemObject newItem = ItemObject.CreateNewItemObj(itemData, destinationEquip.transform);

            if (!destinationEquip.SetItemEquip(newItem))
            {
                inventory.UndoSlot(newItem);
                inventory.UndoItem(newItem);
            }
        }
        else
        {
            //도착지점이 인벤칸 -> 새로운 아이템을 생성하여 나눈 아이템 생성
            ItemData newData = new ItemData();
            newData.SetItemData(packet.DestinationItem);

            destinationGrid.CreateItemObjAndPlace(newData);
        }

        inventory.BackUpSlot(sourceItem);
        inventory.BackUpItem(sourceItem);

        ChangeItemObjectId(sourceItem, packet.SourceItem.ObjectId);
        InventoryController.UpdatePlayerWeight();
    }

    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ INVENTORY PACKET ENDㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/

    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ GUN PACKET START ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    //총알의 시작지점과 끝지점을 받아 총알을 발사해 궤적을 그림
    internal static void S_RaycastShootHandler(PacketSession session, IMessage message)
    {
        S_RaycastShoot packet = message as S_RaycastShoot;
        Managers.SystemLog.Message("S_RaycastShoot");

        //hit으로 인한 데미지는 다른 패킷으로 줌 -> ChangeHpHandler

        //총을 쏜 객체만 
        GameObject shootingPlayer = Managers.Object.FindById(packet.ShootPlayerId);
        
        Vector2 hitPoint = new Vector2(packet.HitPointX, packet.HitPointY);
        Vector2 startPoint = new Vector2(packet.StartPosX, packet.StartPosY);
        

        if (shootingPlayer.GetComponent<PlayerController>().Id == Managers.Object.MyPlayer.Id)
        {
            //쏜사람이 플레이어라면 총알감소 및 총알 텍스트 변경
            Gun shooterGun = Managers.Object.MyPlayer.usingGun;
            if (shooterGun == null)
            {
                Debug.LogError("S_RaycastShootHandler error : 총 스크립트를 찾을수 없음");
            }
            shooterGun.gunLine.SetBulletLine(startPoint, hitPoint);

            shooterGun.UseAmmo();
            UIManager.Instance.SetAmmoText();

            shooterGun.StartEffect();
        }

        //총알 발사
        Bullet bullet = Managers.Resource.Instantiate($"Objects/BulletObjPref/BulletBase").GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.Log("리소스에서 총알 로드 실패");
            return;
        }

        Vector2 dir = (hitPoint - startPoint).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bullet.transform.position = startPoint;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        bullet.startPos = startPoint;
        bullet.endPos = hitPoint;


        float distance = Vector2.Distance(bullet.startPos, bullet.endPos);
        Debug.Log($"이동거리 = {distance}");

        

        Managers.SystemLog.Message($"S_RaycastShoot : startPos {startPoint}, endPos {hitPoint}");
    }

    

    internal static void S_ChangeAppearanceHandler(PacketSession session, IMessage message)
    {
        S_ChangeAppearance packet = message as S_ChangeAppearance;
        Managers.SystemLog.Message("S_ChangeAppearanceHandler");
        Managers.SystemLog.Message($"S_ChangeAppearanceHandler {packet.ObjectId}, {packet.GunType.Part}");

        if(packet.ObjectId == Managers.Object.MyPlayer.Id)
        {
            //return;
        }

        GameObject targetPlayer = Managers.Object.FindById(packet.ObjectId);
       
            
        if (packet.GunType.Part == 0)
        {
            Managers.SystemLog.Message("S_ChangeAppearanceHandler : no usingGun in hand");
            targetPlayer.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
            return;
        }

        //Sprite targetSprite = Resources.Load<Sprite>($"Sprite/Item/{Data_master_item_base.GetData(packet.GunId).icon}");
        Sprite targetSprite = Resources.Load<Sprite>($"Sprite/Item/{Data_master_item_base.GetData(packet.GunType.Item.ItemId).icon}");
        if(targetSprite == null)
        {
            Managers.SystemLog.Message("S_ChangeAppearanceHandler : Can't find item with packet.GunId");
        }
        Managers.SystemLog.Message($"S_ChangeAppearanceHandler : spriteName : {targetSprite.name}");

        targetPlayer.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = targetSprite;
        Managers.SystemLog.Message($"S_ChangeAppearanceHandler : targetPlayer : {targetPlayer.name}");
        
    }
    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ GUN PACKET END ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/


    internal static void S_UpdateQuestHandler(PacketSession session, IMessage message)
    {
        S_UpdateQuest packet = message as S_UpdateQuest;
        Managers.SystemLog.Message("S_UpdateQuestHandler");

        MyPlayerController player = Managers.Object.MyPlayer;
        if (player != null)
        {
            var quest = packet.Quest;
            player._Quest.UpdateQuest(quest.Id, quest.Progress, quest.Completed);
        }

    }

    internal static void S_GundataUpdateHandler(PacketSession session, IMessage message)
    {
        S_GundataUpdate packet = message as S_GundataUpdate;

       // Managers.SystemLog.Message($"{packet.GunData}");
        Managers.SystemLog.Message("S_GundataUpdateHandler");

        //서버 완성시 해제
        Managers.Object.MyPlayer.usingGun.ReloadDone(packet.GunData.Item.Attributes.LoadedAmmo);

    }

    internal static void S_TrapActionHandler(PacketSession session, IMessage message)
    {
        S_TrapAction packet = message as S_TrapAction;
        Managers.SystemLog.Message("S_TrapActionHandler");

        if(packet != null && packet.IsActive)
        {
            Mine mine = Managers.Object.FindById(packet.ObjectId).GetComponent<Mine>();
            mine.Explosion(packet.ObjectId);
        }
    }

    internal static void S_AiMoveHandler(PacketSession session, IMessage message)
    {
        S_AiMove packet = message as S_AiMove;
        Managers.SystemLog.Message($"S_AiMove {packet.ObjectId}");
        
        GameObject enemy = Managers.Object.FindById(packet.ObjectId);
        Vector2 instance = enemy.transform.position;
        foreach (Vector2IntInfo info in packet.PosList)
        {
            Debug.DrawLine(instance, new Vector2(info.X,info.Y), Color.red);
            instance = new Vector2(info.X, info.Y);
        }
    }

    internal static void S_AiSpawnHandler(PacketSession session, IMessage message)
    {
        S_AiSpawn packet = message as S_AiSpawn;

        GameObject enemy = Managers.Object.FindById(packet.ObjectId);

        enemy.GetComponent<EnemyAI>().SetData(packet);
    }
    internal static void S_AiAttackHandler(PacketSession session, IMessage message)
    {
        S_AiSpawn packet = message as S_AiSpawn;

        GameObject enemy = Managers.Object.FindById(packet.ObjectId);

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

         Managers.SystemLog.Message("S_SkillHandler");
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

        Managers.SystemLog.Message($"previous : S_Stat {statpacket.ObjectId}{cc.Stat}");

        cc.Stat.MergeFrom(statpacket.StatInfo);

        #region IsPlayer
        MyPlayerController mc = cc as MyPlayerController;
        if (mc != null)
        {
            mc.CheakUpdateLevel();
        }
        #endregion
        Managers.SystemLog.Message($"Next : S_Stat {statpacket.StatInfo}");
    }*/

    /*internal static void S_LobbyPlayerInfoHandler(PacketSession session, IMessage message)
    {
        //서버에서 로비에관한 정보
        Managers.SystemLog.Message("S_LobbyPlayerInfoHandler");
        var lobbyPlayerInfo = (S_LobbyPlayerInfo)message;
        GameObject.Find("LobbyScene").GetComponent<LobbyScene>().DataUpdate(lobbyPlayerInfo);
    }*/
}
