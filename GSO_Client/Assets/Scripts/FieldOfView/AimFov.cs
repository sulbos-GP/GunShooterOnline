using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimFov : FieldOfView
{
    //�÷��̾ ���� ������ �þ�.
    protected override void Start()
    {
        base.Start();
        viewDistance = 10f;
        fov = 90;
        rayCount = 100;
    }

    // Update is called once per frame
    private void Update()
    {
        ShowFov(fov, rayCount, viewDistance, fov);
    }
}
