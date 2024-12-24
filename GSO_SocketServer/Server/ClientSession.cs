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
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public BattleGameRoom Room { get; set; }


        private readonly object _lock = new();
        private long _lastSendTick;

        //패킷 모아보내기
        private int _reservedSendBytes;
        private List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)> _reserveQueue = new();
        public Player MyPlayer { get; set; }

        public void Send(IMessage packet, DeliveryMethod reliableSequenced = DeliveryMethod.ReliableOrdered)
        {
            var msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            var msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            var size = (ushort)packet.CalculateSize();
            var sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, sizeof(ushort), sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 2 * sizeof(ushort), size);

            lock (_lock)
            {
                _reserveQueue.Add((sendBuffer, reliableSequenced));
                _reservedSendBytes += sendBuffer.Length;
            }
        }

        //실제 network IO 처리하는 부분
        public override void FlushSend()
        {
            List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)> sendList = null;
            lock (_lock)
            {
                var delta = Environment.TickCount64 - _lastSendTick;
                if (delta < 50 && _reservedSendBytes < 1500)
                    return;

                _lastSendTick = Environment.TickCount64;
                _reservedSendBytes = 0;

                sendList = _reserveQueue;
                _reserveQueue = new List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)>();
            }

            base.Send(sendList);
        }




        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected  123: {endPoint}");
          
            /* 
             * LastTick = LogicTimer.Tick;
             * S_Ping s_Ping = new S_Ping();
              s_Ping.IsEnd = false;
              s_Ping.Tick = LogicTimer.Tick;

              Send(s_Ping);*/
            //Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer, byte channelNumber)
        {
            //Console.WriteLine($"OnRecvPacket  buffer: {buffer} ,channelNumber {channelNumber} ");

            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            if (Room != null)
            {
            	GameRoom room = Room;

                if(MyPlayer == null)
                {

                    Console.WriteLine("MyPlayer is null");
                }

            	room.Push(() => room.LeaveGame(MyPlayer.Id));
            	Room = null;
            }
            mSessionManager.Remove(this);



            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }


    }
}
