using Google.Protobuf.Protocol;
using MathNet.Numerics;
using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;��������
    public float exitTime;  //�����µ� �ɸ��� �ð�
    private float remainingTime; // ���� �ð�
    private bool isExiting; // ���� Ż�� ������ ����

    private float hpIndex;
    private Vector2 posIndex;

    Coroutine exitCoroutine;

    private void Awake()
    {
        Init();
        SetTriggerSize();
    }
    protected override void Init()
    {
        base.Init();
        isExiting = false;
        interactRange = 1;
        if (exitTime == 0)
        {
            exitTime = 4;
        }
        remainingTime = exitTime;
        hpIndex = 0;
        posIndex = Vector2.zero;
    }


    protected override void SetTriggerSize()
    {
        BoxCollider2D Collider = transform.GetComponent<BoxCollider2D>();
        Collider.size = new Vector2(interactRange, interactRange);
    }

    [ContextMenu("ExitZone interact")]
    public override void Interact()
    {
        Debug.Log("interact");
        if (!isExiting) {
            exitCoroutine = StartCoroutine(ExitCoroutine(exitTime));
        }
        

    }
    [ContextMenu("ExitZone Cancel")]
    public void CancelExit()
    {
        //�ǰݵǰų� ������ ��� isExiting�� false�� ��ȯ�Ұ�
        if (exitCoroutine != null)
        {
            Debug.Log("interrupted");
            StopCoroutine(exitCoroutine); // �ڷ�ƾ ����
            Init();
        }
    }

    private IEnumerator ExitCoroutine(float exitTime)
    {
        Debug.Log("Exit");

        isExiting = true;
        remainingTime = exitTime;
        hpIndex = Managers.Object.MyPlayer.Hp;
        posIndex = Managers.Object.MyPlayer.transform.position;

        while (remainingTime > 0)
        {
            if (ExitCheck() == false)
            {
                CancelExit();
            }

            remainingTime -= 0.1f;
            UpdateTimerUI(remainingTime); // UI ������Ʈ
            yield return new WaitForSeconds(0.1f); // 1�ʸ��� ������Ʈ
        }

        // Ż�� ���� �� ó��
        C_ExitGame packet = new C_ExitGame()
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ExitId = objectId
        };
        Managers.Network.Send(packet);

        Debug.Log("C_LeaveGame");

        //Ŭ���̾�Ʈ�� ��� ������Ʈ�� ���� Ŭ����
        Managers.Object.Clear();
        Managers.Object.DebugDics();

        // ���� ���� �κ��
        Managers.Scene.LoadScene(Define.Scene.Lobby);

        isExiting = false; // Ż�� ����
    }

    private bool ExitCheck()
    {
        //hp�� ��ȭ�� ������
        if(hpIndex != Managers.Object.MyPlayer.Hp)
        {
            return false;
        }
        
        //�Ÿ��� �޶������
        if (Vector2.Distance(posIndex, Managers.Object.MyPlayer.transform.position) > 0.1f)
        {
            return false;
        }

        //���� ����
        if (Managers.Object.MyPlayer.GetComponent<InputController>()._isFiring)
        {
            return false;
        }

        return true;
    }

    private void UpdateTimerUI(float time)
    {
        Debug.Log($"���� �ð�: {time}��");
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
