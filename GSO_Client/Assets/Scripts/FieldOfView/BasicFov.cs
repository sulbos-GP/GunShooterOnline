using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFov : FieldOfView
{
    //�÷��̾� �ֺ� �����̿� ���̴� �þ�
    protected override void Start()
    {
        base.Start();
        viewDistance = 1.5f;
        fov = 360f;
        rayCount = 400;
    }

    void Update()
    {
        showFov(fov, rayCount, viewDistance);
    }
}
