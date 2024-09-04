using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Win32;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class NetworkService
    {
        public NetManager       mManager;
        public LogicTimer       mGameLogicTimer;
        public Thread           mCoreWorkThread;

        public IPEndPoint       mEndPoint;
        public Func<Session>    mSessionFactory;
        public string           mAcceptKey;

        public NetworkService()
        {
            mCoreWorkThread = new Thread(OnCoreWorkLoop);

        }

        protected abstract bool OnStart();
        protected abstract bool OnStop();

        public void Start()
        {
            if (true == mManager.IsRunning)
            {
                return;
            }

            if (false == OnStart())
            {
                return;
            }

            mCoreWorkThread.Start();
            mGameLogicTimer.Start();
        }

        public void Stop()
        {

            if (false == mManager.IsRunning)
            {
                return;
            }

            if (false == OnStop())
            {
                return;
            }

            if (mManager != null)
            {
                mManager.Stop(true);
            }

            if(mGameLogicTimer != null)
            {
                mGameLogicTimer.Stop();
            }

        }

        public void OnCoreWorkLoop()
        {
            while (mManager.IsRunning)
            {
                mManager.PollEvents();
                mGameLogicTimer.Update();
                
            }

            
        }

    }

    public class ServerNetworkService : NetworkService
    {
        public Listener        mListener;
        public SessionManager  mSessionManager;

        public int  mRegister = 100;
        public int  mBackLog = 100;

        public bool mUseChannel = false;
        public GameRoom gameRoom;
        public int mMaxChannelNumber =  1;

        public ServerNetworkService()
        {
            mGameLogicTimer = new LogicTimer(OnLogicUpdate);
            mListener = new Listener(this);
            mManager = new NetManager(mListener);
            //mGameRoomManager = new GameRoomManager(this);
            mSessionManager = new SessionManager();


        }

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, string acceptKey, int register, int backLog)
        {
            mEndPoint       = endPoint;
            mSessionFactory = sessionFactory;
            mAcceptKey      = acceptKey;
            mRegister       = register;
            mBackLog        = backLog;
        } public void Init(IPEndPoint endPoint,  string acceptKey, int register, int backLog)
        {
            mEndPoint       = endPoint;
            mAcceptKey      = acceptKey;
            mRegister       = register;
            mBackLog        = backLog;
        }

        //채널(게임방) 설정
        public void SetChannel(bool use, GameRoom roomFactory, int number)
        {
            mUseChannel = use;
            gameRoom = roomFactory;
            mMaxChannelNumber = number;
        }

        protected override bool OnStart()
        {

            if(mUseChannel == true)
            {

            }

            mManager.DisconnectTimeout = 100000;
            mManager.SimulateLatency = true;


            if (false == mManager.Start(mEndPoint.Port))
            {
                return false;
            }


            return true;
        }

        protected override bool OnStop()
        {
            return true;
        }

        public void OnLogicUpdate()
        {
            if (mManager.IsRunning)
            {
                mSessionManager.FlushSend();
                gameRoom.LogicUpdate();
                //QuadTreeManmager Update()
            }
        }
    }

    public class ClientNetworkService : NetworkService
    {
        private Connector   mConnector;

        public ClientNetworkService()
        {
            mConnector = new Connector(this);
            mManager = new NetManager(mConnector);
        }

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, string acceptKey)
        {
            mEndPoint = endPoint;
            mSessionFactory = sessionFactory;
            mAcceptKey = acceptKey;
        }

        protected override bool OnStart()
        {
            if(false == mManager.Start())
            {
                return false;
            }

            NetPeer peer = mManager.Connect(mEndPoint, mAcceptKey);
            if (peer == null)
            {
                return false;
            }

            return true;
        }

        protected override bool OnStop()
        {
            return true;
        }
    }
}
