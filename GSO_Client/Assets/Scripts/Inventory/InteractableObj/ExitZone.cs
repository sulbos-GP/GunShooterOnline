using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;��������
    public float ExitTime;  //�����µ� �ɸ��� �ð�
    private float remainingTime; // ���� �ð�
    private bool isExiting; // ���� Ż�� ������ ����
    private void Awake()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        interactRange = 1;
        if (ExitTime == 0)
        {
            ExitTime = 4;
        }
        remainingTime = ExitTime;
        SetTriggerSize();
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
        StartCoroutine(ExitCoroutine(ExitTime));

    }
    [ContextMenu("ExitZone interrupt")]
    public void InterruptExit()
    {
        //�ǰݵǰų� ������ ���
        if (isExiting)
        {
            Debug.Log("interrupted");
            remainingTime = ExitTime; // ���� �ð� �ʱ�ȭ
            isExiting = false;
            StopAllCoroutines(); // �ڷ�ƾ ����
        }

    }

    public IEnumerator ExitCoroutine(float exitTime)
    {
        Debug.Log("Exit");

        isExiting = true;
        remainingTime = exitTime;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.1f); // 1�ʸ��� ������Ʈ
            remainingTime -= 0.1f;
            UpdateTimerUI(remainingTime); // UI ������Ʈ
        }

        // Ż�� ���� �� ó��
        C_ExitGame packet = new C_ExitGame()
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ExitId = objectId
        };
        Managers.Network.Send(packet);
        Debug.Log("C_LeaveGame");

        // ���� ���� �κ��
        Managers.Scene.LoadScene(Define.Scene.Lobby);

        isExiting = false; // Ż�� ����
    }

    private void UpdateTimerUI(float time)
    {
        Debug.Log($"���� �ð�: {time}��");
    }
}
