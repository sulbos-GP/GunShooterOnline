using Google.Protobuf.Protocol;
using System.Collections;
using TMPro;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;아직없음

    public float exitTime;  //나가는데 걸리는 시간
    private float remainingTime; // 남은 시간
    private bool isExiting = false; // 현재 탈출 중인지 여부

    private Coroutine exitCoroutine;
    private Coroutine retryCoroutine;

    public GameObject timerTextPref; //생성할 타이머의 프리팹
    private GameObject timerUI; //프리팹으로 생성한 UI객체
    private TextMeshProUGUI timeText; //타이머 객체의 텍스트UI

    private void Awake()
    {
        isExiting = false;
        Init();
        SetTriggerSize();
    }

    protected override void Init()
    {
        base.Init();
        interactRange = 1;
        if (exitTime == 0)
        {
            exitTime = 8;
        }
        remainingTime = exitTime;
    }


    protected override void SetTriggerSize()
    {
        BoxCollider2D Collider = transform.GetComponent<BoxCollider2D>();
        Collider.size = new Vector2(interactRange, interactRange);
    }

    [ContextMenu("ExitZone interact")]
    public override void Interact()
    {
        if (false == isExiting) 
        {
            Debug.Log("ExitZone interact");

            timerUI = Managers.Resource.Instantiate("UI/TimerText", GameObject.Find("Canvas").transform);
            timeText = timerUI.GetComponentInChildren<TextMeshProUGUI>();
            if (timeText == null)
            {
                Debug.Log("Can't find UI/TimerText");
                return;
            }
            exitCoroutine = StartCoroutine(ExitCoroutine(exitTime));

            C_ExitGame packet = new C_ExitGame()
            {
                IsNormal = true,
                PlayerId = Managers.Object.MyPlayer.Id,
                ExitId = objectId
            };
            Managers.Network.Send(packet);
        }
        else
        {
            Debug.Log("already exitZone interact");
        }
    }

    [ContextMenu("ExitZone Cancel")]
    public void CancelExit(float retryTime)
    {
        //피격되거나 움직일 경우 isExiting을 false로 변환할것
        if (exitCoroutine != null)
        {
            Debug.Log("ExitZone Cancel");
            StopCoroutine(exitCoroutine); // 코루틴 중지

            timeText = null;
            Managers.Resource.Destroy(timerUI);

            Init();
        }

        if(retryCoroutine == null)
        {
            retryCoroutine = StartCoroutine(RetryCoroutine(retryTime));
        }
    }

    private IEnumerator RetryCoroutine(float retryTime)
    {
        Debug.Log("Start RetryCoroutine");

        while (retryTime > 0)
        {
            retryTime -= 0.1f;
            yield return new WaitForSeconds(0.1f); // 1초마다 업데이트
        }

        Debug.Log("End RetryCoroutine");

        isExiting = false;
        retryCoroutine = null;
    }

    private IEnumerator ExitCoroutine(float exitTime)
    {
        Debug.Log("ExitCoroutine");

        isExiting = true;
        remainingTime = exitTime;
        
        while (remainingTime > 0)
        {
            remainingTime -= 0.1f;

            int seconds = Mathf.FloorToInt(remainingTime);
            int milliseconds = Mathf.FloorToInt((remainingTime - seconds) * 10);

            Vector2 playerPos = Camera.main.WorldToScreenPoint(Managers.Object.MyPlayer.transform.position);
            timerUI.transform.position = playerPos + new Vector2(0, 40);

            if (timeText != null)
            {
                timeText.text = string.Format("{0}:{1}", seconds, milliseconds);
            }

            yield return new WaitForSeconds(0.1f); // 1초마다 업데이트
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
