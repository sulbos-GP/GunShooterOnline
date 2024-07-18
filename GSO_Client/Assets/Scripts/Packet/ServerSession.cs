using System;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using UnityEngine;

public class ServerSession : PacketSession
{
    public void Send(IMessage packet)
    {
        var msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        var msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

        var size = (ushort)packet.CalculateSize();
        var sendbuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendbuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendbuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendbuffer, 4, size);
        Send(new ArraySegment<byte>(sendbuffer));
    }


    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected : {endPoint}");
        PacketManager.Instance.CustomHandler = (s, m, i) => { PacketQueue.Instance.Push(i, m); };
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log("OnDisconnected");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}