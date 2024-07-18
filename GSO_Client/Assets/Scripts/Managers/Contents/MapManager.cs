using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private int size;
    public GameObject CurrentMap { get; private set; }
    public RoomController RoomController => CurrentMap.GetComponent<RoomController>();


    public GameObject LoadMap(int mapId)
    {
        DestroyMap();

        var mapName = "Map_" + mapId.ToString("000");
        var go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";
        go.transform.position += new Vector3(0, 0);
        CurrentMap = go;
        go.GetComponent<RoomController>().MapInit();
        
        Vector2 offset = go.transform.Find("Center").transform.position;
        go.transform.position = new Vector2(-offset.x, -offset.y);
        
        return go;
    }

    public void DestroyMap()
    {
        var map = GameObject.Find("Map");
        if (map != null)
        {
            Destroy(map);
            CurrentMap = null;
        }
    }


    [ContextMenu("InstanceMap")]
    public void InstanceMap(Transform tr)
    {
        var map = new GameObject { name = "@Map" };
        var go = Managers.Resource.Instantiate("Map/StandardRoom", map.transform);

        go.transform.position = tr.position;
    }


    public bool MapRoomInfoInit(S_RoomInfo roomPacket)
    {
        var map = Managers.Map.CurrentMap;
        var first = true;

        if (map != null)
        {
            foreach (Transform room in map.transform)
            {
                if(room.name == "Center") continue;
                
                
                Room _room;
                if (room.TryGetComponent(out _room))
                {
                    var pos = room.Find("Pos").transform.Find("Center");
                    if (pos == null)
                        Debug.LogError("�ʿ���");


                    RoomInfo rinfo;
                    
                    if (first)
                    {
                        rinfo = roomPacket.RoomInfos.First(r => Vector2.Distance(new Vector2(r.PosX, r.PosY), pos.localPosition) < 0.01f);

                        Vector2 distance = pos.localPosition - pos.position;
                        map.transform.position += (Vector3)distance;
                        first = false;
                    }
                    else
                    {
                        rinfo = roomPacket.RoomInfos.Single(r => Vector2.Distance(new Vector2(r.PosX, r.PosY), pos.position) < 0.01f);
                    }

                    if (rinfo == null)
                        Debug.LogError("error _ 2");
                    _room.roomLevel = rinfo.RoomLevel;
                    _room.RoomId = rinfo.RoomId;
                    _room.RoomType = (RoomType)rinfo.RoomType;
                }
                else
                {
                    Debug.LogError("error _ 1");
                    return false;
                }
            }

            return true;
        }

        Debug.LogError("�� ����");
        return false;
    }

    public bool MapRoomUpdate(S_RoomInfo roomPacket)
    {
        if (roomPacket.RoomInfos.Count > 1)
            Debug.LogError("MapRoomUpdate Error");
        var Map = Managers.Map.CurrentMap;


        var info = roomPacket.RoomInfos[0];
        if (Map != null)
        {
            var _room = RoomController.Rooms.First(r => r.GetComponent<Room>().RoomId == info.RoomId).GetComponent<Room>();
            
            if (_room != null & _room.RoomId != 0)
            {
                _room.OwnerPlayer = Managers.Object.FindById(info.OwnerId);
                _room.TryOwnerPlayer = Managers.Object.FindById(info.TryOwnerId);
                _room.TryOwnerProgress = info.TryOwnerValue;

                return true;
            }

            Debug.Log("�� ã�� ����");
            return false;
        }

        Debug.LogError("�� ����");
        return false;
    }
}