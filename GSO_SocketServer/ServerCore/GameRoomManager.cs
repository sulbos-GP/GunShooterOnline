using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class GameRoomManager
    {
        private ServerNetworkService mNetworkService;
        Dictionary<byte, GameRoom> mGameRooms = new Dictionary<byte, GameRoom>();
        object _lock = new object();

        public GameRoomManager(ServerNetworkService serverNetworkService) 
        {
            mNetworkService = serverNetworkService;
        }

        public void CreateGameRooms()
        {
            for(int number = 1; number <= mNetworkService.mMaxChannelNumber; number++)
            {
                GameRoom room = mNetworkService.mGameRoomFactory.Invoke();
                room.mGameRoomNumber = Convert.ToByte(number);
                room.mGameRoomManager = this;
            }
        }

        public void Push(byte number, Action job)
        {
            GameRoom room = null;
            mGameRooms.TryGetValue(number, out room);
            if (room != null)
            {
                room.Push(job);
            }
        }

    }
}
