using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;아직없음
    public float ExitTime;  //나가는데 걸리는 시간

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

        //인게임 종료 및 플레이어를 로비씬으로 이동.(인벤토리 보존)
        //서버에 탈출한 플레이어의 ID

        //C_LeaveGame packet = new C_LeaveGame();

        //Managers.Network.Send(packet);
        Debug.Log("C_LeaveGame");

        //게임씬을 로비로
        Managers.Scene.LoadScene(Define.Scene.Lobby);

    }
}
