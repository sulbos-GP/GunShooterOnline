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

    //TO-DO : 추후에 스텟 클래스 제작 예정.
    public void SetHealth(int var)
    {
        _health += var;
    }

    public void Awake()
    {
        Init();
    }

}
