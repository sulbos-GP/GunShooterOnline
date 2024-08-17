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
        //UnitManager.Instance.loadPlayer();
        ObjectInfo test = new()
        {
            ObjectId = 16777216,
            Name = "player",
            PositionInfo = new PositionInfo()
            {
                PosX = 0,
                PosY = 0,
                RotZ = 0,
            }
        };
        Managers.Object.Add(test,true);
    }

    private void Update()
    {
        //Camera.main.transform.position = localUnit.transform.position;
    }
}
