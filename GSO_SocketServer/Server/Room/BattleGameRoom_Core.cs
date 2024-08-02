using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using QuadTree;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public partial class BattleGameRoom : GameRoom
    {
        Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();
        Dictionary<int, CreatureObj> _monsterDic = new Dictionary<int, CreatureObj>();
        Dictionary<int, SkillObj> _skillObjDic = new Dictionary<int, SkillObj>();


        public Map mMap { get; } 
        public BattleGameRoom()
        {
            mMap = new Map(r: this);
            mMap.Init();
        }

        public override void Init()
        {

        }

        public override void LogicUpdate()
        {
            Flush();


            QuadTreeManager quadTreeManager = new QuadTreeManager();
            quadTreeManager.Update();




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
            foreach (Player player in _playerDic.Values) 
                player.Session.Send(message,DeliveryMethod.ReliableSequenced);

        }
        
        public override void EnterGame(object obj)
        {
            GameObject gameObject = (GameObject)obj;

            if (gameObject == null) 
                return;

            var type = ObjectManager.GetObjectTypeById(gameObject.Id);


            if (type == GameObjectType.Player)
            {
                var player = gameObject as Player;
                _playerDic.Add(gameObject.Id, player);
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
/*
                if (mMap.SetPosAndRoomtsId(player) == false) 
                    Console.WriteLine("맵 스폰 오류");*/

                //mMap.AddObject(player);


                //본인에게 정보 전송
                {
                    var enterPacket = new S_EnterGame();
                    enterPacket.Player = player.info;
                    player.Session.Send(enterPacket);

                    //player.Vision.Update();

                    //--------------------------------------------
                   // mMap.SendMapInfo(player);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                var monster = gameObject as Monster;
                _monsterDic.Add(gameObject.Id, monster);
                monster.gameRoom = this;

                //mMap.AddObject(monster);
                monster.Update();
            }
            else if (type == GameObjectType.Projectile) //&& type == GameObjectType.Scopeskill)
            {
                var skillObj = gameObject as SkillObj;
                _skillObjDic.Add(gameObject.Id, skillObj);
                skillObj.gameRoom = this;

                //mMap.AddObject(skillObj);
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
        
        public Player GetPlayer(int id)
        {
            return _playerDic.TryGetValue(id , out var player) ? player : null;
        }
    }
}
