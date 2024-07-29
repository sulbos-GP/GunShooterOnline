using Google.Protobuf;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public abstract class PacketSession : Session
	{
		public static readonly int HeaderSize = 2;

		// [size(2)][packetId(2)][ ... ][size(2)][packetId(2)][ ... ]
		public sealed override int OnRecv(ArraySegment<byte> buffer, byte channelNumber)
		{
			int processLen = 0;
			int packetCount = 0;

			while (true)
			{
				// 최소한 헤더는 파싱할 수 있는지 확인
				if (buffer.Count < HeaderSize)
					break;

				// 패킷이 완전체로 도착했는지 확인
				ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
				if (buffer.Count < dataSize)
					break;

				// 여기까지 왔으면 패킷 조립 가능
				OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize), channelNumber);
				packetCount++;

				processLen += dataSize;
				buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
			}

			if (packetCount > 1)
				Console.WriteLine($"패킷 모아보내기 : {packetCount}");

			return processLen;

		}

		public abstract void OnRecvPacket(ArraySegment<byte> buffer, byte channelNumber);
	}

	public abstract class Session
	{
		public SessionManager mSessionManager;
        public NetPeer mPeer;

        public int mPing;
        public int mId;
		public byte mGameRoomNumber;

		object _lock = new object();

		public abstract void OnConnected(EndPoint endPoint);
		public abstract int  OnRecv(ArraySegment<byte> buffer, byte channelNumber);
        public abstract void OnSend(int numOfBytes);
		public abstract void OnDisconnected(EndPoint endPoint);

		void Clear()
		{
			lock (_lock)
			{

			}
		}

		public void Init(NetPeer peer)
		{
            mPeer = peer;
		}
        
        public void Send(List<(ArraySegment<byte>, DeliveryMethod)> sendBuffList)
		{
			if (sendBuffList.Count == 0)
			{
                return;
            }

            if (mPeer.ConnectionState.Equals(ConnectionState.Disconnected))
            {
                return;
            }

            lock (_lock)
			{
				int sentByte = 0;
                foreach ((ArraySegment<byte>, DeliveryMethod) item in sendBuffList)
				{
					sentByte += item.Item1.Count;
                    mPeer.Send(item.Item1, item.Item2);
                }
				OnSend(sentByte);
            }
		}

		public void Send(ArraySegment<byte> sendBuff, DeliveryMethod deliveryMethod)
		{
			lock (_lock)
			{
                int sentByte = sendBuff.Count;
                mPeer.Send(sendBuff, deliveryMethod);
                OnSend(sentByte);
            }
		}

		public void Disconnect()
		{
			mPeer.Disconnect();

			OnDisconnected(mPeer);

			Clear();
		}

	}
}
