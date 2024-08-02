using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Unit localUnit => UnitManager.Instance.CurrentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.loadPlayer();
    }

    private void Update()
    {
        //Camera.main.transform.position = localUnit.transform.position;
    }
}
