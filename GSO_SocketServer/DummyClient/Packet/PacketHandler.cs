using DummyClient;
using LiteNetLib;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void S2C_ChatHandler(PacketSession session, IPacket packet)
    {
        /*S2C_Chat pkt = packet as S2C_Chat;
        ServerSession serverSession = session as ServerSession;

		Console.WriteLine("Client Recv : " + pkt.chat);

        C2S_Chat chat = new C2S_Chat();
		chat.chat = pkt.chat;
		serverSession.Send(chat.Write(), DeliveryMethod.ReliableOrdered);*/
    }

}
