using System.Collections;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

public class UnityRay 
{

    /*
    void Start()
    {
        //Ray raycast = new Ray(

        //RaycastManager.Instance.Add(0,raycast);

    }

    RaycastHit2D hit;
    Vector2 dir;
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;

            // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ (z���� 0���� ����)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y));

            // z�� ���� ���� (2D ������ ��� z=0)
            mouseWorldPosition.z = 0;

            dir = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y) -
                new Vector2(transform.position.x, transform.position.y); 



            hit = RaycastManager.Raycast
                (new Vector2(transform.position.x, transform.position.y), dir, 100);
            
        }
        if(Input.GetMouseButtonUp(0))
        {
            hit = null;
        }

        //RaycastManager.Instance.GetRaycast(0).SetDir()
    }


    private void OnDrawGizmos()
    {
        // ���� ��ġ�� ���� ��ü�� ��ġ�� ����
        Vector3 start = transform.position;


        // �Ÿ� ����
        float distance = 100f;

        // �� ��ġ ���
        Vector3 end = start + new Vector3(dir.X,dir.Y,0) * distance;

        // Gizmos ������ ���������� ����
        Gizmos.color = Color.red;

        // Ray�� �׸���
        Gizmos.DrawLine(start, end); 


        if(hit != null)
        {
            Gizmos.DrawSphere(new Vector3(hit.hitPoint.Value.X, hit.hitPoint.Value.Y), 0.5f);
            Debug.Log("hit");
        }
    }
    */
}
