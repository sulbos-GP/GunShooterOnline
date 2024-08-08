using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;
using LiteNetLib;

namespace DummyClient
{
	class ServerSession : PacketSession
	{
		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

           //C2S_Chat chat = new C2S_Chat();
           //chat.chat = "Hello";
           //this.Send(chat.Write(), DeliveryMethod.ReliableOrdered);
        }

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer, byte channelNumber)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}

		public override void FlushSend()
		{

		}
	}
}
