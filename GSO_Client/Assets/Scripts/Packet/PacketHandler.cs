﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using MathNet.Numerics.LinearAlgebra.Factorization;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg;
using Server.Data;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


internal class PacketHandler
{
    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ GAMESYSTEM PACKET START ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        
        
        Managers.SystemLog.Message("S_EnterGameHandler");
        var enterGamePacket = (S_EnterGame)packet;
        Managers.Object.Clear();

        UIManager.Instance.leftTime = enterGamePacket.GameData.LeftTime;

        Managers.SystemLog.Message($"{enterGamePacket.Player}");
        Managers.Object.Add(enterGamePacket.Player, true);

        Managers.Object.MyPlayer.GetComponent<DebugShape>().SetDrawLine(enterGamePacket.Player.Shape.Width, enterGamePacket.Player.Shape.Height);

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
        //플레이어 제외 오브젝트
        var spawnPacket = (S_Spawn)packet;
        
        foreach (var info in spawnPacket.Objects)
        {
            Managers.Object.Add(info, false);
            DebugShape ds = Managers.Object.FindById(info.ObjectId).GetComponent<DebugShape>();
            if (ds != null)
            {
                ds.SetDrawLine(info.Shape.Width, info.Shape.Height);
            }

            var type = (info.ObjectId >> 24) & 0x7f;
            var creature = Managers.Object.FindById(info.ObjectId).GetComponent<CreatureController>();
            if(creature == null)
            {
                continue;
            }

            var Stats = info.StatInfo;
            creature.Hp = Stats.Hp;
            creature.MaxHp = Stats.MaxHp;
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

        //Managers.SystemLog.Message("S_MoveHandler : " + go.name);
        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
        {
            return;
        }

        //지정 오브젝트의 이동위치로 이동
        var cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.UpdatePosInfo(movePacket.PositionInfo);
        cc.UpdateMoving();   
        cc.PosInfo = movePacket.PositionInfo;//??

        //디버그라인 업데이트
        var ds = go.GetComponent<DebugShape>();
        if(ds != null)
        {
            ds.UpdateDrawLine();
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage message)
    {
        Managers.SystemLog.Message("S_ChangeHpHandler");
        var changeHpPacket = message as S_ChangeHp;

        var go = Managers.Object.FindById(changeHpPacket.ObjectId);
        if(go == null)
        {
            Managers.SystemLog.Message("S_ChangeHpHandler : can't find ObjectId");
            return;
        }
        
        var creature = go.GetComponent<CreatureController>();
        if(creature == null)
        {
            Managers.SystemLog.Message("S_ChangeHpHandler : obj is not creature");
            return;
        }

        if (creature.Hp > changeHpPacket.Hp)
        {
            //피격
            creature.Hit();
            if (go.GetComponent<MyPlayerController>() != null)
                creature.GetComponent<MyPlayerController>().StartCoroutine(UIManager.Instance.SetHitEffect());
            creature.Hp = changeHpPacket.Hp;
        }
        else if (creature.Hp < changeHpPacket.Hp)
        {
            //회복
            creature.Hp = Mathf.Min(changeHpPacket.Hp, creature.MaxHp); //과치료 방지
        }

        creature.Hp = changeHpPacket.Hp;
        //go가 나 자신이라면 UI변화
        if (creature.Id == Managers.Object.MyPlayer.Id)
        {
            UIManager.Instance.SetHpText();
        }
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
        //모든 플레이어에게 전송되는 핸들러
        S_ExitGame packet = message as S_ExitGame;
        Managers.SystemLog.Message("S_ExitGame");

        if (Managers.Object.MyPlayer == null)
        {
            return;
        }

        
        if (!packet.IsSuccess)
        {
            //실패했을경우 exit을 보낸 플레이어의 exitzone 반응 제거
            if (packet.PlayerId != Managers.Object.MyPlayer.Id)
            {
                return;
            }

            ExitZone exitZone = Managers.Object.FindById(packet.ExitId).GetComponent<ExitZone>();
            if (exitZone != null)
            {
                exitZone.CancelExit(packet.RetryTime / 1000.0f);
            }

            return;
        }

        var targetPlayer = Managers.Object.FindById(packet.PlayerId);

        //나간 플레이어의 경우 초기화 및 씬 이동
        if (packet.PlayerId == Managers.Object.MyPlayer.Id)
        {
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
        }

        //오브젝트 딕셔너리에서 해당 플레이어를 삭제 ( 나와 다른 플레이어에게 동일하게 적용)
        Managers.Resource.Destroy(targetPlayer);
        Managers.Object.Remove(packet.PlayerId);

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

        Managers.Scene.LoadScene(Define.Scene.loading);
        //접속 완료
        //Managers.Scene.LoadScene(Define.Scene.Forest);


        Managers.SystemLog.Message("S_JoinServer : success");


        /*C_EnterGame c_EnterGame = new C_EnterGame();
        //c_EnterGame.Credential =

        Managers.Network.Send(c_EnterGame);
        Debug.Log("Send c_EnterGame In GameScene");*/


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
        FadeManager.instance.SetLoadComplete();
        
        //자신의 플레이어 외에 다른 플레이어 생성
        foreach (ObjectInfo obj in packet.Objects)
        {
            Managers.Object.Add(obj, false);
            //Spawn Player

            //Vector2 vec2 = new Vector2(obj.PositionInfo.PosX, obj.PositionInfo.PosY);

            var player = Managers.Object.FindById(obj.ObjectId).GetComponent<CreatureController>();
            var Stats = obj.StatInfo;
            player.Hp = Stats.Hp;
            player.MaxHp = Stats.MaxHp;

            //if (targetPlayer == null)
            //{
            //    continue;
            //}
            //targetPlayer.SpawnPlayer(vec2);
            Managers.SystemLog.Message("S_SpawnHandler : spawnID : " + obj.ObjectId);
        }
        
        UIManager.Instance.StopLoading();

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

        InventoryController.Instance.OnWaitSwitchPacket = false;
        
        //불러오기에 실패할경우 인벤토리가 off상태로 보장
        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_LoadInventory : fail to load");

            if (inventory.isActive)
            {
                inventory.invenUIControl(false);
            }

            InventoryController.Instance.interactBoxId = null;
 
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
        if (packet.SourceObjectId == InventoryController.PlayerSlotId)
        {
            //장착칸 설정
            foreach (PS_GearInfo packetItem in packet.GearInfos)
            {
                EquipSlotBase targetSlot = InventoryController.equipSlotDic[(int)packetItem.Part];
                ItemData targetItem;

                ItemData gearedItem = inventory.GetItemInDictByGearCode((int)packetItem.Part);

                if (gearedItem != null)
                {
                    gearedItem.SetItemData(packetItem.Item);
                    targetItem = gearedItem;
                }
                else
                {
                    targetItem = new ItemData();
                    targetItem.SetItemData(packetItem.Item);
                }

                if (targetItem.itemId == 300)
                {
                    Debug.Log("300번 기본가방은 생성안함 = 제외");
                    continue;
                }

                ItemObject newItem = ItemObject.InstantItemObj(targetItem, targetSlot.transform);
                targetSlot.SetItemEquip(newItem);
            }

            //플레이어의 인벤토리
            inventory.playerInvenUI.InventorySet(); //그리드 생성

            GridObject playerGrid = inventory.playerInvenUI.instantGrid;
            playerGrid.objectId = packet.SourceObjectId;
            playerGrid.PlaceItemsInGrid(packetItemList);

            InventoryController.UpdateInvenWeight();
            InventoryController.UpdateInvenWeight(false); //other인벤이 없을경우 other의 무게 텍스트를 초기화 하기위해 필요
        }
        //타인의 인벤토리의 경우
        else
        {
            Box box = Managers.Object.FindById(packet.SourceObjectId).GetComponent<Box>();

            OtherInventoryUI otherInvenUI = inventory.otherInvenUI;
            otherInvenUI.InventorySet();
            otherInvenUI.instantGrid.InstantGrid(box.size, box.weight);
            GridObject boxGrid = otherInvenUI.instantGrid;

            boxGrid.objectId = packet.SourceObjectId;
            boxGrid.PlaceItemsInGrid(packetItemList);

            InventoryController.UpdateInvenWeight(false);
            InventoryPacket.SendLoadInvenPacket();
        }
    }

    /// <summary>
    /// 인벤토리를 닫기
    /// </summary>
    internal static void S_CloseInventoryHandler(PacketSession session, IMessage message)
    {
        S_CloseInventory packet = message as S_CloseInventory;
        Managers.SystemLog.Message("S_CloseInventory");
        InventoryController inventory = InventoryController.Instance;
        InventoryController.Instance.OnWaitSwitchPacket = false;
        InventoryController.Instance.interactBoxId = null;

        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_CloseInventory : Didn't accepted by Server");
            
            return;
        }

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

        if (false == packet.IsSuccess)
        {
            targetItem.HideItem();
        }
       
    }

