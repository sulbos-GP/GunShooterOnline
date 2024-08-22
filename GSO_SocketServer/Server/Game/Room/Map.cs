using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Google.Protobuf.Protocol;
using QuadTree;
using Server.Data;
using Server.Game;
using ServerCore;
using Server.Game.Object.Item;

namespace Server.Game;

public struct Pos
{
    public int X;
    public int Y;

    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Pos lhs, Pos rhs)
    {
        return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public static bool operator !=(Pos lhs, Pos rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object obj)
    {
        return (Pos)obj == this;
    }

    public override int GetHashCode()
    {
        long val = (Y << 32) | X;
        return val.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}


public class Map
{
    private int[,] _collisions;
    //private GameObject[,] _objects;

    public int Width;
    public int Height;

    private int roomSize;


    public Vector2Int Bleft { get; private set; }
    public Vector2Int Tright { get; private set; }

    BattleGameRoom battleRoom;


    #region rootableObjects
    public readonly List<BoxObject> rootableObjects = new List<BoxObject>();
    #endregion

    #region EscapeObj
    //List<RootableObject> _rootableObjects = new List<RootableObject>();
    public readonly List<ExitZone> exitZones = new List<ExitZone>();

    #endregion

    public Map(BattleGameRoom r)
    {
        battleRoom = r;
    }

    public void Init()
    {
        loadMap("Forest");
    }

    #region item
    //TODO : 나중에 아이템 메니져로?

    private void SetRandomItem()
    {
        /*
        var db = ItemDB.items.Values.ToArray(); 

        //ItemObject item = ObjectManager.Instance.Add<ItemObject>();
        ItemObject item = new ItemObject();

        item.itemDataInfo = db[new Random().Next(0, db.Length)];

        foreach (var r in _rootableObjects)
        {
            r.inventory.instantGrid[0].p
        }*/
    }

    #endregion



    public void loadMap(string mapName, string pathPrefix = "")
    {
        //TODO : 파일 위치 josn으로 관리하기!! 240813 지승현
        //var Distance = 22;

        //----------------------------------------
        mapName = "Forest";

        // Collision 관련 파일
#if DOCKER
        var text = File.ReadAllText("/app/MapData/" + $"{mapName}.txt");
#else
        var text = File.ReadAllText(" ./../../../../../MapData/" + $"{mapName}.txt");
#endif


        var reader = new StringReader(text);

        var _ = reader.ReadLine();

        var minIndex = reader.ReadLine().Split('/');


        Bleft = new Vector2Int(int.Parse(minIndex[0]), int.Parse(minIndex[1]));
        roomSize = int.Parse(reader.ReadLine());

        Width = roomSize;
        Height = roomSize;

        Tright = new Vector2Int(Bleft.x + (roomSize - 1), Bleft.y + (roomSize - 1));


        _collisions = new int[roomSize, roomSize];
       // _objects = new GameObject[roomSize, roomSize];


        //for (var x = roomSize - 1; x >= 0; x--)
        for (var x = 0; x < roomSize; x++)
            Buffer.BlockCopy(
                Array.ConvertAll(reader.ReadLine().Split(','), s => int.Parse(s)),
                0, _collisions, x * roomSize * sizeof(int), roomSize * sizeof(int));

        //동적 생성
        for (int i = 0; i < roomSize; i++)
        {

            for (int j = 0; j < roomSize; j++)
            {

                if (_collisions[i,j]  == 2) // 박스
                {
                    BoxObject rb = ObjectManager.Instance.Add<BoxObject>();
                    rb.CellPos = new Vector2(i + Bleft.x, j + Bleft.y);
                    rb.Init();
                    rb.info.Name = "Box";
                    rootableObjects.Add(rb);
                }
                else if (_collisions[i, j] == 3) //탈출구
                {
                    ExitZone exit = ObjectManager.Instance.Add<ExitZone>();
                    exit.CellPos = new Vector2(i + Bleft.x, j + Bleft.y);
                    exit.info.Name = "ExitZone";
                    exitZones.Add(exit);
                }


            }
        }

    

        Console.WriteLine("InitMap End"+ $"rootableObjects Count : {rootableObjects.Count}  , exitZones count : {exitZones.Count}");



        //         while (true)
        //         {
        //	int x = int.Parse(Console.ReadLine());
        //	int y = int.Parse(Console.ReadLine());
        //	bool k = CanGo(new Vector2Int(x, y), false);
        //             Console.WriteLine(k);
        //}
    }

    public void UpdateMap()
    {
        //속도가 느릴것 같음 쓰레드 새로 생성하는 느낌으로 가자!!
        /*  foreach (Room room in Rooms)
        {
            room.quadTreeManager.Insert(room.Players,room.Objects);
            room.quadTreeManager.Update();
        }*/
    }
    
    
    public bool ApplyLeave(GameObject gameObject)
    {
        if (gameObject.gameRoom == null)
            return false;
        /*if (gameObject.gameRoom.Map != this)
            return false;*/

        var posInfo = gameObject.PosInfo;
        if (posInfo.PosX < Bleft.x || posInfo.PosX > Tright.x)
            return false;
        if (posInfo.PosY < Bleft.y || posInfo.PosY > Tright.y)
            return false;

        {
            var pos = Cell2Pos(posInfo.PosX, posInfo.PosY);
            //if (_objects[pos.X, pos.Y] == gameObject)
            //    _objects[pos.X, pos.Y] = null;
        }

        return true;
    }

    public bool ApplyMove(GameObject gameObject, Vector2Int dest, bool cheakObjects = true, bool collision = true)
    {
        ApplyLeave(gameObject);

        if (gameObject.gameRoom == null)
        {
            Console.WriteLine($"{dest.x}, {dest.y} 이동불가능");
            return false;
        }

        var posInfo = gameObject.PosInfo;

        /*if (gameObject.gameRoom.Map != this)
            return false;*/

        if (CanGo(dest, false))
        {
            // 이동 성공
            return true;
        }


        {
            //var pos = Cell2Pos(posInfo.PosX, posInfo.PosY);
            //_objects[pos.X, pos.Y] = gameObject;
        }

       

        return false;
    }

    public bool CanGo(Vector2Int cellPos, bool cheakObjects = true)
    {
        if (cellPos.x < Bleft.x || cellPos.x > Tright.x)
            return false;
        if (cellPos.y < Bleft.y || cellPos.y > Tright.y)
            return false;

        var x = cellPos.x - Bleft.x;
        var y = cellPos.y - Bleft.y;

        //Console.WriteLine($"{cellPos.x}, {cellPos.y}and {x}, {y}");

        return _collisions[x, y] == 0; // && (!cheakObjects || _objects[x, y] == null)
    }

    public void SetMonster(GameRoom room, int monsterCount)
    {
        var CanSpwanRandomMonsterArr = DataManager.MonsterDict.Keys.Where(i => i < 50).ToArray();

        /*foreach (var r in Rooms)
        {
            if (r.RoomType == RoomType.PATH)
                continue;

            for (var i = 0; i < monsterCount; i++)
            {
                var rand = new Random();

                var rint = rand.Next(0, CanSpwanRandomMonsterArr.Count());
                var monster = ObjectManager.Instance.Add<Monster>();
                {
                    monster.CurrentRoomId = r.Id;
                    monster.Init(CanSpwanRandomMonsterArr[rint]); //side , posinfo

                    var Round = 10; //temp 나중에 받아와야함

                    monster.PosInfo.PosX = r.PosX + rand.Next(-Round, Round);
                    monster.PosInfo.PosY = r.PosY + rand.Next(-Round, Round);
                }

                room.Push(room.EnterGame, monster, false);
            } //갯수마다
        } //	방마다*/

        /*foreach (Room p in Rooms)
        {
            for (int i = 0; i < monsterCount; i++)
            {
                //나중에는 맵의 불,물,지옥 지형에 따라 몬스터 class로 이런식으로 렌덤값 추출
                Random rand = new Random();
                int r = rand.Next(1, DataManager.MonsterDict.Count + 1);
                Monster monster = ObjectManager.Instance.Add<Monster>();
                {
                    monster.CurrentPlanetId = p.Id;
                    monster.Init(r); //side , posinfo

                    int t = monster.Side;
                    float small = 0.3f;
                    if (t == 0)
                    {
                        monster.PosInfo.PosX = p.PosX + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
                        monster.PosInfo.PosY = p.PosY + p.Round / 2 + small;
                    }
                    else if (t == 1)
                    {
                        monster.PosInfo.PosX = p.PosX + p.Round / 2 + small;
                        monster.PosInfo.PosY = p.PosY + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
                    }
                    else if (t == 2)
                    {
                        monster.PosInfo.PosX = p.PosX + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
                        monster.PosInfo.PosY = p.PosY - p.Round / 2 - small;
                    }
                    else if (t == 3)
                    {
                        monster.PosInfo.PosX = p.PosX - p.Round / 2 - small;
                        monster.PosInfo.PosY = p.PosY + rand.Next(-(p.Round / 2 - 1), p.Round / 2 - 1);
                    }
                    else
                    {
                        Console.WriteLine("GetPlanetRotationById 오류");
                    }


                };


                room.Push(room.EnterGame, monster, false);
                //p.Objects.Add(monster);

            }

        }*/
    }


    public void ApplyProjectile(Vector2 pos, Vector2 dir)
    {
        //dir - pos
    }

   /* public void SendMapInfo(Player p)
    {
        var roomPacket = new S_RoomInfo();
        foreach (var room in Rooms)
        {
            var roomInfo = new RoomInfo();
            roomInfo.RoomId = room.Id;
            roomInfo.RoomLevel = room.RoomLevel;
            roomInfo.RoomType = (int)room.RoomType;
            roomInfo.PosX = room.PosX;
            roomInfo.PosY = room.PosY;
            roomPacket.RoomInfos.Add(roomInfo);
        }

        p.Session.Send(roomPacket);
    }*/


    #region A* PathFinding

    // U D L R
    private readonly int[] _deltaY = { 1, -1, 0, 0 };
    private readonly int[] _deltaX = { 0, 0, -1, 1 };
    private int[] _cost = { 10, 10, 10, 10 };

    public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool checkObjects = true)
    {
        var path = new List<Pos>();
        var closeList = new HashSet<Pos>(); // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        var openList = new Dictionary<Pos, int>(); // OpenList
        var parent = new Dictionary<Pos, Pos>();

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        var pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        var pos = Cell2Pos(startCellPos);
        var dest = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        openList.Add(pos, 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));

        pq.Push(new PQNode
            { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent.Add(pos, pos);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            var pqNode = pq.Pop();
            var node = new Pos(pqNode.X, pqNode.Y);
            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closeList.Contains(node))
                continue;

            // 방문한다
            closeList.Add(node);

            // 목적지 도착했으면 바로 종료
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (var i = 0; i < _deltaY.Length; i++)
            {
                var next = new Pos(node.X + _deltaX[i], node.Y + _deltaY[i]);

                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (next.Y != dest.Y || next.X != dest.X)
                    if (CanGo(Pos2Cell(next), checkObjects) == false) // CellPos
                        continue;

                // 이미 방문한 곳이면 스킵
                if (closeList.Contains(next))
                    continue;

                // 비용 계산
                var g = 0; // node.G + _cost[i];
                var h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵

                var value = 0;
                if (openList.TryGetValue(next, out value) == false)
                    value = int.MaxValue;

                if (value < g + h)
                    continue;

                // 예약 진행
                if (openList.TryAdd(next, g + h) == false)
                    openList[next] = g + h;

                pq.Push(new PQNode { F = g + h, G = g, Y = next.Y, X = next.X });

                if (parent.TryAdd(next, node) == false)
                    parent[next] = node;
            }
        }


        if (parent.Count() <= 1)
            return null;

        return CalcCellPathFromParent(parent, dest);
    }

    private List<Vector2Int> CalcCellPathFromParent(Dictionary<Pos, Pos> parent, Pos dest)
    {
        var cells = new List<Vector2Int>();

        var pos = dest;
        while (parent[pos] != pos)
        {
            cells.Add(Pos2Cell(pos));
            var t = parent[pos];
            //Console.WriteLine($"{t.X},{t.Y}");
            pos = parent[pos];
        }

        cells.Add(Pos2Cell(pos));
        cells.Reverse();

        return cells;
    }

    private Pos Cell2Pos(Vector2Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(cell.x - Bleft.x, cell.y - Bleft.y);
    }

    private Pos Cell2Pos(float x, float y)
    {
        // CellPos -> ArrayPos
        return new Pos((int)MathF.Round(x) - Bleft.x, (int)MathF.Round(y) - Bleft.y);
    }

    private Vector2Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector2Int(Bleft.x + pos.X, Bleft.y + pos.Y);
    }

    #endregion
}