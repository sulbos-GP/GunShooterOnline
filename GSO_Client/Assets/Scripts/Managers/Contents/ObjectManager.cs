using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    private readonly Dictionary<int, GameObject> _objects = new();
    public readonly Dictionary<int, InvenData> _inventoryDic = new(); //게임 내에 생성된 모든 인벤토리 데이터
    public readonly Dictionary<int, InventoryGrid> _gridDic = new(); //현재 UI에 열려있는 그리드 오브젝트
    public readonly Dictionary<int, ItemObject> _itemDic = new(); //현재 UI에 존재하는 아이템 오브젝트
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
            return;

        var type = GetObjectType(info.ObjectId);
        //GameObjectType type = GameObjectType.Player;
        if (type == GameObjectType.Player)
        {
            if (myPlayer)
            {
                var go = Managers.Resource.Instantiate("Creature/Player/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PositionInfo;
                //MyPlayer.Stat.MergeFrom(info.StatInfo);
                MyPlayer.SyncPos();
            }
            else
            {
                var go = Managers.Resource.Instantiate("Creature/Player/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                var pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PositionInfo;
                //pc.Stat = info.StatInfo;
                pc.SyncPos();

                foreach (Transform t in Managers.Map.CurrentMap.transform)
                {
                    //if (t.GetComponent<PlantsController>().Id == info.PositionInfo.CurrentPlanetId)
                    //{
                    //    go.transform.parent = t.transform;
                    //}
                }
            }
        }
        else if (type == GameObjectType.Monster)
        {
            var go = Managers.Resource.Instantiate($"Creature/Enemy/{info.Name}");
            //GameObject go = Managers.Resource.Instantiate("Creature/{}");

            go.name = info.Name ?? "오류";
            _objects.Add(info.ObjectId, go);

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
            _objects.Add(info.ObjectId, go);

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
            var go = Managers.Resource.Instantiate($"Objects/{info.Name}"); //생성할 오브젝트 경로(박스)
            go.name = $"{info.Name}"; //이름설정
            _objects.Add(info.ObjectId, go); //오브젝트 딕션너리에 추가
            InvenObj invenObj = go.GetComponent<InvenObj>();
            //invenObj.objectId에 해당 오브젝트의 아이디 추가할것
           

            //Add로 인벤 데이터를 생성하여 invenObj.invenData에 넣기
            //(플레이어가 해당 오브젝트와 인터렉트시 이 데이터를 플레이어의 otherInven의 인벤데이터로 불러옴)
            
        }
        else if (type == GameObjectType.InvenData)
        {
           //인벤토리를 가지는 오브젝트가 생성될때 (박스 혹은 플레이어) 인벤데이터를 생성하고 해당 인벤데이터를 딕셔너리에 추가

            //인벤데이터를 생성할때 그리드데이터가 생성되고 그리드 데이터가 생성될때 아이템 데이터까지 모두 생성됨.
        }
    }

    public void AddGridDic(int gridId, InventoryGrid grid)
    {
        //그리드가 생성될때(즉 인벤토리UI를 열때 혹은 박스와 인터렉트 할때)
        //생성된 그리드를 그리드 딕셔너리에 추가
        _gridDic.Add(gridId, grid);
    }

    public void RemoveGridDic()
    {
        //플레이어가 UI를 닫으면 모두 삭제
        _gridDic.Clear();
    }

    public void AddItemDic(int itemId, ItemObject item)
    {
        //아이템이 생성될때(즉 인벤토리UI를 열때 혹은 박스와 인터렉트 할때)
        //생성된 아이템을 아이템 딕셔너리에 추가
        _itemDic.Add(itemId, item);
    }

    public void RemoveItemDic()
    {
        //플레이어가 UI를 닫으면 모두 삭제
        _itemDic.Clear();
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

    public void RemoveItem(int id)
    {
        if (MyPlayer != null && MyPlayer.Id == id)
            return;
        if (_itemDic.ContainsKey(id) == false)
            return;

        var go = FindItemDicById(id);
        if (go == null)
            return;

        _itemDic.Remove(id);
        Managers.Resource.Destroy(go);
    }

    private GameObject FindItemDicById(int id)
    {
        ItemObject go = null;
        _itemDic.TryGetValue(id, out go);


        return go.gameObject;
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

    public void Clear()
    {
        foreach (var obj in _objects.Values)
            Managers.Resource.Destroy(obj);

        _objects.Clear();
        MyPlayer = null;
    }
}