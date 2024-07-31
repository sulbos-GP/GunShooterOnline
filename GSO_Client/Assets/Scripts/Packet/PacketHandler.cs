using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
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