using System.Collections.Generic;
using Google.Protobuf;

public class PacketMessage
{
    public ushort Id { get; set; }
    public IMessage Message { get; set; }
}

public class PacketQueue
{
    private readonly object _lock = new();


    private readonly Queue<PacketMessage> _packetQueue = new();
    public static PacketQueue Instance { get; } = new();


    public void Push(ushort id, IMessage message)
    {
        var packetMessage = new PacketMessage { Id = id, Message = message };
        lock (_lock)
        {
            _packetQueue.Enqueue(packetMessage);
        }
    }

    public PacketMessage Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0)
                return null;

            return _packetQueue.Dequeue();
        }
    }

    public List<PacketMessage> PopAll() //서버 페킷 받는 위치
    {
        var list = new List<PacketMessage>();

        lock (_lock)
        {
            while (_packetQueue.Count > 0)
                list.Add(_packetQueue.Dequeue());
        }

        return list;
    }
}