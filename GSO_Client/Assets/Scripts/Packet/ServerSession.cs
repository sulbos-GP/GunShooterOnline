using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using ServerCore;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class ServerSession : PacketSession
{

    private readonly object _lock = new();
    private long _lastSendTick;

    //패킷 모아보내기
    private int _reservedSendBytes;
    private List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)> _reserveQueue = new();

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
    public void FlushSend()
    {
        List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)> sendList = null;
        lock (_lock)
        {
            if ( _reservedSendBytes < 1500)
                return;

            _reservedSendBytes = 0;

            sendList = _reserveQueue;
            _reserveQueue = new List<(ArraySegment<byte> Segment, DeliveryMethod reliableSequenced)>();
        }

        base.Send(sendList);
    }


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
}
