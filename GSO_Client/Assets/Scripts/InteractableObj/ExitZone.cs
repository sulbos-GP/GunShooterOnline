using Google.Protobuf.Protocol;
using System.Collections;
using TMPro;
using UnityEngine;

public class ExitZone : InteractableObject
{
    // public GameObject gameEndUI;��������

    public float exitTime;  //�����µ� �ɸ��� �ð�
    private float remainingTime; // ���� �ð�
    private bool isExiting; // ���� Ż�� ������ ����

    private float hpIndex;
    private Vector2 posIndex;

    private Coroutine exitCoroutine;
    public GameObject timerTextPref; //������ Ÿ�̸��� ������
    private GameObject timerUI; //���������� ������ UI��ü
    private TextMeshProUGUI timeText; //Ÿ�̸� ��ü�� �ؽ�ƮUI

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
            Debug.Log("ã������");
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
        //�ǰݵǰų� ������ ��� isExiting�� false�� ��ȯ�Ұ�
        if (exitCoroutine != null)
        {
            Debug.Log("interrupted");
            StopCoroutine(exitCoroutine); // �ڷ�ƾ ����
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
            UpdateTimerUI(remainingTime); // UI ������Ʈ
            yield return new WaitForSeconds(0.1f); // 1�ʸ��� ������Ʈ
        }
        DestroyTimerText();
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
        var evniorment = Managers.EnvConfig.GetEnvironmentConfig();
        if(evniorment.LobbyName == "Shelter")
        {
            Managers.Scene.LoadScene(Define.Scene.Shelter);
        }
        else
        {
            Managers.Scene.LoadScene(Define.Scene.Lobby);
        }


        isExiting = false; // Ż�� ����
    }

    private bool ExitCheck()
    {
        //hp�� ��ȭ�� ������ -> �÷��̾ �ǰݴ��� ���°� �߰��ɰ�� �ǰ����� �ٲܰ�
        if(hpIndex != Managers.Object.MyPlayer.Hp)
        {
            return false;
        }
        
        //�Ÿ��� �޶������
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
