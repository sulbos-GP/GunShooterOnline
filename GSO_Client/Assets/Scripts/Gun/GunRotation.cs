using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{
    public float rotationSpeed = 10f; // 초당 회전 속도 (도/초)

    //public NetworkRotation Network;


    public const int targetFrameRate = 60; // 초당 실행할 프레임 수
    private Coroutine _updateCoroutine;
    float interval = 1f / targetFrameRate; // 프레임 간격 계산

    void Start()
    {
        // 코루틴 시작
        _updateCoroutine = StartCoroutine(CustomUpdate());
    }

    IEnumerator CustomUpdate()
    {

        while (true)
        {

            Logic();
            // 다음 프레임까지 대기
            yield return new WaitForSeconds(interval);
        }
    }

    void OnDisable()
    {
        if (_updateCoroutine != null)
        {
            StopCoroutine(_updateCoroutine);
            _updateCoroutine = null;
        }
    }

    public void SetTargetFrameRate(int frameRate)
    {
        // 프레임 속도 조정
        //targetFrameRate = Mathf.Max(1, frameRate); // 최소 1프레임 이상
        //Debug.Log("Target frame rate set to: " + targetFrameRate);
    }


    private int SendCount = 0;
    void Logic()
    {

        // 1. 마우스 위치 가져오기 (월드 좌표)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 2D이므로 Z축 값은 0으로 고정

        // 2. 방향 벡터 계산
        Vector3 direction = mousePosition - transform.position;

        //Debug.Log(direction);
        //각도 계산 (라디안 -> 도)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;


        float currentAngle = transform.eulerAngles.z;

        float newAngle = MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed);

        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        #region Netorwk

        if (SendCount++ % 10 == 0)
        {
            //승현 NetworkSimulator.Instance.SendPacket(Network.Recv, newAngle);
            //SendCount = 0;
        }


        #endregion


    }




    public static float MoveTowardsAngle(float currentAngle, float targetAngle, float maxDelta)
    {
        float deltaAngle = DeltaAngle(currentAngle, targetAngle);

      /*  승현 if (Math.Abs(deltaAngle) <= maxDelta)
        {

            return targetAngle;
        }*/



        // 3. 그렇지 않으면 maxDelta 만큼 이동
        return currentAngle + Sign(deltaAngle) * maxDelta;
    }

    // DeltaAngle 구현: 두 각도 사이의 가장 짧은 거리 계산
    public static float DeltaAngle(float current, float target)
    {
        float delta = Repeat(target - current, 360f);
        if (delta > 180f)
            delta -= 360f;
        return delta;
    }

    // Repeat 구현: 값을 0~360 범위로 클램핑
    public static float Repeat(float value, float max)
    {
        return value - Mathf.Floor(value / max) * max;
    }

    // Sign 구현: 부호 계산
    public static float Sign(float value)
    {
        return value < 0 ? -1f : 1f;
    }
}
