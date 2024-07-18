using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class BattleGameRoom : GameRoom
    {
        public Map mMap { get; } = new();

        private readonly Dictionary<int, Monster> _MonsterList = new();
        private readonly Dictionary<int, Player> _playerList = new();
        private readonly Dictionary<int, SkillObj> _skillObjList = new();


        public override void Init()
        {

        }

        public override void LogicUpdate()
        {
            Flush();
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }

        public override void Clear()
        {
        }

        public override void BroadCast(int id, IMessage message)
        {
            var _players = mMap.GetPlanetPlayers(id);
            if (_players == null || _players.Count <= 0)
                return;

            foreach (var player in _players) player.Session.Send(message);

        }
        
        public override void EnterGame(object obj, bool randomPos)
        {
            GameObject gameObject = (GameObject)obj;

            if (gameObject == null) 
                return;

            var type = ObjectManager.GetObjectTypeById(gameObject.Id);


            if (type == GameObjectType.Player)
            {
                var player = gameObject as Player;
                _playerList.Add(gameObject.Id, player);
                player.gameRoom = this;

                //player.RefreshAddtionalStat();

                //TODO : 삭제
                if (player.Hp <= 0)
                    player.OnDead(player);


                //GetZone(player.CellPos).Players.Add(player);
                //for (int i = 0; i < 5; i++)
                //{
                //    if (Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y)) == true)
                //        break;
                //}

                if (mMap.SetPosAndRoomtsId(player) == false) 
                    Console.WriteLine("맵 스폰 오류");

                mMap.AddObject(player);


                //본인에게 정보 전송
                {
                    var enterPacket = new S_EnterGame();
                    enterPacket.Player = player.info;
                    player.Session.Send(enterPacket);

                    player.Vision.Update();

                    //--------------------------------------------
                    mMap.SendMapInfo(player);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                var monster = gameObject as Monster;
                _MonsterList.Add(gameObject.Id, monster);
                monster.gameRoom = this;

                mMap.AddObject(monster);
                monster.Update();
            }
            else if (type == GameObjectType.Projectile && type == GameObjectType.Scopeskill)
            {
                var skillObj = gameObject as SkillObj;
                _skillObjList.Add(gameObject.Id, skillObj);
                skillObj.gameRoom = this;

                mMap.AddObject(skillObj);
                skillObj.Update();
            } //if끝

            {
                var spawnpacket = new S_Spawn();
                spawnpacket.Objects.Add(gameObject.info);
                BroadCast(gameObject.CurrentRoomId, spawnpacket);

                var ChangePacket = new S_ChangeHp();
                ChangePacket.ObjectId = gameObject.Id;
                ChangePacket.Hp = gameObject.Hp;
                BroadCast(gameObject.CurrentRoomId, ChangePacket);
            }
        }


        public override void LeaveGame(int id)
        {
            
        }

        
    }
}
