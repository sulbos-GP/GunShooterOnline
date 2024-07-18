using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using LiteNetLib.Utils;
using LiteNetLib;
using System.Reflection.PortableExecutable;

namespace Server
{
    public class ClientSession : PacketSession
	{
		public int SessionId { get; set; }
		public BattleGameRoom Room { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected  123: {endPoint}");
            //Program.Room.Push(() => Program.Room.Enter(this));
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer, byte channelNumber)
		{
            PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			//SessionManager.Instance.Remove(this);
			//if (Room != null)
			//{
			//	GameRoom room = Room;
			//	room.Push(() => room.Leave(this));
			//	Room = null;
			//}

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