    private static void IsGearSlotOrGrid(int objectId, ref EquipSlotBase equipSlot, ref GridObject gridObject)
    {
        if (objectId > InventoryController.PlayerSlotId && objectId <= InventoryController.MaxEquipSlots)
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
        return objectId == InventoryController.PlayerSlotId ? InventoryController.Instance.playerInvenUI.instantGrid : InventoryController.Instance.otherInvenUI.instantGrid;
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
            if (packet.DestinationObjectId > InventoryController.PlayerSlotId && packet.DestinationObjectId <= InventoryController.MaxEquipSlots)
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

        InventoryController.UpdateInvenWeight();
        InventoryController.UpdateInvenWeight(false);
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
        InventoryController.UpdateInvenWeight();
        InventoryController.UpdateInvenWeight(false);
    }

    internal static void S_MergeItemHandler(PacketSession session, IMessage message)
    {
        S_MergeItem packet = message as S_MergeItem;
        Managers.SystemLog.Message($"S_MergeItem : 감소 아이템 아이디 = {packet.MergedItem.ObjectId}, 증가 아이템 아이디 = {packet.CombinedItem.ObjectId}");


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
        mergedItem.ItemAmount = packet.MergedItem.Amount; //개수 감소
        combinedItem.ItemAmount = packet.CombinedItem.Amount; //개수 증가

        if (sourceEquip != null)
        {
            sourceEquip.GetComponent<RecoverySlot>().UpdateQuickSlotAmount(mergedItem.ItemAmount);
        }

        if (destinationEquip != null)
        {
            destinationEquip.GetComponent<RecoverySlot>().UpdateQuickSlotAmount(combinedItem.ItemAmount);
        }

        //옮긴 아이템의 양이 0개가 되면 파괴 아니면 원래 위치로 이동
        if (mergedItem.ItemAmount <= 0)
        {
            inventory.DestroyItem(mergedItem);
        }
        else
        {
            inventory.UndoSlot(mergedItem);
            inventory.UndoItem(mergedItem);
        }

        InventoryController.UpdateInvenWeight();
        InventoryController.UpdateInvenWeight(false);
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
            Debug.LogError($"S_Divide : can't find object with ObjectId {packet.SourceItem.ObjectId}");
            return;
        }

