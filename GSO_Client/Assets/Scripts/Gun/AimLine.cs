using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AimLine : MonoBehaviour
{
    //√—æÀ±À¿˚ ∂Û¿Œ ∑ª¥ı∑Ø
    public LineRenderer bulletLine;

    //πﬂªÁπ¸¿ß ∂Û¿Œ∑ª¥ı∑Ø
    private LineRenderer aimLineA;
    private LineRenderer aimLineB;
    public Color lineColor = new Color(1f, 0f, 0f, 0.5f);

    public void Init()
    {
        bulletLine = GetComponent<LineRenderer>();
        aimLineA = transform.GetChild(1).GetComponent<LineRenderer>();
        aimLineB = transform.GetChild(2).GetComponent<LineRenderer>();

        bulletLine.positionCount = 2;
        SetAimLine(aimLineA);
        SetAimLine(aimLineB);
    }

    private void SetAimLine(LineRenderer line)
    {
        line.positionCount = 2;
        line.startColor = lineColor;
        line.endColor = lineColor;
    }

    public void OnAimLine()
    {
        aimLineA.enabled = true;
        aimLineB.enabled = true;
    }

    public void OffAimLine()
    {
        aimLineA.enabled = false;
        aimLineB.enabled = false;
    }

    public void SetAimLine(Vector3 startpos, Vector3 endPosA, Vector3 endPosB)
    {
        SetLine(aimLineA,startpos, endPosA);
        SetLine(aimLineB, startpos, endPosB);
    }

    public void SetBulletLine(Vector3 startpos, Vector3 endPos)
    {
        SetLine(bulletLine, startpos, endPos);
    }

    private void SetLine(LineRenderer target,Vector3 startpos, Vector3 endPos)
    {
        target.SetPosition(0, startpos);
        target.SetPosition(1, endPos);
    }
}
