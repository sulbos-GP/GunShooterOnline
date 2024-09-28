using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
	public Action<PacketSession,IMessage,ushort> CustomHandler { get; set; }
		
	public void Register()
	{
				
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SLoadInventory, MakePacket<S_LoadInventory>);
		_handler.Add((ushort)MsgId.SLoadInventory, PacketHandler.S_LoadInventoryHandler);		
		_onRecv.Add((ushort)MsgId.SCloseInventory, MakePacket<S_CloseInventory>);
		_handler.Add((ushort)MsgId.SCloseInventory, PacketHandler.S_CloseInventoryHandler);		
		_onRecv.Add((ushort)MsgId.SMergeItem, MakePacket<S_MergeItem>);
		_handler.Add((ushort)MsgId.SMergeItem, PacketHandler.S_MergeItemHandler);		
		_onRecv.Add((ushort)MsgId.SDevideItem, MakePacket<S_DevideItem>);
		_handler.Add((ushort)MsgId.SDevideItem, PacketHandler.S_DevideItemHandler);		
		_onRecv.Add((ushort)MsgId.SMoveItem, MakePacket<S_MoveItem>);
		_handler.Add((ushort)MsgId.SMoveItem, PacketHandler.S_MoveItemHandler);		
		_onRecv.Add((ushort)MsgId.SDeleteItem, MakePacket<S_DeleteItem>);
		_handler.Add((ushort)MsgId.SDeleteItem, PacketHandler.S_DeleteItemHandler);		
		_onRecv.Add((ushort)MsgId.SSearchItem, MakePacket<S_SearchItem>);
		_handler.Add((ushort)MsgId.SSearchItem, PacketHandler.S_SearchItemHandler);		
		_onRecv.Add((ushort)MsgId.SRaycastHit, MakePacket<S_RaycastHit>);
		_handler.Add((ushort)MsgId.SRaycastHit, PacketHandler.S_RaycastHitHandler);		
		_onRecv.Add((ushort)MsgId.SExitGame, MakePacket<S_ExitGame>);
		_handler.Add((ushort)MsgId.SExitGame, PacketHandler.S_ExitGameHandler);		
		_onRecv.Add((ushort)MsgId.SJoinServer, MakePacket<S_JoinServer>);
		_handler.Add((ushort)MsgId.SJoinServer, PacketHandler.S_JoinServerHandler);		
		_onRecv.Add((ushort)MsgId.SGameStart, MakePacket<S_GameStart>);
		_handler.Add((ushort)MsgId.SGameStart, PacketHandler.S_GameStartHandler);		
		_onRecv.Add((ushort)MsgId.SWaitingStatus, MakePacket<S_WaitingStatus>);
		_handler.Add((ushort)MsgId.SWaitingStatus, PacketHandler.S_WaitingStatusHandler);		
		_onRecv.Add((ushort)MsgId.SChangeAppearance, MakePacket<S_ChangeAppearance>);
		_handler.Add((ushort)MsgId.SChangeAppearance, PacketHandler.S_ChangeAppearanceHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;
		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
		
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
  
		if(CustomHandler != null)
        {
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}