        if (!packet.IsSuccess)
        {
            Managers.SystemLog.Message("S_Divide failed");
            inventory.UndoSlot(sourceItem);
            inventory.UndoItem(sourceItem);
            return;
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

        if (packet.DestinationObjectId > InventoryController.PlayerSlotId && packet.DestinationObjectId <= InventoryController.MaxEquipSlots)
        {
            //도착지점이 장착칸 -> 이 경우는 소모품의 경우 
            ItemData itemData = new ItemData();
            itemData.SetItemData(packet.DestinationItem);

            ItemObject newItem = ItemObject.InstantItemObj(itemData, destinationEquip.transform);

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

            destinationGrid.PlaceItemAfterCreate(newData);
        }

        inventory.BackUpSlot(sourceItem);
        inventory.BackUpItem(sourceItem);

        InventoryController.UpdateInvenWeight();
        InventoryController.UpdateInvenWeight(false);
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
        
        if (shootingPlayer == null)
        {
            Debug.LogWarning("shootingPlayer is null");
        }


        Vector2 hitPoint = new Vector2(packet.HitPointX, packet.HitPointY);
        Vector2 startPoint = new Vector2(packet.StartPosX, packet.StartPosY);

        Debug.Log($"startPoint {startPoint},hitPoint {hitPoint} ");

        if (shootingPlayer.GetComponent<CreatureController>().Id == Managers.Object.MyPlayer.Id)
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

        GameObject targetPlayer = Managers.Object.FindById(packet.ObjectId);


        SpriteRenderer gunSprite = targetPlayer.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

        if (packet.GunType == null ||  packet.GunType.Part == 0)
        {
            //총의 스프라이트
            gunSprite.sprite = null;
            return;
        }
        //var t = targetPlayer.GetComponent<PlayerController>().usingGun = packet.GunType.Part;


        Sprite targetSprite = Resources.Load<Sprite>($"Sprite/Item/{Data_master_item_base.GetData(packet.GunType.Item.ItemId).icon}");
        if(targetSprite == null)
        {
            Managers.SystemLog.Message("S_ChangeAppearanceHandler : Can't find item with packet.GunId");
            Debug.LogError("S_ChangeAppearanceHandler : Can't find item with packet.GunId");
            return;
        }
        gunSprite.sprite = targetSprite;
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
        Managers.SystemLog.Message("S_GundataUpdateHandler");

        var go = Managers.Object.FindById(packet.OwnerId);

        if (go == null)
        {
            Debug.Log("아이디를 찾지못함");
            return;
        }

        if(packet.OwnerId == Managers.Object.MyPlayer.Id)
        {

            if (packet.GunRoation != null)
            {
                //Managers.Object.MyPlayer.usingGun.GunRoationHandle(packet.GunRoation.Roation);
                //return;
            }
            else
            {
                if (packet.GunReloadSuccess)
                {
                    Managers.Object.MyPlayer.usingGun.ReloadDone(packet.GunData.Item.Attributes.LoadedAmmo);
                }
                else
                {
                    Managers.Object.MyPlayer.usingGun.ReloadFail();
                }
            }
        }
        else
        {

            if(packet.GunRoation != null)
            {
                PlayerController r = go.GetComponent<PlayerController>();
                var t = r.usingGun;
                t.GunRoationHandle(packet.GunRoation.Roation);
            }
        }
    }

