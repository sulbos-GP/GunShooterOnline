using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{
    public float rotationSpeed = 10f; // �ʴ� ȸ�� �ӵ� (��/��)

    //public NetworkRotation Network;


    public const int targetFrameRate = 60; // �ʴ� ������ ������ ��
    private Coroutine _updateCoroutine;
    float interval = 1f / targetFrameRate; // ������ ���� ���

    void Start()
    {
        // �ڷ�ƾ ����
        _updateCoroutine = StartCoroutine(CustomUpdate());
    }

    IEnumerator CustomUpdate()
    {

        while (true)
        {

            Logic();
            // ���� �����ӱ��� ���
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
        // ������ �ӵ� ����
        //targetFrameRate = Mathf.Max(1, frameRate); // �ּ� 1������ �̻�
        //Debug.Log("Target frame rate set to: " + targetFrameRate);
    }


    private int SendCount = 0;
    void Logic()
    {

        // 1. ���콺 ��ġ �������� (���� ��ǥ)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 2D�̹Ƿ� Z�� ���� 0���� ����

        // 2. ���� ���� ���
        Vector3 direction = mousePosition - transform.position;

        //Debug.Log(direction);
        //���� ��� (���� -> ��)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;


        float currentAngle = transform.eulerAngles.z;

        float newAngle = MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed);

        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        #region Netorwk

        if (SendCount++ % 10 == 0)
        {
            //���� NetworkSimulator.Instance.SendPacket(Network.Recv, newAngle);
            //SendCount = 0;
        }


        #endregion


    }




    public static float MoveTowardsAngle(float currentAngle, float targetAngle, float maxDelta)
    {
        float deltaAngle = DeltaAngle(currentAngle, targetAngle);

      /*  ���� if (Math.Abs(deltaAngle) <= maxDelta)
        {

            return targetAngle;
        }*/



        // 3. �׷��� ������ maxDelta ��ŭ �̵�
        return currentAngle + Sign(deltaAngle) * maxDelta;
    }

    // DeltaAngle ����: �� ���� ������ ���� ª�� �Ÿ� ���
    public static float DeltaAngle(float current, float target)
    {
        float delta = Repeat(target - current, 360f);
        if (delta > 180f)
            delta -= 360f;
        return delta;
    }

    // Repeat ����: ���� 0~360 ������ Ŭ����
    public static float Repeat(float value, float max)
    {
        return value - Mathf.Floor(value / max) * max;
    }

    // Sign ����: ��ȣ ���
    public static float Sign(float value)
    {
        return value < 0 ? -1f : 1f;
    }
}
