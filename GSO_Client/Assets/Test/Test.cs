using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        GameObject go = Managers.Map.LoadMap(9);
        Debug.Log(go.transform.position);
        Debug.Log(go.transform.localPosition);
        // Vector2 offset = go.transform.Find("Center").transform.position;
        // go.transform.position = new Vector2(-offset.x, -offset.y);
    }

}
