using ServerCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;





public class World : MonoBehaviour
{
    //public int RoomSkinId;
    public Vector2Int pos;

    public List<GameObject> currentPlayers = new();   //  player in world
    public List<GameObject> worldObjects = new();     //  object in world

    private void Start()
    {
    }

    private void Update()
    {
        ControllRoom();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /*  Gizmos.color = new Color(1, 0, 0, 0.1f);
          Gizmos.DrawCube(transform.Find("Pos").transform.position, new Vector3(21, 21));
          Gizmos.color = new Color(0, 1, 0, 0.5f);
          if (RoomType == RoomType.SPAWN)
              Gizmos.DrawCube(transform.Find("Pos").transform.position, new Vector3(5, 5));*/
    }
#endif





    //----------------- ???? --------------------------


    //-------------------  update -------------------
    private void ControllRoom()
    {
        if (currentPlayers == null)
        {
        }
    }

    //--------------------- end update----------------
}