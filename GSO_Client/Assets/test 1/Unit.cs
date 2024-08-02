using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GunStat[] _guns = new GunStat[2];
    public int CurGun = 0;

    public int InstanceID { get;private set; }


    public UnitStat unitStat;
    public void Init()
    {
        InstanceID = gameObject.GetInstanceID();
        unitStat = new UnitStat();
        unitStat.Init();
    }

    //TO-DO : Awake -> Spawn
    public void Awake()
    {

    }

    public void Spawn()
    {
        Init();
    }

}
