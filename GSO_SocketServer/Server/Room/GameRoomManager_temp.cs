using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public class GameRoomManager_temp : JobSerializer
    {
        private int _roomId = 1;
        private readonly Dictionary<int, GameRoom> _rooms = new();

        public void Update()
        {
            Flush();

            //TODO : Thread Pool로 시간 맞춰서 돌리기
            foreach (var gameroom in _rooms.Values)
                gameroom.LogicUpdate();
        }


        public GameRoom Add(int mapId)
        {
            var gameRoom = new BattleGameRoom();
            gameRoom.Push(gameRoom.Init);

            gameRoom.RoomId = _roomId;
            _rooms.Add(_roomId, gameRoom);
            _roomId++;
            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            return _rooms.Remove(roomId);
        }

        public GameRoom Find(int roomId)
        {
            GameRoom room = null;
            if (_rooms.TryGetValue(roomId, out room))
                return room;
            return null;
        }
    }
}
