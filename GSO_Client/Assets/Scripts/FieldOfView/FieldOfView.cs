using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [Header("component")]
    private Mesh mesh;
    [SerializeField] private LayerMask layerMask; //������ ���̾ ���̿� ������ �ϴ� ����

    [Header("view property")]
    [SerializeField] protected float viewDistance; //�⺻�þ� ������ �ִ�Ÿ�
    [SerializeField] protected float fov; //�⺻�þ� �þ߰�.
    [SerializeField] protected int rayCount; //�ú��þ� ������ ����.

    [SerializeField] protected bool showView;

    private Vector3 origin; //���� = �÷��̾��� ��ġ
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
        float angle = startingAngle + setAngleOffset; //���̰� �߻�Ǵ� ����. 
        float angleIncrease = setFov / setRayCount; //�ޱ��� ������. ��

        //mesh�� �� ����. �̺κ��� �޽��� ���� ���ΰ� �ʿ�
        Vector3[] vertices = new Vector3[setRayCount + 1 + 1]; //����. ������ ���� + �������� ����
        Vector2[] uv = new Vector2[vertices.Length]; //uv��ǥ : vertex�� ����
        int[] triangles = new int[setRayCount * 3]; //�޽��� ������ �ﰢ���� ���� = ���̰���*3

        vertices[0] = origin; //ù������ ����
        int vertexIndex = 1;
        int triangleIndex = 0;

        //������ ���� ��ŭ ������ ��ǥ ���
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

            angle -= angleIncrease; //�ݽð�������� ����� +=��. �ð�������� ����� -=
        }

        //mesh ���� �� �� ������Ƽ ����
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    
    public Vector3 GetVectorFromAngle(float angle)
    {
        //angle�� �������� ��ȯ�� �� ���⺤�� ���
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

    public void SetOrigin(Vector3 origin) //������ ��ġ -> �÷��̾��� ��ġ
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection) //�÷��̾ �ٶ󺸴� �������� �������� ����
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection)-fov/2f;
    }
}
