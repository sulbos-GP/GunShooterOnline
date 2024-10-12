using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Spreadsheet;
using UnityEngine;


public class ObjectManager
{
    //게임 내의 플레이어, 상자 , 탈출구 등 모든 오브젝트
    private readonly Dictionary<int, GameObject> _objects = new();

    //게임룸 안에 존재하는 인벤토리에 관련된 데이터들
    //추가 : 핸들러에서 InventorySet을 할경우 각 데이터들이 추가됨
    //삭제 : 인벤토리와 그리드는 해당 객체가 사라질때, 아이템은 합쳐지거나 삭제될때
    
    public readonly Dictionary<int, S_RaycastShoot> _rayDic = new();
    public MyPlayerController MyPlayer { get; set; }


    public static GameObjectType GetObjectType(int id)
    {
        var type = (id >> 24) & 0x7f;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
            return;

        if (_objects.ContainsKey(info.ObjectId))
        {
            Managers.SystemLog.Message($"{info.ObjectId} _objects Contains Key");

            return;
        }

        Managers.SystemLog.Message("spawnID : " + info.ObjectId);

        var type = GetObjectType(info.ObjectId);
        //GameObjectType type = GameObjectType.Player;
        if (type == GameObjectType.Player)
        {
            if (myPlayer)
            {
                var go = Managers.Resource.Instantiate("Creature/Player/MyPlayer");
                //var go = Managers.Resource.Instantiate("UnitModel");
                go.name = info.Name;
                if(_objects.TryAdd(info.ObjectId, go) == false)
                {
                    Managers.SystemLog.Message($"{info.ObjectId} _objects add fail");
                }
                  

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PositionInfo;
                //MyPlayer.Stat.MergeFrom(info.StatInfo);
                MyPlayer.SyncPos();
                MyPlayer.playerInput = MyPlayer.GetComponent<InputController>().playerInput;
            }
            else
            {
                var go = Managers.Resource.Instantiate("Creature/Player/Player");
                go.name = info.Name;
                if (_objects.TryAdd(info.ObjectId, go) == false)
                {
                    Managers.SystemLog.Message($"{info.ObjectId} _objects add fail");
                }

                var pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PositionInfo;
                //pc.Stat = info.StatInfo;
                pc.SyncPos();
                //foreach (Transform t in Managers.Map.CurrentMap.transform)
                //{
                //if (t.GetComponent<PlantsController>().Id == info.PositionInfo.CurrentPlanetId)
                //{
                //    go.transform.parent = t.transform;
                //}
                //}
            }


        }
        else if (type == GameObjectType.Monster)
        {
            var go = Managers.Resource.Instantiate($"Creature/Enemy/{info.Name}");
            //GameObject go = Managers.Resource.Instantiate("Creature/{}");

            go.name = info.Name ?? "오류";
            if (_objects.TryAdd(info.ObjectId, go) == false)
            {
                Managers.SystemLog.Message($"{info.ObjectId} _objects add fail");
            }

            var mc = go.GetComponent<MonsterController>();


            //mc.PlanetSide = info.PositionInfo.Side;
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PositionInfo;
            //mc.Stat = info.StatInfo;
            mc.SyncPos();

            Debug.Log("다시소환");
            mc.ChangeStat?.Invoke();

            //foreach (Transform t in Managers.Map.CurrentMap.transform)
            //{
            //    //if (t.GetComponent<PlantsController>().Id == info.PositionInfo.CurrentPlanetId)
            //    //{
            //    //    go.transform.parent = t.transform;
            //    //}
            //}


            //mc.transform.gameObject.SetActive(false);
        }
        else if (type == GameObjectType.Projectile)
        {
            var go = Managers.Resource.Instantiate($"Objects/{info.Name}");

            go.name = $"{info.Name}";
            if (_objects.TryAdd(info.ObjectId, go) == false)
            {
                Managers.SystemLog.Message($"{info.ObjectId} _objects add fail");
            }

            var ac = go.GetComponent<ArrowController>();
            ac.Id = info.ObjectId;
            ac.OwnerId = info.OwnerId;
            ac.PosInfo = info.PositionInfo;
            //ac.Stat = info.StatInfo;
            ac.SyncPos(false);
            if (info.SkillId != 0)
                ac.SkillId = info.SkillId;

            //Debug.Log($"pos {ac.PosInfo.PosX}, {ac.PosInfo.PosY}");
        }
        else if (type == GameObjectType.Box)
        {
            var go = Managers.Resource.Instantiate($"Objects/Box"); //생성할 오브젝트 경로(박스)
            go.name = $"{info.Name}"; //이름설정
            go.transform.position = new Vector2(info.PositionInfo.PosX, info.PositionInfo.PosY);
            if (_objects.TryAdd(info.ObjectId, go) == false)
            {
                Managers.SystemLog.Message($"{info.ObjectId} _objects add fail");
            } //오브젝트 딕션너리에 추가

            Box boxScr = go.GetComponent<Box>();
            boxScr.objectId = info.ObjectId;
            boxScr.interactType = InteractType.InventoryObj;
            boxScr.SetBox((int)info.Box.X, (int)info.Box.Y, info.Box.Weight);
            //boxScr.interactRange = 나중에 필요시 추가(박스의 종류를 나눌경우)

            //Add로 인벤 데이터를 생성하여 boxScr.invenData에 넣기
            //(플레이어가 해당 오브젝트와 인터렉트시 이 데이터를 플레이어의 otherInven의 인벤데이터로 불러옴)

        }
        else if (type == GameObjectType.Exitzone)
        {
            var go = Managers.Resource.Instantiate($"Objects/ExitZone"); //생성할 오브젝트 경로(박스)
            go.name = $"{info.Name}"; //이름설정
            go.transform.position = new Vector2(info.PositionInfo.PosX, info.PositionInfo.PosY);
            if (_objects.TryAdd(info.ObjectId, go) == false)
            {
                Managers.SystemLog.Message("_objects add fail");
            }

            ExitZone exitZone = go.GetComponent<ExitZone>();
            exitZone.objectId = info.ObjectId;
            exitZone.interactType = InteractType.Exit;
            exitZone.exitTime = 5; //임시추가
        }
    }


    public void Remove(int id)
    {
        if (MyPlayer != null && MyPlayer.Id == id)
            return;
        if (_objects.ContainsKey(id) == false)
            return;

        var go = FindById(id);
        if (go == null)
            return;
        

        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }



    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);


        return go;
    }


    public GameObject FindCreature(Vector3Int cellPos)
    {
        foreach (var obj in _objects.Values)
        {
            var cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (var obj in _objects.Values)
            if (condition.Invoke(obj))
                return obj;

        return null;
    }

    public void DebugDics()
    {
        Debug.Log($"Object : ");
        foreach (GameObject obj in _objects.Values)
        {
            Debug.Log($"{obj.name} ");
        }
    }

    public void Clear()
    {
        foreach (var obj in _objects.Values)
            Managers.Resource.Destroy(obj);
        _objects.Clear();
        MyPlayer = null;
    }

    
}