using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    private readonly Dictionary<int, GameObject> _objects = new();
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

    public void Clear()
    {
        foreach (var obj in _objects.Values)
            Managers.Resource.Destroy(obj);

        _objects.Clear();
        MyPlayer = null;
    }
}