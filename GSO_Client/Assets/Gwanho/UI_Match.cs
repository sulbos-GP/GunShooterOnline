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
    private bool mIsJoin = true;                //True=매칭참여, False=매칭취소

    private void Awake()
    {
        mMatchButton.onClick.AddListener(OnClickMatch);
        mMatchModeText.text = "매칭 참여";
        mMatchTimeText.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 매칭 버튼 클릭시
    /// </summary>
    private void OnClickMatch()
    {
        if(mIsProcessMatch)
        {
            SystemLogManager.Instance.LogMessage("매칭 작업이 진행중 입니다. 조금 뒤에 다시 시도해 주세요");
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
            SystemLogManager.Instance.LogMessage("매칭 참여 요청...");
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
            SystemLogManager.Instance.LogMessage($"매칭 참여 실패 : {error}");
        }

    }

    /// <summary>
    /// 매칭 참여 응답
    /// </summary>
    private void OnProcessMatchJoin(MatchmakerJoinRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == 0)
        {
            SystemLogManager.Instance.LogMessage("매칭 참여 요청 성공");

            mMatchModeText.text = "매칭 취소";
            mIsJoin = false;

            mMatchTimeText.transform.parent.gameObject.SetActive(true);
            StartCoroutine(UpdateTimer());
        }
        else
        {
            SystemLogManager.Instance.LogMessage("매칭 참여 요청 실패");
        }
    }

    /// <summary>
    /// 매칭 취소 요청
    /// </summary>
    private void OnMatchCancle()
    {
        try
        {
            SystemLogManager.Instance.LogMessage("매칭 취소 요청...");
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
            SystemLogManager.Instance.LogMessage($"매칭 취소 실패 : {error}");
        }
    }

    /// <summary>
    /// 매칭 취소 응답
    /// </summary>
    private void OnProcessMatchCancle(MatchmakerCancleRes response)
    {
        mIsProcessMatch = false;
        if (response.error_code == 0)
        {
            SystemLogManager.Instance.LogMessage("매칭 취소 요청 성공");

            mMatchModeText.text = "매칭 참여";
            mIsJoin = true;

            mMatchTimeText.transform.parent.gameObject.SetActive(false);
            StopCoroutine(UpdateTimer());
        }
        else
        {
            SystemLogManager.Instance.LogMessage("매칭 취소 요청 실패");
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

}
