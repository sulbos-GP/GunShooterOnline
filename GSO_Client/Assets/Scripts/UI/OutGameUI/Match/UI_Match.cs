using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using static AuthorizeResource;
using static MatchmakerHub;
using static MatchmakerResource;

public class UI_Match : MonoBehaviour
{
    [SerializeField]
    private Button mMatchButton;

    [SerializeField]
    private TextMeshProUGUI mMatchStateText;

    [SerializeField]
    private TextMeshProUGUI mMatchTimeText;

    private GameObject mLoadingScreen;

    private bool mIsProcessMatch = false;
    private bool mIsJoin = true;                //True=��Ī����, False=��Ī���

    private void Awake()
    {
        mMatchButton.onClick.AddListener(OnClickMatch);
        mMatchStateText.text = "��Ī ����";
        mMatchTimeText.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// ��Ī ��ư Ŭ����
    /// </summary>
    private void OnClickMatch()
    {
        if(mIsProcessMatch)
        {
            SystemLogManager.Instance.LogMessage("��Ī �۾��� ������ �Դϴ�. ���� �ڿ� �ٽ� �õ��� �ּ���");
            return;
        }

        if(mIsJoin)
        {
            OnMatchJoin();
        }
        else
        {
            OnMatchCancle();
        }

    }

    /// <summary>
    /// ��Ī ���� ��û
    /// </summary>
    private void OnMatchJoin()
    {
        try
        {
            mMatchStateText.text = "��Ī ���� ��...";
            SystemLogManager.Instance.LogMessage("��Ī ���� ��û...");
            mIsProcessMatch = true;

            var header = new HeaderVerfiyPlayer
            {
                uid = Managers.Web.mCredential.uid,
                access_token = Managers.Web.mCredential.access_token,
            };

            var body = new MatchmakerJoinReq
            {
                world = "Forest",
                region = "asia"
            };

            MatchmakerService service = new MatchmakerService();
            MatchJoinRequest request = service.mMatchmakerResource.GetMatchJoinRequest(header, body);
            request.ExecuteAsync(OnProcessMatchJoin);
        }
        catch (HttpRequestException error)
        {
            mMatchStateText.text = "��Ī ���� ����";
            SystemLogManager.Instance.LogMessage($"��Ī ���� ���� : {error.Message}");
        }

    }

    /// <summary>
    /// ��Ī ���� ����
    /// </summary>
    private void OnProcessMatchJoin(MatchmakerJoinRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == 0)
        {
            SystemLogManager.Instance.LogMessage("��Ī ���� ��û ����");

            mMatchStateText.text = "���� ��Ī ��...";
            mIsJoin = false;

            mMatchTimeText.transform.parent.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
        else
        {
            SystemLogManager.Instance.LogMessage("��Ī ���� ��û ����");
        }
    }

    /// <summary>
    /// ��Ī ��� ��û
    /// </summary>
    private void OnMatchCancle()
    {
        try
        {
            mMatchStateText.text = "��Ī ��� ��...";
            SystemLogManager.Instance.LogMessage("��Ī ��� ��û...");
            mIsProcessMatch = true;

            var header = new HeaderVerfiyPlayer
            {
                uid = Managers.Web.mCredential.uid,
                access_token = Managers.Web.mCredential.access_token,
            };

            var body = new MatchmakerCancleReq
            {
                world = "Forest",
                region = "asia"
            };

            MatchmakerService service = new MatchmakerService();
            MatchCancleRequest request = service.mMatchmakerResource.GetMatchCancleRequest(header, body);
            request.ExecuteAsync(OnProcessMatchCancle);
        }
        catch (HttpRequestException error)
        {
            mMatchStateText.text = "��Ī ��� ����";
            SystemLogManager.Instance.LogMessage($"��Ī ��� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// ��Ī ��� ����
    /// </summary>
    private void OnProcessMatchCancle(MatchmakerCancleRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == 0)
        {
            SystemLogManager.Instance.LogMessage("��Ī ��� ��û ����");

            mIsJoin = true;

            mMatchTimeText.transform.parent.gameObject.SetActive(false);
            StopCoroutine(UpdateTimer());
        }
        else
        {
            SystemLogManager.Instance.LogMessage("��Ī ��� ��û ����");
        }
    }

    /// <summary>
    /// ���ð� Ÿ�̸� ������Ʈ
    /// </summary>
    private IEnumerator UpdateTimer()
    {
        int elapsedTime = 0;
        const float delay = 1.0f;

        while (!mIsJoin)
        {
            elapsedTime += 1;
            UpdateTimerDisplay(elapsedTime);
            yield return new WaitForSeconds(delay);
        }

    }
    private void UpdateTimerDisplay(int elapsedTime)
    {
        int seconds = elapsedTime;
        int minutes = seconds / 60;
        seconds = seconds % 60;

        mMatchTimeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    /// <summary>
    /// ��Ī ����
    /// </summary>
    public void OnMatchComplete()
    {
        mMatchButton.interactable = false;
        mMatchStateText.text = "��Ī �Ϸ�";

        mMatchTimeText.gameObject.SetActive(false);
        mMatchButton.gameObject.SetActive(false);
        StopCoroutine(UpdateTimer());
    }

}
