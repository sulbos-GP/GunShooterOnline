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
            mMatchStateText.text = "��Ī ���� ����";
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

            var packet = new MatchmakerCancleReq
            {
                world = "Forest",
                region = "asia"
            };

            MatchmakerService service = new MatchmakerService();
            MatchCancleRequest request = service.mMatchmakerResource.GetMatchCancleRequest(packet);
            request.ExecuteAsync(OnProcessMatchCancle);
        }
        catch (HttpRequestException error)
        {
            mMatchStateText.text = "��Ī ��� ����";
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
    public void S2C_MatchComplete(MatchProfile response)
    {

        mMatchButton.interactable = false;
        mMatchStateText.text = "��Ī �Ϸ�";

        mMatchTimeText.transform.parent.gameObject.SetActive(false);
        StopCoroutine(UpdateTimer());

        SystemLogManager.Instance.LogMessage($"��ġ�� �����Ǿ����ϴ� {response.host_ip}:{response.host_port}");

        //����
        response.host_ip = "127.0.0.1";

        Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

        Managers.Network.ConnectToGame(response.host_ip);

    }



}
