using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public Unit CurrentPlayer { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public void loadPlayer()
    {
        Unit player = Resources.Load<Unit>("Unit/UnitModel");
        GameObject playerObj = Instantiate(player).gameObject;
        //playerObj.GetComponent<Unit>().Init();
        playerObj.name = "Player";
        CurrentPlayer = playerObj.GetComponent<Unit>();
    }
}
