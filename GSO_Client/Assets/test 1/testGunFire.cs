using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGunFire : MonoBehaviour
{
    public float time = 0f;
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        animator = GetComponent<Animator>();
        time += Time.deltaTime;
        if(time>1.0f)
        {
            time -= 1.0f;
            animator.SetTrigger("Fire");
           //gameObject.GetComponentInChildren<Gun>().Fire();
        }
    }
}
