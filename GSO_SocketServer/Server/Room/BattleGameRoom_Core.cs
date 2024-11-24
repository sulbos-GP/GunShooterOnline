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
using System.Linq;
using System.Diagnostics;
using Server.Game.Quest;
using WebCommonLibrary.Models.GameDatabase;
using Server.Game.Quest.Interfaces;
using Server.Game.Object;
using Google.Protobuf.WellKnownTypes;

namespace Server
{
    public partial class BattleGameRoom : GameRoom
    {
        //GameServer Onstart에서 받아옴
        public List<int> connectPlayer = new List<int>();

        Dictionary<int, Player> _playerDic = new Dictionary<int, Player>();
        Dictionary<int, CreatureObj> _enemyDic = new Dictionary<int, CreatureObj>();
        Dictionary<int, SkillObj> _skillObjDic = new Dictionary<int, SkillObj>();

        //public List<object> Escapes = new List<object>();
        public Dictionary<int, MatchOutcome> MatchInfo = new Dictionary<int, MatchOutcome>();

        private Stopwatch playTime = new Stopwatch();
        public bool IsGameStarted { get; protected set; } = false;
        public bool IsGameEnd { get; protected set; } = false;

        QuadTreeManager quadTreeManager = new QuadTreeManager();

        public Map map { get; private set; }
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

            playTime = new Stopwatch();
        }

        public override void LogicUpdate()
        {
            //물리 엔진 먼저
            quadTreeManager.Update();
            //방 처리
            Flush();
            //Update 처리
            ObjectManager.Instance.Update();




        }

        public override void Start()
        {

        }

        public override void Stop()
        {
            // 나머지 사람들 전부 쫓아 내기
            IsGameEnd = true;

            //프로그램 종료 하기
            //CheakGameDone();
        }

        public override void Clear()
        {
        }


        private bool CheakGameDone()
        {
            if (_playerDic.Values.Count == 0)
            {
                Stop();
                return true;
            }


            return false;
        }


        public override void BroadCast(IMessage message)
        {
            if((message  as S_Die )!= null)
            {
                Console.WriteLine("S_DIE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

            foreach (Player player in _playerDic.Values)
                player.Session.Send(message, DeliveryMethod.ReliableSequenced);

        }





        public override void EnterGame(object obj)
        {
            GameObject gameObject = (GameObject)obj;

            if (gameObject == null)
                return;

            playTime.Start();

            var type = ObjectManager.GetObjectTypeById(gameObject.Id);


            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
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

                   player.inventory = new Inventory(player);
                    foreach (PS_ItemInfo item in player.inventory.storage.GetItems(player.Id))
                    {
                        enterPacket.ItemInfos.Add(item);
                    }

                    player.Quest = new Quests(player);
                    foreach (IQuest quest in player.Quest.QuestList)
                    {
                        enterPacket.Quests.Add(quest.Packet);
                    }

                    player.Init();

                    player.inventory.storage.PrintInvenContents();

                    enterPacket.GameData = new GameDataInfo()
                    {
                        LeftTime = Program.minutes
                    };

                    player.Session.Send(enterPacket);

                    //player.Vision.Update();
                }


                if (IsGameStarted == true)  //나중에 다시 접속하는 player라면
                {
                    Console.WriteLine("=========== IsGameStarted ===========");

                    // 다른 플레이어 정보
                    var spawnPacket = new S_Spawn();

                    foreach (var p in _playerDic.Values)
                    {
                        if(player.Id == p.Id)
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
            else if (type == GameObjectType.Enemyai)
            {
                var enemy = gameObject as EnemyAI;
                _enemyDic.Add(gameObject.Id, enemy);
                enemy.gameRoom = this;

                //mMap.AddObject(monster);


              /*  S_AiSpawn info = new S_AiSpawn();
                info.DetectRange = enemy.detectionRange;
                info.ChaseRange = enemy.chaseRange;
                info.AttackRange = enemy.attackRange;
                info.SpawnZone = new Vector2IntInfo()
                {
                    X = (int)enemy.spawnPoint.X,
                    Y = (int)enemy.spawnPoint.Y
                };


                BroadCast(info);*/








                //enemy.Update();
            }
            else if (type == GameObjectType.Projectile) //&& type == GameObjectType.Scopeskill)
            {
                var skillObj = gameObject as SkillObj;
                _skillObjDic.Add(gameObject.Id, skillObj);
                skillObj.gameRoom = this;

                //mMap.AddObject(skillObj);
                //skillObj.Update();
            } //if끝




            if (IsGameStarted == true)   //나중에 다시 접속하는 player라면
            {
                var spawnpacket = new S_Spawn();
                spawnpacket.Objects.Add(gameObject.info);
                BroadCast(spawnpacket);


                CreatureObj creatureObj = gameObject as CreatureObj;
                if (creatureObj != null)
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


            foreach(Mine mine in map.mines)
            {
                spawnPacket.Objects.Add(mine.info);
            }


            foreach (EnemyAI enemy in ObjectManager.Instance.GetEnemyAIs())
            {
                spawnPacket.Objects.Add(enemy.info);

               
            }


            enterPlayer.Session.Send(spawnPacket);


            foreach (EnemyAI enemy in ObjectManager.Instance.GetEnemyAIs())
            {

                S_AiSpawn info = new S_AiSpawn();
                info.ObjectId = enemy.Id;
                info.DetectRange = enemy.detectionRange;
                info.ChaseRange = enemy.chaseRange;
                info.AttackRange = enemy.attackRange;
                info.SpawnZone = new Vector2IntInfo()
                {
                    X = (int)enemy.spawnPoint.X,
                    Y = (int)enemy.spawnPoint.Y
                };


                enterPlayer.Session.Send(info);
            }

         

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
            else if (type == GameObjectType.Enemyai)
            {
                bool t = _enemyDic.Remove(id);

                if (t == true)
                {
                    Console.WriteLine($"LeaveGame id : {id}");
                }

                if (MatchInfo.Remove(id) == false)
                {
                    Console.WriteLine("MatchInfo.Remove(id) == false");
                }
            }

            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.ObjcetIds.Add(id);
            BroadCast(despawnPacket);


            //CheakGameDone();


        }
        public Player GetPlayer(int id)
        {
            return _playerDic.TryGetValue(id, out var player) ? player : null;
        }

        public List<Player> GetPlayers()
        {
            return _playerDic.Values.ToList();
        }

        public void PostPlayerStats(int playerId)
        {
            Player player = GetPlayer(playerId);
            if (player == null)
            {
                return;
            }

            MatchInfo.TryGetValue(player.UID,  out MatchOutcome outcome);
            if(outcome == null)
            {
                return;
            }

            //파밍 계산
            {
                var curInventoryAndGear = player.inventory.GetInventoryObjectIds().Union(player.gear.GetPartObjectIds()).ToList();
                var oldInventoryAndGear = player.inventory.GetInitInventoryObjectIds().Union(player.gear.GetInitPartObjectIds()).ToList();
                outcome.farming = curInventoryAndGear.Except(oldInventoryAndGear).Count();
            }

            //생존 시간 계산
            {
                outcome.survival_time = (int)playTime.Elapsed.TotalMinutes;
            }

#if DOCKER
            Program.web.Lobby.PostPlayerStats(player.UID, outcome).Wait();
#endif
        }
    }
}
