using Google.Protobuf;
using LiteNetLib;
using System;
using System.Collections.Generic;



namespace ServerCore
{
    public abstract class GameRoom : JobSerializer
    {
        //public GameRoomManager mGameRoomManager;
        public byte mGameRoomNumber;

        public int RoomId { get; set; }



        List<Session> mSessions = new List<Session>();
        //JobQueue mJobQueue = new JobQueue();
        List<ArraySegment<byte>> mPendingList = new List<ArraySegment<byte>>();

        public abstract void Init();
        public abstract void LogicUpdate(); // 상속받은 class에서 Job처리
        public abstract void Start();
        public abstract void Stop();
        public abstract void Clear();


        public abstract void BroadCast(IMessage message);

        public abstract void EnterGame(object gameObject);

        public abstract void LeaveGame(int id);


       /* public void Push(Action job)
        {
            _jobQueue.Push(job);
        }*/

        /*public void Flush()
        {
            foreach (Session session in mSessions)
            {
                session.Send(mPendingList, DeliveryMethod.ReliableOrdered);
            }
            mPendingList.Clear();
        }*/

       /* public void Broadcast(ArraySegment<byte> segment)
        {
            mPendingList.Add(segment);
        }*/
    }
}
