using Google.Protobuf.Protocol;
using NPOI.OpenXmlFormats.Dml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;아직없음
    public float ExitTime;  //나가는데 걸리는 시간
    private float remainingTime; // 남은 시간
    private bool isExiting; // 현재 탈출 중인지 여부
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
        //피격되거나 움직일 경우
        if (isExiting)
        {
            Debug.Log("interrupted");
            remainingTime = ExitTime; // 남은 시간 초기화
            isExiting = false;
            StopAllCoroutines(); // 코루틴 중지
        }

    }

    public IEnumerator ExitCoroutine(float exitTime)
    {
        Debug.Log("Exit");

        isExiting = true;
        remainingTime = exitTime;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.1f); // 1초마다 업데이트
            remainingTime -= 0.1f;
            UpdateTimerUI(remainingTime); // UI 업데이트
        }

        // 탈출 성공 시 처리
        C_ExitGame packet = new C_ExitGame()
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ExitId = objectId
        };
        Managers.Network.Send(packet);
        Debug.Log("C_LeaveGame");

        // 게임 씬을 로비로
        Managers.Scene.LoadScene(Define.Scene.Lobby);

        isExiting = false; // 탈출 종료
    }

    private void UpdateTimerUI(float time)
    {
        Debug.Log($"남은 시간: {time}초");
    }
}
