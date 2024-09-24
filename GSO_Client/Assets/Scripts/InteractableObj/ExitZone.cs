using Google.Protobuf.Protocol;
using System.Collections;
using TMPro;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;아직없음

    public float exitTime;  //나가는데 걸리는 시간
    private float remainingTime; // 남은 시간
    private bool isExiting; // 현재 탈출 중인지 여부

    private float hpIndex;
    private Vector2 posIndex;

    private Coroutine exitCoroutine;
    public GameObject timerTextPref; //생성할 타이머의 프리팹
    private GameObject timerUI; //프리팹으로 생성한 UI객체
    private TextMeshProUGUI timeText; //타이머 객체의 텍스트UI

    private void Awake()
    {
        Init();
        SetTriggerSize();
    }

    private void CreateTimerText()
    {
        timerUI = Managers.Resource.Instantiate("UI/TimerText", GameObject.Find("Canvas").transform);
        timeText= timerUI.GetComponentInChildren<TextMeshProUGUI>();
        if (timeText != null) {
            Debug.Log("찾지못함");
        }
    }

    private void DestroyTimerText()
    {
        timeText = null;
        Managers.Resource.Destroy(timerUI);
    }

    protected override void Init()
    {
        base.Init();
        isExiting = false;
        interactRange = 1;
        if (exitTime == 0)
        {
            exitTime = 8;
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
            CreateTimerText();
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
            DestroyTimerText();
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
        DestroyTimerText();
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
        var evniorment = Managers.EnvConfig.GetEnvironmentConfig();
        if(evniorment.LobbyName == "Shelter")
        {
            Managers.Scene.LoadScene(Define.Scene.Shelter);
        }
        else
        {
            Managers.Scene.LoadScene(Define.Scene.Lobby);
        }


        isExiting = false; // 탈출 종료
    }

    private bool ExitCheck()
    {
        //hp에 변화가 생길경우 -> 플레이어가 피격당한 상태가 추가될경우 피격으로 바꿀것
        if(hpIndex != Managers.Object.MyPlayer.Hp)
        {
            return false;
        }
        
        //거리가 달라질경우
        if (Vector2.Distance(posIndex, Managers.Object.MyPlayer.transform.position) > 0.1f)
        {
            return false;
        }

        return true;
    }

    private void UpdateTimerUI(float time)
    {
        

        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 10);

        Vector2 playerPos = Camera.main.WorldToScreenPoint(Managers.Object.MyPlayer.transform.position);
        timerUI.transform.position = playerPos + new Vector2(0,40);

        if (timeText != null)
        {
            timeText.text = string.Format("{0}:{1}", seconds, milliseconds);
        }
        
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
