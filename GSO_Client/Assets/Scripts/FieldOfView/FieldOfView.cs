using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [Header("component")]
    private Mesh mesh;
    [SerializeField] private LayerMask layerMask; //선택한 레이어만 레이에 막히게 하는 역할

    [Header("view property")]
    [SerializeField] protected float viewDistance; //기본시야 레이의 최대거리
    [SerializeField] protected float fov; //기본시야 시야각.
    [SerializeField] protected int rayCount; //시본시야 레이의 개수.

    [SerializeField] protected bool showView;

    private Vector3 origin; //원점 = 플레이어의 위치
    private float startingAngle;

    protected virtual void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        startingAngle = 0;
        origin = Vector3.zero;
        showView = true;
    }

    public void showFov(float setFov, int setRayCount, float setViewDistance ,float setAngleOffset = 0)
    {
        float angle = startingAngle + setAngleOffset; //레이가 발사되는 각도. 
        float angleIncrease = setFov / setRayCount; //앵글의 증가량. 총

        //mesh에 들어갈 변수. 이부분은 메쉬에 대한 공부가 필요
        Vector3[] vertices = new Vector3[setRayCount + 1 + 1]; //정점. 레이의 개수 + 시작점과 끝점
        Vector2[] uv = new Vector2[vertices.Length]; //uv좌표 : vertex와 동일
        int[] triangles = new int[setRayCount * 3]; //메쉬를 형성할 삼각형의 개수 = 레이개수*3

        vertices[0] = origin; //첫정점은 원점
        int vertexIndex = 1;
        int triangleIndex = 0;

        //레이의 개수 만큼 정점의 좌표 계산
        for (int i = 0; i <= setRayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), setViewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * setViewDistance;
            }
            else
            {
                vertex = raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;

            angle -= angleIncrease; //반시계방향으로 진행시 +=로. 시계방향으로 진행시 -=
        }

        //mesh 생성 및 각 프로퍼티 연결
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    
    public Vector3 GetVectorFromAngle(float angle)
    {
        //angle을 라디안으로 변환한 후 방향벡터 계산
        float angleToRadian = angle * (Mathf.PI / 180f); 
        return new Vector3(Mathf.Cos(angleToRadian),Mathf.Sin(angleToRadian)); 
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public void SetOrigin(Vector3 origin) //원점의 위치 -> 플레이어의 위치
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection) //플레이어가 바라보는 방향으로 시작지점 설정
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection)-fov/2f;
    }
}
