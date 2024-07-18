using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public List<GameObject> Rooms = new();
    public List<GameObject> Players = new();
    public int Distance = 44;


    [ContextMenu("MapInfo")]
    public void MapPosInfo()
    {
        foreach (Transform t in transform) Debug.Log(t.position);
    }


    [ContextMenu("MapInit")]
    public void MapInit()
    {
        SetClear();
        SetDoors();
        SetRoomID();
    }

    public void SetClear()
    {
        Rooms.Clear();
        Debug.Log("Rooms.Clear() transform.childCount}");
    }

    public void SetDoors()
    {
        var map = GameObject.FindGameObjectWithTag("Map");
        if (map == null)
            map = gameObject;

        foreach (Transform go in map.transform)
            if (go.name.Contains("Room") || go.name.Contains("room"))
                Rooms.Add(go.gameObject);

        Rooms = Rooms.OrderBy(t => t.transform.position.y)
            .ThenBy(t => t.transform.position.x).ToList();

        foreach (var r in Rooms)
        {
            Doors doors = new();
            Vector2 main = r.transform.position;
            var temp = 0;
            foreach (var next in Rooms)
            {
                if (r == next)
                    continue;

                Vector2 sub = next.transform.position;
                if (Vector2.Distance(main, sub) < Distance + 1)
                {
                    temp++;
                    if (main.x < sub.x && main.y == sub.y)
                        doors.Right = true;
                    else if (main.x > sub.x && main.y == sub.y)
                        doors.Left = true;
                    else if (main.x == sub.x && main.y < sub.y)
                        doors.Top = true;
                    else if (main.x == sub.x && main.y > sub.y)
                        doors.Bottom = true;
                    else
                        Debug.LogError("���̻�");
                } // doors ��
            }

            //Debug.Log($"{r.name} {temp} {doors.Right} {doors.Left}{doors.Top}{doors.Bottom} ");
            r.GetComponent<Room>()?.InitRoom(doors);
        }
    } //setdoor

    private void SetRoomID()
    {
        var t = 0;
        foreach (Transform transform in transform)
        {
            var room = transform.GetComponent<Room>();
            if (room != null) room.RoomId = t++;
        }
    }

}