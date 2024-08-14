using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ServerCore
{
	public class Listener : INetEventListener
    {
        private ServerNetworkService mNetworkService;

        public Listener(ServerNetworkService serverNetworkService)
        {
            mNetworkService = serverNetworkService;
        }

        //외부에서 호출
        public void OnPeerConnected(NetPeer peer)
        {
            Session session = mNetworkService.mSessionFactory.Invoke();
            session.Init(peer);
            session.OnConnected(peer);

            peer.Tag = session;

            mNetworkService.mSessionManager.Insert(session);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Disconnection: {0}, Reason : {1}, Error : {2}", peer, disconnectInfo.Reason.ToString(), disconnectInfo.SocketErrorCode.ToString());
            
            var session = (Session)peer.Tag;
            if (session != null)
            {
                session.OnDisconnected(peer);

                mNetworkService.mSessionManager.Remove(session);
            }

        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine($"[{endPoint}] NetworkError: " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            if (false == mNetworkService.mUseChannel)
            {
                channelNumber = 0;
            }

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

            if(mNetworkService.mManager.PoolCount >= mNetworkService.mBackLog)
            {
                return;
            }

            if (mNetworkService.mSessionManager.GetSessionCount() >= mNetworkService.mRegister)
            {
                return;
            }

       
            
            NetPeer peer = request.AcceptIfKey(mNetworkService.mAcceptKey);
            if(peer.Tag == null)
            {
                request.Reject();
            }
        }

    }
}
