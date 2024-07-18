using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Doors
{
    public bool Bottom = false;
    public bool Left = false;
    public bool Right = false;
    public bool Top = false;
}

public enum RoomType //TODO : ?????? ????
{
    SPAWN = 0,
    ROOM = 1,
    PATH = 2,
    BOSSROOM = 3,
    PLAYEROWNROOM = 4
}

public class Room : MonoBehaviour
{
    public int RoomId; //????? (???????? ????)
    public RoomType RoomType; //?????? 1: ???? 2:???? 3:??? 4:??????? ??????
    public int RoomSkinId; //??????(???) ?????
    public int roomLevel; //?? ???
    public Vector2Int pos; //?? ???

    public List<GameObject> doors = new();
    public GameObject OwnerPlayer; //??????
    public List<GameObject> currentPlayers = new(); //???????
    public List<GameObject> roomObjects = new(); // ???????

    

    public GameObject TryOwnerPlayer; // ?????? ??????
    private float _tryOwnerProgress; //??????
    private int _lastconquerId = 0;
    
    
    private GameObject occupationBar;
    public float TryOwnerProgress
    {
        get => _tryOwnerProgress;
        set
        {
            _tryOwnerProgress = value;
            
            if (occupationBar == null)
            {
                Transform can = GameObject.Find("Canvas").transform;
                if(can != null)
                    occupationBar = Managers.Resource.Instantiate("UI/OoccupationBar",can) ;
                else
                    Debug.Log("켄버스 오류");
            }
            
            
            if (OwnerPlayer == TryOwnerPlayer) //if owner is me 주인이 나이면
            {
                if (_lastconquerId != OwnerPlayer.GetComponent<CreatureController>().Id)
                {
                    _lastconquerId = OwnerPlayer.GetComponent<CreatureController>().Id;
                    Managers.Resource.Instantiate("UI/Rewards/RewardCanvas");
                    
                }
                if (occupationBar != null)
                {
                    Managers.Resource.Destroy(occupationBar);
                }
                
            }
            else //내가 주인이 아니면
            {
                Debug.Log($"{OwnerPlayer?.name}{TryOwnerPlayer.name},{_tryOwnerProgress}");
                
                occupationBar.GetComponent<UIScrollbar>().size = _tryOwnerProgress;
                occupationBar.GetComponent<UIScrollbar>().colors = new ColorBlock(){normalColor = Color.magenta};
            }
            
            
            
                
        }
    }


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
        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawCube(transform.Find("Pos").transform.position, new Vector3(21, 21));
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if (RoomType == RoomType.SPAWN)
            Gizmos.DrawCube(transform.Find("Pos").transform.position, new Vector3(5, 5));
    }
    #endif


    //-------------------------?? ???----------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentPlayers.Contains(collision.gameObject) == false &&
            collision.isTrigger == false)
        {
            currentPlayers.Add(collision.gameObject);
            MyPlayerController mc;
            if (collision.TryGetComponent(out mc))
                if (mc.CurrentPlanetId != RoomId)
                    mc.CurrentPlanetId = RoomId;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentPlayers.Contains(collision.gameObject) &&
            collision.isTrigger == false) currentPlayers.Remove(collision.gameObject);
    }


    //----------------- ???? --------------------------
    public void InitRoom(Doors _door) // ?????? ???? ??????
    {
        doors.Clear();

        foreach (Transform go in gameObject.transform.Find("Pos"))
            if (go.GetComponent<Tilemap>() != null)
            {
                doors.Add(go.gameObject);
                go.gameObject.SetActive(true);
            }

        if (_door.Left)
        {
            var go = doors.Find(t => t.name.Contains("Left"));
            if (go == null)
                go = doors.Find(t => t.name.Contains("left"));

            go.SetActive(false);
        }

        if (_door.Right)
        {
            var go = doors.Find(t => t.name.Contains("Right"));
            if (go == null)
                go = doors.Find(t => t.name.Contains("right"));

            go.SetActive(false);
        }

        if (_door.Top)
        {
            var go = doors.Find(t => t.name.Contains("Top"));
            if (go == null)
                go = doors.Find(t => t.name.Contains("top"));

            go.SetActive(false);
        }

        if (_door.Bottom)
        {
            var go = doors.Find(t => t.name.Contains("Bottom"));
            if (go == null)
                go = doors.Find(t => t.name.Contains("bottom"));

            go.SetActive(false);
        }
    }


    //------------------- ???????-------------------
    private void ControllRoom()
    {
        if (currentPlayers == null)
        {
        }
    }

    //--------------------- ?? ??? ????----------------
}