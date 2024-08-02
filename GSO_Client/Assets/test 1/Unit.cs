using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GunStat[] _guns = new GunStat[2];
    public int CurGun = 0;

    public int _health { get; private set; }

    public void Init()
    {
        _health = 100;
    }

    public void Awake()
    {
        Init();
    }

}
