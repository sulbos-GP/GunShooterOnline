using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
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
            Managers.SystemLog.Message("��Ī �۾��� ������ �Դϴ�. ���� �ڿ� �ٽ� �õ��� �ּ���");
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
            Managers.SystemLog.Message("��Ī ���� ��û...");
            mIsProcessMatch = true;

            ClientCredential crediential = Managers.Web.Credential;
            if (crediential == null)
            {
                return;
            }

            var header = new HeaderVerfiyPlayer
            {
                uid = crediential.uid.ToString(),
                access_token = crediential.access_token,
            };

            var body = new JoinMatchReq
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
            Managers.SystemLog.Message($"��Ī ���� ���� : {error.Message}");
        }

    }

    /// <summary>
    /// ��Ī ���� ����
    /// </summary>
    private void OnProcessMatchJoin(JoinMatchRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == WebErrorCode.None)
        {
            Managers.SystemLog.Message("��Ī ���� ��û ����");

            mMatchStateText.text = "���� ��Ī ��...";
            mIsJoin = false;

            mMatchTimeText.transform.parent.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
        else
        {
            Managers.SystemLog.Message("��Ī ���� ��û ����");
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
            Managers.SystemLog.Message("��Ī ��� ��û...");
            mIsProcessMatch = true;

            ClientCredential credential = Managers.Web.Credential;
            if (credential == null)
            {
                return;
            }

            var header = new HeaderVerfiyPlayer
            {
                uid = credential.uid.ToString(),
                access_token = credential.access_token,
            };

            var body = new CancleMatchReq
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
            Managers.SystemLog.Message($"��Ī ��� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// ��Ī ��� ����
    /// </summary>
    private void OnProcessMatchCancle(CancleMatchRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == WebErrorCode.None)
        {
            Managers.SystemLog.Message("��Ī ��� ��û ����");

            mIsJoin = true;

            mMatchTimeText.transform.parent.gameObject.SetActive(false);
            StopCoroutine(UpdateTimer());
        }
        else
        {
            Managers.SystemLog.Message("��Ī ��� ��û ����");
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
