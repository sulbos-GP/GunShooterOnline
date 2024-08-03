#if UNITY_EDITOR

using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class WorldGenerator
{
    //static int Length = 140; //무조건 짝수

    static int HalfLength = 0;

    //public static List<Tuple<Transform, Vector2Int>> Rooms = new List<Tuple<Transform, Vector2Int>>();
    public static Tuple<Transform, Vector2Int> Room;
    static string MapStr = "";
    static string MapTempStr = "";
    static string serverPath = "";
    static string clinetPath = "";
    static int[,] map;
    static float distance; // -55.5 -55.5 -> -55 -55   = result = 0.5f
    static Vector2 startPosTopLeft = Vector2.zero;

    static Vector2 min = new Vector2(99999, 99999);


    [MenuItem("Tools/GenerateMap")]
    private static void GenerateMap()
    {

        //serverPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../GSO_SocketServer/Data"));
        clinetPath = Application.dataPath + "/Data";
        MakeMap();
    }


    public static void MakeMap()
    {

        // x좌표,y좌표,종류(1:스폰2:일반3:통로),방종류(1,2,3...)
        Debug.Log("시작");

        map = null;
        distance = 0;
        //MapStr = "";


        GameObject go = GameObject.FindGameObjectWithTag("World");
        //go.GetComponent<World>().InventorySet();




        Vector2 t = go.transform.Find("Pos").position;

        Room = new Tuple<Transform, Vector2Int>(go.transform, new Vector2Int(Mathf.RoundToInt(t.x), Mathf.RoundToInt(t.y)));
        //Vector2 t = tr.transform.Find("Pos").transform.Find("Center").position;

        // 튜플 추가


        /*    Rooms = Rooms.OrderBy(t => t.Item2.y)
                .ThenBy(t => t.Item2.x).ToList();*/
        // 7 8 9
        // 4 5 6
        // 1 2 3
        //순서 정리




        MapInit(Room);




        World world = Room.Item1.GetComponent<World>();
        if (world == null)
        {
            Debug.LogError("오류");
            return;
        }

        WriteMap(Room.Item1, Room.Item2);


        //MapStr += Environment.NewLine;


        //--------------  -------------- 끝 -------------- ---------------


        Debug.Log(min);

        string total =
             Environment.NewLine
             //+ MapStr
             + min.x + '/' + min.y + Environment.NewLine
             + map.GetLength(0) + Environment.NewLine
             + MapToString();

        Debug.Log($"끝{total.Length}");
        //File.WriteAllText(serverPath + $"Map_2.txt", total);
        File.WriteAllText(clinetPath + $"/Map_2.txt", total);

    }

    private void Init()
    {
        Debug.Log("InventorySet");
    }

    private static void MapInit(Tuple<Transform, Vector2Int> _room)
    {

        World world = _room.Item1.GetComponent<World>();
        if (world == null)
        {
            Debug.LogError("오류");
            return;
        }

        Tilemap _base = null;

        foreach (Transform _tr in _room.Item1)
        {
            if (_tr.name.Contains("base") == true)
            {
                _base = _tr.GetComponent<Tilemap>();
                break;
            }
        }  //------------------------ _base 찾기 ------------------------
        if (_base == null)
        {
            Debug.LogError("base 없음");
        }

        Vector2 tempVector = (Vector2)_base.CellToWorld(_base.cellBounds.position);       // ------------distance----------------

        distance = Math.Abs(Mathf.CeilToInt(tempVector.x)) - Math.Abs(tempVector.x) + 1;

        //Debug.Log(_room.Item2 + " ," + tempVector); //------------------------------ 디버그 ----------------------------

        if (tempVector.x < min.x)
        {
            min.x = Mathf.CeilToInt(tempVector.x);
            //Debug.Log(_room.Item2 + "min.x" + min.x);
        }
        if (tempVector.y < min.y)
        {
            min.y = Mathf.CeilToInt(tempVector.y);
            //Debug.Log(_room.Item2 + "min.y" + min.y);
        }


        if (map == null)         // ------------------------------ 맵 처음 실행시 ----------------------------------
        {

            HalfLength = Math.Abs(Mathf.RoundToInt(min.x));
            startPosTopLeft = new Vector2(-HalfLength, +HalfLength);
            map = new int[HalfLength * 2 + 1, HalfLength * 2 + 1]; //0도 포함

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = 0;
                }
            }
        }
        else
        {
            Debug.Log("오류");
        }


    }

    private static void WriteMap(Transform tr, Vector2Int pos)
    {

        Tilemap _base = null;
        Tilemap _wall = null;
        foreach (Transform _tr in tr)
        {
            if (_tr.name.Contains("base") == true)
            {
                _base = _tr.GetComponent<Tilemap>();
                continue;
            }
            else if (_tr.name.Contains("Wall") == true)
            {
                _wall = _tr.GetComponent<Tilemap>();
                continue;
            }
        }

        //------------------------ _base 찾기 ------------------------




        if (_base == null)
        {
            Debug.LogError("base 없음");
        }

        Vector2 _min = _base.CellToWorld(new Vector3Int(_base.cellBounds.xMin, _base.cellBounds.yMin));
        Vector2Int min = new Vector2Int(Mathf.RoundToInt(_min.x + distance) + HalfLength, Mathf.RoundToInt(_min.y + distance) + HalfLength);

        //Vector2 _max = _base.CellToWorld(new Vector3Int(_base.cellBounds.xMax, _base.cellBounds.yMax));
        //Vector2Int max = new Vector2Int(Mathf.RoundToInt(_max.x + distance) + HalfLength, Mathf.RoundToInt(_max.y + distance) + HalfLength);

        Vector2Int currnet = min;

        for (int y = _base.cellBounds.yMin; y <= _base.cellBounds.yMax; y++)
        //for (int y = _base.cellBounds.yMax - 1; y >= _base.cellBounds.yMin ; y--)
        {
            currnet.x = min.x;

            for (int x = _base.cellBounds.xMin; x <= _base.cellBounds.xMax; x++)
            {
                //Debug.Log(currnet);

                TileBase tile = _wall.GetTile(new Vector3Int(x, y, 0));

                if (tile != null)
                {


                    //Debug.Log($"x = {currnet.x} y= {currnet.y}");

                   /* if (map[currnet.y, currnet.x] == 0)
                        map[currnet.y, currnet.x] = 1;*/

                    if (map[currnet.x, currnet.y] == 0)
                        map[currnet.x, currnet.y] = 1;

                }
                else
                {
                    //int temp = map[x + pos.x + HalfLength, y + pos.y + HalfLength];
                    //if (temp == 0)
                    //    map[x + pos.x + HalfLength, y + pos.y + HalfLength] = 0;
                }
                currnet.x++;
            }
            currnet.y++;
        }


    }

    private static string MapToString()
    {

        string result = string.Empty;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                result += $"{map[i, j]}";
                if (j < map.GetLength(1) - 1)
                    result += ",";
            }

            result += Environment.NewLine;
        }
        return result;

    }


    #region LaodMap
    private static void LoadMap()
    {

    }

    #endregion

}
#endif