using Google.Protobuf.Protocol;
using MathNet.Numerics;
using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;아직없음
    public float exitTime;  //나가는데 걸리는 시간
    private float remainingTime; // 남은 시간
    private bool isExiting; // 현재 탈출 중인지 여부

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
        //피격되거나 움직일 경우 isExiting을 false로 변환할것
        if (exitCoroutine != null)
        {
            Debug.Log("interrupted");
            StopCoroutine(exitCoroutine); // 코루틴 중지
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
            UpdateTimerUI(remainingTime); // UI 업데이트
            yield return new WaitForSeconds(0.1f); // 1초마다 업데이트
        }

        // 탈출 성공 시 처리
        C_ExitGame packet = new C_ExitGame()
        {
            PlayerId = Managers.Object.MyPlayer.Id,
            ExitId = objectId
        };
        Managers.Network.Send(packet);

        Debug.Log("C_LeaveGame");

        //클라이언트의 모든 오브젝트의 내용 클리어
        Managers.Object.Clear();
        Managers.Object.DebugDics();

        // 게임 씬을 로비로
        Managers.Scene.LoadScene(Define.Scene.Lobby);

        isExiting = false; // 탈출 종료
    }

    private bool ExitCheck()
    {
        //hp에 변화가 생길경우
        if(hpIndex != Managers.Object.MyPlayer.Hp)
        {
            return false;
        }
        
        //거리가 달라질경우
        if (Vector2.Distance(posIndex, Managers.Object.MyPlayer.transform.position) > 0.1f)
        {
            return false;
        }

        //총을 쏠경우
        if (Managers.Object.MyPlayer.GetComponent<InputController>()._isFiring)
        {
            return false;
        }

        return true;
    }

    private void UpdateTimerUI(float time)
    {
        Debug.Log($"남은 시간: {time}초");
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
