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
				
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
		_handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.CLoadInventory, MakePacket<C_LoadInventory>);
		_handler.Add((ushort)MsgId.CLoadInventory, PacketHandler.C_LoadInventoryHandler);		
		_onRecv.Add((ushort)MsgId.CCloseInventory, MakePacket<C_CloseInventory>);
		_handler.Add((ushort)MsgId.CCloseInventory, PacketHandler.C_CloseInventoryHandler);		
		_onRecv.Add((ushort)MsgId.CMergeItem, MakePacket<C_MergeItem>);
		_handler.Add((ushort)MsgId.CMergeItem, PacketHandler.C_MergeItemHandler);		
		_onRecv.Add((ushort)MsgId.CDevideItem, MakePacket<C_DevideItem>);
		_handler.Add((ushort)MsgId.CDevideItem, PacketHandler.C_DevideItemHandler);		
		_onRecv.Add((ushort)MsgId.CMoveItem, MakePacket<C_MoveItem>);
		_handler.Add((ushort)MsgId.CMoveItem, PacketHandler.C_MoveItemHandler);		
		_onRecv.Add((ushort)MsgId.CDeleteItem, MakePacket<C_DeleteItem>);
		_handler.Add((ushort)MsgId.CDeleteItem, PacketHandler.C_DeleteItemHandler);		
		_onRecv.Add((ushort)MsgId.CSearchItem, MakePacket<C_SearchItem>);
		_handler.Add((ushort)MsgId.CSearchItem, PacketHandler.C_SearchItemHandler);		
		_onRecv.Add((ushort)MsgId.CRaycastShoot, MakePacket<C_RaycastShoot>);
		_handler.Add((ushort)MsgId.CRaycastShoot, PacketHandler.C_RaycastShootHandler);		
		_onRecv.Add((ushort)MsgId.CExitGame, MakePacket<C_ExitGame>);
		_handler.Add((ushort)MsgId.CExitGame, PacketHandler.C_ExitGameHandler);		
		_onRecv.Add((ushort)MsgId.CJoinServer, MakePacket<C_JoinServer>);
		_handler.Add((ushort)MsgId.CJoinServer, PacketHandler.C_JoinServerHandler);
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