    internal static void S_TrapActionHandler(PacketSession session, IMessage message)
    {
        S_TrapAction packet = message as S_TrapAction;
        Managers.SystemLog.Message("S_TrapActionHandler");

        if(packet != null && packet.IsActive)
        {
            var go = Managers.Object.FindById(packet.ObjectId);
            if(go == null)
            {
                Debug.Log("아이디를 찾지못함");
                return;
            }
            Debug.Log($"{packet.ObjectId}폭발!");
            Mine mine = go.GetComponent<Mine>();
            mine.Explosion(packet.ObjectId);
        }
    }

    internal static void S_AiMoveHandler(PacketSession session, IMessage message)
    {
        S_AiMove packet = message as S_AiMove;
        //Managers.SystemLog.Message($"S_AiMove {packet.ObjectId}");
        
        GameObject enemy = Managers.Object.FindById(packet.ObjectId);
        Vector2 instance = enemy.transform.position;

        //Debug.Log(packet.PosList.ToList().Count); 
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

    internal static void S_AiAttackReadyHandler(PacketSession session, IMessage message)
    {
        S_AiAttackReady packet = message as S_AiAttackReady;

        EnemyAI enemy = Managers.Object.FindById(packet.ObjectId).GetComponent<EnemyAI>();
        if(packet.Start != null)
        {
            //원거리
            enemy.DrawAttackLine(new Vector2(packet.Start.X, packet.Start.Y), new Vector2(packet.Dir.X, packet.Dir.Y));
            Debug.Log("원거리 공격 라인");
        }
        else
        {
            //근거리
            enemy.DrawAttackLine(new Vector2(packet.Shape.CenterPosX, packet.Shape.CenterPosY), packet.Shape.Width, packet.Shape.Height);
            Debug.Log("근거리 공격 라인");
        }


    }

    internal static void S_AiAttackShotHandler(PacketSession session, IMessage message)
    {
        S_AiAttackShot packet = message as S_AiAttackShot;

        EnemyAI enemy = Managers.Object.FindById(packet.ObjectId).GetComponent<EnemyAI>();
        enemy.ClearLine();
        enemy.SetAniamtionAttack();
    }


    static ulong _last = 0;
    internal static void S_PingHandler(PacketSession session, IMessage message)
    {
        S_Ping packet = message as S_Ping;

        if(packet.IsEnd  == true)
        {

            Managers.Network.ResetTick(packet.Tick, (uint)(LogicTimer.Tick - _last) / 2);

        }
        else
        {
            //LogicTimer.Tick = packet.Tick;
            
            C_Pong c_Pong = new C_Pong();
            //c_Pong.Tick = LogicTimer.Tick;
            Managers.Network.Send(c_Pong);

            _last = LogicTimer.Tick;
            //Debug.Log("Last : "+ _last);
        }

    }

    internal static void S_ErrorHandler(PacketSession session, IMessage message)
    {
        S_Error s_Error = message as S_Error;

        switch (s_Error.ErrorCode)
        {
            case ErrorType.Success:
                //성공
                Debug.Log("에러 없음");
                if (NotifyUI.instance != null)
                {
                    NotifyUI.instance.SetContent("Not Error");
                    NotifyUI.instance.SetTitle("Not Error");
                    NotifyUI.instance.Show();
                }
                break;
            case ErrorType.ServerLoading:
                //서버 초기화중
                Debug.Log("ServerLoading:" +  s_Error.ErrorStr); //241220 승현ErrorStr는 없다고 생각하고 그냥
                if (NotifyUI.instance != null)
                {
                    NotifyUI.instance.SetContent("ServerLoading:" +  s_Error.ErrorStr);
                    NotifyUI.instance.SetTitle("ServerLoading");
                    NotifyUI.instance.Show();
                }
                break;
            case ErrorType.ConnectionLost:
                Debug.Log("ConnectionLost:" + s_Error.ErrorStr);
                break;
            case ErrorType.Timeout:
                Debug.Log("Timeout:" + s_Error.ErrorStr);
                break;
            case ErrorType.InvalidRequest:
                Debug.Log("InvalidRequest:" + s_Error.ErrorStr);
                break;
            case ErrorType.Unauthorized:
                Debug.Log("Unauthorized:" + s_Error.ErrorStr);
                break;
            case ErrorType.Forbidden:
                Debug.Log("Forbidden:" + s_Error.ErrorStr);
                break;
            case ErrorType.ResourceNotFound:
                Debug.Log("ResourceNotFound:" + s_Error.ErrorStr);
                break;
            case ErrorType.InternalServerError:
                Debug.Log("InternalServerError:" + s_Error.ErrorStr);
                break;
            case ErrorType.MaintenanceMode:
                Debug.Log("MaintenanceMode:" + s_Error.ErrorStr);
                break;
            case ErrorType.DataCorruption:
                Debug.Log("DataCorruption:" + s_Error.ErrorStr);
                break;
            case ErrorType.DuplicateAction:
                Debug.Log("DuplicateAction:" + s_Error.ErrorStr);
                break;
            case ErrorType.RateLimitExceeded:
                Debug.Log("RateLimitExceeded:" + s_Error.ErrorStr);
                break;
            case ErrorType.VersionMismatch:
                Debug.Log("VersionMismatch:" + s_Error.ErrorStr);
                break;
            case ErrorType.PaymentRequired:
                Debug.Log("PaymentRequired:" + s_Error.ErrorStr);
                break;
            case ErrorType.Banned:
                Debug.Log("Banned:" + s_Error.ErrorStr);
                break;
            case ErrorType.InvalidSession:
                Debug.Log("InvalidSession:" + s_Error.ErrorStr);
                break;
            case ErrorType.OutOfResources:
                Debug.Log("OutOfResources:" + s_Error.ErrorStr);
                break;
            case ErrorType.MatchmakingFailed:
                Debug.Log("MatchmakingFailed:" + s_Error.ErrorStr);
                if (NotifyUI.instance != null)
                {
                    NotifyUI.instance.SetContent("MatchmakingFailed:"+s_Error.ErrorStr);
                    NotifyUI.instance.SetTitle("MatchmakingFailed");
                    NotifyUI.instance.Show();
                }
                break;
            case ErrorType.InvalidGameState:
                Debug.Log("InvalidGameState:" + s_Error.ErrorStr);
                break;
            case ErrorType.CheatingDetected:
                Debug.Log("CheatingDetected:" + s_Error.ErrorStr);
                break;
            default:
                break;
        }
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
