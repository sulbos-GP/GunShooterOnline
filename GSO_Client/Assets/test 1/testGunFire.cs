using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGunFire : MonoBehaviour
{
    public float time = 0f;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time>1.0f)
        {
            time -= 1.0f;
            gameObject.GetComponentInChildren<Gun>().Fire();
        }
    }
}
