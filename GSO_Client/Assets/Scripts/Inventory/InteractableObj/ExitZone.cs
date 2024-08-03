using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;��������
    public float ExitTime;  //�����µ� �ɸ��� �ð�

    private void Awake()
    {
        Init();
    }
    protected override void Init()
    {
        base.Init();
        interactRange = 1;

        SetTriggerSize();
    }


    protected override void SetTriggerSize()
    {
        BoxCollider2D Collider = transform.GetComponent<BoxCollider2D>();
        Collider.size = new Vector2(interactRange, interactRange);
    }

    public override void Interact()
    {
        StartCoroutine(ExitCoroutine(ExitTime));
        
    }

    public void InterruptExit()
    {
        StopCoroutine(ExitCoroutine(ExitTime));
    }

    public IEnumerator ExitCoroutine(float exitTime)
    {
        yield return new WaitForSeconds(exitTime);

        //�ΰ��� ���� �� �÷��̾ �κ������ �̵�.(�κ��丮 ����)
        //������ Ż���� �÷��̾��� ID

        //C_LeaveGame packet = new C_LeaveGame();

        //Managers.Network.Send(packet);
        Debug.Log("C_LeaveGame");

        //���Ӿ��� �κ��
        Managers.Scene.LoadScene(Define.Scene.Lobby);

    }
}
