using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimFov : FieldOfView
{
    //플레이어가 보는 방향의 시야.
    protected override void Start()
    {
        base.Start();
        viewDistance = 10f;
        fov = 45;
        rayCount = 50;
    }

    // Update is called once per frame
    private void Update()
    {
        ShowFov(fov, rayCount, viewDistance, fov);
    }
}
