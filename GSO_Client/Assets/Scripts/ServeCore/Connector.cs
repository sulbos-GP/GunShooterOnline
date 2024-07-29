using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ServerCore
{
    internal class Connector : INetEventListener
    {
        private ClientNetworkService    mNetworkService;

        public Connector(ClientNetworkService clientNetworkService)
        {
            mNetworkService = clientNetworkService;
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Session session = mNetworkService.mSessionFactory.Invoke();
            session.Init(peer);
            session.OnConnected(peer);

            peer.Tag = session;

            //LogicTimer.Start();
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Disconnection: {0}, Reason : {1}, Error : {2}", peer, disconnectInfo.Reason.ToString(), disconnectInfo.SocketErrorCode.ToString());
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine($"[{endPoint}] NetworkError: " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            ArraySegment<byte> packet = reader.GetBytesSegment(reader.AvailableBytes);
            var session = (Session)peer.Tag;
            if (session != null)
            {
                session.OnRecv(packet, channelNumber);
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            var session = (Session)peer.Tag;
            if (session != null)
            {
                session.mPing = latency;
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            //클라이언트 사용 X
        }
    }
}
