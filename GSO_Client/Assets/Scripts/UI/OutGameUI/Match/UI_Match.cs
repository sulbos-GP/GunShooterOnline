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
    private bool mIsJoin = true;                //True=매칭참여, False=매칭취소

    private void Awake()
    {
        mMatchButton.onClick.AddListener(OnClickMatch);
        mMatchStateText.text = "매칭 상태";
        mMatchTimeText.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 매칭 버튼 클릭시
    /// </summary>
    private void OnClickMatch()
    {
        if(mIsProcessMatch)
        {
            Managers.SystemLog.Message("매칭 작업이 진행중 입니다. 조금 뒤에 다시 시도해 주세요");
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
    /// 매칭 참여 요청
    /// </summary>
    private void OnMatchJoin()
    {
        try
        {
            mMatchStateText.text = "매칭 참여 중...";
            Managers.SystemLog.Message("매칭 참여 요청...");
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
            mMatchStateText.text = "매칭 참여 실패";
            Managers.SystemLog.Message($"매칭 참여 실패 : {error.Message}");
        }

    }

    /// <summary>
    /// 매칭 참여 응답
    /// </summary>
    private void OnProcessMatchJoin(JoinMatchRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == WebErrorCode.None)
        {
            Managers.SystemLog.Message("매칭 참여 요청 성공");

            mMatchStateText.text = "게임 매칭 중...";
            mIsJoin = false;

            mMatchTimeText.transform.parent.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
        else
        {
            Managers.SystemLog.Message("매칭 참여 요청 실패");
        }
    }

    /// <summary>
    /// 매칭 취소 요청
    /// </summary>
    private void OnMatchCancle()
    {
        try
        {
            mMatchStateText.text = "매칭 취소 중...";
            Managers.SystemLog.Message("매칭 취소 요청...");
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
            mMatchStateText.text = "매칭 취소 실패";
            Managers.SystemLog.Message($"매칭 취소 실패 : {error.Message}");
        }
    }

    /// <summary>
    /// 매칭 취소 응답
    /// </summary>
    private void OnProcessMatchCancle(CancleMatchRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == WebErrorCode.None)
        {
            Managers.SystemLog.Message("매칭 취소 요청 성공");

            mIsJoin = true;

            mMatchTimeText.transform.parent.gameObject.SetActive(false);
            StopCoroutine(UpdateTimer());
        }
        else
        {
            Managers.SystemLog.Message("매칭 취소 요청 실패");
        }
    }

    /// <summary>
    /// 대기시간 타이머 업데이트
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
    /// 매칭 성공
    /// </summary>
    public void OnMatchComplete()
    {
        mMatchButton.interactable = false;
        mMatchStateText.text = "매칭 완료";

        mMatchTimeText.gameObject.SetActive(false);
        mMatchButton.gameObject.SetActive(false);
        StopCoroutine(UpdateTimer());
    }

}
