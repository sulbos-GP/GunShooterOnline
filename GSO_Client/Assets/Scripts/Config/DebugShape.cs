using NPOI.OpenXmlFormats.Wordprocessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rectangle
{
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;

    public Rectangle(float width, float height)
    {
        topLeft = new Vector2(-width / 2, -height / 2);
        topRight = new Vector2(+width / 2, -height / 2);
        bottomLeft = new Vector2(-width / 2, +height / 2);
        bottomRight = new Vector2(+width / 2, +height / 2);
    }
};

public class DebugShape : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Rectangle rect;
    //private SkillType skillType = SkillType.None; //애니메이션용

    public bool activeLine;

    private float _width;
    private float _height;


    private bool isLineSet;
    public void Init()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 5; // 사각형을 만들기 위해 5개의 점이 필요합니다.
        lineRenderer.loop = false; // 마지막 점이 처음으로 연결되지 않도록 설정합니다.
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        isLineSet = false;
    }

    private void Awake()
    {
        Init();
    }

    public void SetDrawLine(float width, float height)
    {
        isLineSet = true;

        _width = width;
        _height = height;

        rect = new Rectangle(width, height);

        // LineRenderer의 점 위치 설정
        UpdateDrawLine();
    }

    [ContextMenu("히트박스 그리기")]
    public void UpdateDrawLine()
    {
#if UNITY_EDITOR
        if (!isLineSet)
        {
            return;
        }
        lineRenderer.positionCount = 5;
        lineRenderer.SetPosition(0, (Vector2)gameObject.transform.position + rect.topLeft);
        lineRenderer.SetPosition(1, (Vector2)gameObject.transform.position + rect.topRight);
        lineRenderer.SetPosition(2, (Vector2)gameObject.transform.position + rect.bottomRight);
        lineRenderer.SetPosition(3, (Vector2)gameObject.transform.position + rect.bottomLeft);
        lineRenderer.SetPosition(4, (Vector2)gameObject.transform.position + rect.topLeft);
#endif
    }

    [ContextMenu("히트박스 지우기")]
    public void ClearDrawLine()
    {
#if UNITY_EDITOR
        lineRenderer.positionCount = 0;
#endif
    }
}
