using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Gun[] _guns = new Gun[2];
    public int CurGun = 0;

    public int _health { get; private set; }

    public void Init()
    {
        _health = 100;
    }
}
