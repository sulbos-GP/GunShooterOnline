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


public class MapEditor
{
    //static int Length = 140; //무조건 짝수

    static int HalfLength = 0;
    
    public static List<Tuple<Transform, Vector2Int>> Rooms = new List<Tuple<Transform, Vector2Int>>();
    static string MapStr = "";
    static string MapTempStr = "";
    static string Path = "D:\\/";
    static int[,] map;
    static float distance; // -55.5 -55.5 -> -55 -55   = result = 0.5f
    static Vector2 startPosTopLeft = Vector2.zero;

    static Vector2 min = new Vector2(99999, 99999);


    [MenuItem("Tools/GenerateMap")]
    private static void GenerateMap()
    {
        MakeMap();
    }

    
    public static void MakeMap()
    {

        // x좌표,y좌표,종류(1:스폰2:일반3:통로),방종류(1,2,3...)
        Debug.Log("시작");
       
        map = null;
        distance = 0;
        MapStr = "";


        GameObject go = GameObject.FindGameObjectWithTag("Map");
        Rooms.Clear();
        go.GetComponent<RoomController>().MapInit();



        foreach (Transform tr in go.transform)
        {
            if (tr.name == "Center")
            {
                Debug.Log("Center cheak");
                continue;
            }
            
            Vector2 t = tr.transform.Find("Pos").transform.Find("Center").position;
            Rooms.Add(new Tuple<Transform, Vector2Int>(tr, 
                new Vector2Int(Mathf.RoundToInt(t.x), Mathf.RoundToInt(t.y))));
        }
        // 튜플 추가


        Rooms = Rooms.OrderBy(t => t.Item2.y)
            .ThenBy(t => t.Item2.x).ToList();
        // 7 8 9
        // 4 5 6
        // 1 2 3
        //순서 정리




        SetMapSizeAndInit(Rooms);


        int roomCount = 0;

        foreach (Tuple<Transform, Vector2Int> _room in Rooms)
        {

            Room room = _room.Item1.GetComponent<Room>();
            if (room == null)
            {
                Debug.LogError("오류");
                return;
            }

            roomCount++;
            if (_room.Item1.name.Contains("Room") == true)
            {
                MapStr += $"{_room.Item2.x}/{_room.Item2.y}/{room.RoomType}/{room.RoomId}";
                GenMapTemp(_room.Item1,_room.Item2,0);
            }
            else if (_room.Item1.name.Contains("Path") == true)
            { 
                MapStr += $"{_room.Item2.x}/{_room.Item2.y}/{room.RoomType}/{room.RoomId}";
                GenMapTemp(_room.Item1, _room.Item2, 1);
            }
            else
            {
                Debug.LogError("오류" + _room.Item1.name);
            }
            MapStr += Environment.NewLine;
            
        }

        //--------------  -------------- 끝 -------------- ---------------


        Debug.Log(min);

        string total =
             roomCount + Environment.NewLine
             + MapStr
             + min.x +'/'+ min.y + Environment.NewLine
             + map.GetLength(0) + Environment.NewLine
             + MapToString();

        Debug.Log($"끝{total.Length}");
        File.WriteAllText(Path + $"Map_{roomCount.ToString("000")}.txt", total);
        //File.WriteAllText(Path + $"Map_{roomCount.ToString("000")}.txt", total);
        File.WriteAllText("Assets/Resources/Data/" + $"Map_{roomCount.ToString("000")}.txt", total);

    }

    
    private static void SetMapSizeAndInit(List<Tuple<Transform, Vector2Int>> _rooms)
    {
        foreach (Tuple<Transform, Vector2Int> _room in Rooms)
        {
            Room room = _room.Item1.GetComponent<Room>();
            if (room == null)
            {
                Debug.LogError("오류");
                return;
            }

            if (_room.Item1.name.Contains("Room") == true || _room.Item1.name.Contains("Path") == true)
            {

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

                distance = Math.Abs(Mathf.CeilToInt(tempVector.x)) - tempVector.x;

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
            }


        }  // for문 순환  == min초기화

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


    } //------------------------------  한번 순환 끝 ------------------------------



    private static void GenMapTemp(Transform tr, Vector2Int pos, int isRoom)
    {

        Tilemap _base = null;
        foreach (Transform _tr in tr)
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

        Vector2 _min = _base.CellToWorld(new Vector3Int(_base.cellBounds.xMin, _base.cellBounds.yMin)); 
        Vector2Int min = new Vector2Int(Mathf.RoundToInt(_min.x + distance) + HalfLength, Mathf.RoundToInt(_min.y + distance) + HalfLength);
        
        //Vector2 _max = _base.CellToWorld(new Vector3Int(_base.cellBounds.xMax, _base.cellBounds.yMax));
        //Vector2Int max = new Vector2Int(Mathf.RoundToInt(_max.x + distance) + HalfLength, Mathf.RoundToInt(_max.y + distance) + HalfLength);

        Vector2Int currnet = min;

        for (int y = _base.cellBounds.yMin; y <= _base.cellBounds.yMax; y++)
        {
            currnet.x = min.x;
            
            for (int x = _base.cellBounds.xMin; x <= _base.cellBounds.xMax; x++)
            {
                //Debug.Log(currnet);

                TileBase tile = _base.GetTile(new Vector3Int(x, y, 0));
                
                if (tile != null)
                {
                    if (isRoom == 0) //q방
                    {
                        //Debug.Log($"x = {currnet.x} y= {currnet.y}");

                        int temp = map[currnet.x, currnet.y];
                        if (temp == 0)
                            map[currnet.x, currnet.y] = 1;
                    }
                    else if (isRoom == 1) // 길목
                    {
                        int temp = map[currnet.x, currnet.y];
                        if (temp == 0)
                            map[currnet.x, currnet.y] = 2;

                    }
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
                    result+= ",";
            }

            result += Environment.NewLine;
        }
        return result;

    }

}
#endif