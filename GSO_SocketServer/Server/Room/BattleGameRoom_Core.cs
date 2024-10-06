using Google.Protobuf;
using Google.Protobuf.Protocol;
using LiteNetLib;
using QuadTree;
using Server.Game;
using Server.Game.Object.Item;
using ServerCore;
using System;
using System.Collections.Generic;
using Server.Game.Object.Gear;
using System.Net.Sockets;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.Match;
using System.Numerics;

namespace Server
{
    public partial class BattleGameRoom : GameRoom
    {
        //GameServer Onstart에서 받아옴
        public List<int> connectPlayer = new List<int>();

        Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();
        Dictionary<int, CreatureObj> _monsterDic = new Dictionary<int, CreatureObj>();
        Dictionary<int, SkillObj> _skillObjDic = new Dictionary<int, SkillObj>();

        //public List<object> Escapes = new List<object>();
        public Dictionary<int, MatchOutcome> MatchInfo = new Dictionary<int, MatchOutcome>();

        public bool IsGameStarted { get; protected set; } = false;


        public Map map { get; set; }
        public BattleGameRoom()
        {

        }

        /// <summary>
        /// 미리 배치해야하는 오브젝트를 여기에서 초기화
        /// </summary>
        public override void Init()
        {
            map = new Map(r: this);
            map.Init();
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

        public override void BroadCast(IMessage message)
        {
            foreach (Player player in _playerDic.Values)
                player.Session.Send(message, DeliveryMethod.ReliableSequenced);

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




                {   //본인에게 본인 데이터 정보 전송
                    var enterPacket = new S_EnterGame();
                    enterPacket.Player = player.info;

                    if (MatchInfo.TryAdd(player.UID, new MatchOutcome()) == false)
                    {
                        Console.WriteLine("player.UID add same");
                    }



                    player.gear = new Gear(player);
                    foreach (PS_GearInfo item in player.gear.GetPartItems(player.Id))
                    {
                        enterPacket.GearInfos.Add(item);
                    }

                    player.inventory = new Inventory(player, player.UID);
                    foreach (PS_ItemInfo item in player.inventory.storage.GetItems(player.Id))
                    {
                        enterPacket.ItemInfos.Add(item);
                    }


                    player.inventory.storage.PrintInvenContents();
                    player.Session.Send(enterPacket);

                    //player.Vision.Update();
                }


                if (IsGameStarted == true)  //나중에 다시 접속하는 player라면
                {
                    

                    // 다른 플레이어 정보
                    var spawnPacket = new S_Spawn();

                    foreach (var p in _playerDic.Values)
                    {
                        spawnPacket.Objects.Add(p.info);
                    }


                    player.Session.Send(spawnPacket);

                    

                    //--------------------------------------------
                    // mMap.SendMapInfo(player);
                }

                //inventory
                NewEnterSpawnData(player);

                if (_playerDic.TryAdd(gameObject.Id, player))
                {
                    // 시작 때 같이 들어감

                }
                else
                {
                    //중간 난입
                    Console.WriteLine($"Reconnected Game Id{gameObject.Id}");
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
                BroadCast(spawnpacket);

                CreatureObj creatureObj = gameObject as CreatureObj;
                if(creatureObj != null)
                {
                    var ChangePacket = new S_ChangeHp();
                    ChangePacket.ObjectId = creatureObj.Id;
                    ChangePacket.Hp = creatureObj.Hp;
                    BroadCast(ChangePacket);
                }
               
            }
        }


        private void NewEnterSpawnData(Player enterPlayer)
        {
            S_Spawn spawnPacket = new S_Spawn();

            //생성끝
            foreach (BoxObject box in map.rootableObjects)
            {
                Console.WriteLine($"box id : {box.Id}, x = {box.PosInfo.PosX} , y=  {box.PosInfo.PosY}");

                int size = box.info.CalculateSize();
                long x = box.info.Box.X;
                spawnPacket.Objects.Add(box.info);
            }



            foreach (ExitZone exit in map.exitZones)
            {
                //Console.WriteLine($"exit id : {exit.Id}");

                spawnPacket.Objects.Add(exit.info);
            }


            enterPlayer.Session.Send(spawnPacket);
        }


        public override void LeaveGame(int id)
        {

            Console.WriteLine("LeaveGame");
            var type = ObjectManager.GetObjectTypeById(id);


            if (type == GameObjectType.Player)
            {

                bool t =  _playerDic.Remove(id);

                if(t == true)
                {
                    Console.WriteLine($"LeaveGame id : {id}");
                }

                if (MatchInfo.Remove(id) == false)
                {
                    Console.WriteLine("MatchInfo.Remove(id) == false");
                }
               

            }
            else if (type == GameObjectType.Monster)
            {


            }

            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjcetIds.Add(id);
            BroadCast(despawnPacket);


        }
        public Player GetPlayer(int id)
        {
            return _playerDic.TryGetValue(id, out var player) ? player : null;
        }
    }
}
