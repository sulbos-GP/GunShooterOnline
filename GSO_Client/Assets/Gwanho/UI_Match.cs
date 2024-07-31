using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AuthorizeResource;
using static MatchmakerResource;

public class UI_Match : MonoBehaviour
{
    [SerializeField]
    private Button mMatchButton;

    [SerializeField]
    private TextMeshProUGUI mMatchModeText;

    [SerializeField]
    private TextMeshProUGUI mMatchTimeText;

    private GameObject mLoadingScreen;

    private bool mIsProcessMatch = false;
    private bool mIsJoin = true;                //True=��Ī����, False=��Ī���

    private void Awake()
    {
        mMatchButton.onClick.AddListener(OnClickMatch);
        mMatchModeText.text = "��Ī ����";
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
            SystemLogManager.Instance.LogMessage("��Ī ���� ��û...");
            mIsProcessMatch = true;

            var packet = new MatchmakerJoinReq
            {
                world = "Forest",
                region = "asia"
            };

            MatchmakerService service = new MatchmakerService();
            MatchJoinRequest request = service.mMatchmakerResource.GetMatchJoinRequest(packet);
            request.ExecuteAsync(OnProcessMatchJoin);
        }
        catch (HttpRequestException error)
        {
            SystemLogManager.Instance.LogMessage($"��Ī ���� ���� : {error}");
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

            mMatchModeText.text = "��Ī ���";
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
            SystemLogManager.Instance.LogMessage("��Ī ��� ��û...");
            mIsProcessMatch = true;

            var packet = new MatchmakerCancleReq
            {

            };

            MatchmakerService service = new MatchmakerService();
            MatchCancleRequest request = service.mMatchmakerResource.GetMatchCancleRequest(packet);
            request.ExecuteAsync(OnProcessMatchCancle);
        }
        catch (HttpRequestException error)
        {
            SystemLogManager.Instance.LogMessage($"��Ī ��� ���� : {error}");
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

            mMatchModeText.text = "��Ī ����";
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

}
