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

            // 화면 좌표를 월드 좌표로 변환 (z축을 0으로 설정)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y));

            // z축 값을 조정 (2D 게임의 경우 z=0)
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
        // 시작 위치를 현재 객체의 위치로 설정
        Vector3 start = transform.position;


        // 거리 설정
        float distance = 100f;

        // 끝 위치 계산
        Vector3 end = start + new Vector3(dir.X,dir.Y,0) * distance;

        // Gizmos 색상을 붉은색으로 설정
        Gizmos.color = Color.red;

        // Ray를 그리기
        Gizmos.DrawLine(start, end); 


        if(hit != null)
        {
            Gizmos.DrawSphere(new Vector3(hit.hitPoint.Value.X, hit.hitPoint.Value.Y), 0.5f);
            Debug.Log("hit");
        }
    }
    */
}
