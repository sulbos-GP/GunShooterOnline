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

public class UI_MatchState : MonoBehaviour
{
    [SerializeField]
    private TMP_Text matchStateText;

    [SerializeField]
    private TMP_Text matchTimeText;

    private void Awake()
    {
        matchStateText.text = "매칭 상태";
        matchTimeText.gameObject.SetActive(false);
    }

    public void Start()
    {
        
    }
    
    public void SetStateText(string state)
    {
        matchStateText.text = state;
        Managers.SystemLog.Message(state);
    }
    public void StartTimer()
    {
        matchTimeText.gameObject.SetActive(true);
        StartCoroutine(UpdateTimer());
    }

    public void StopTiemr()
    {
        matchTimeText.gameObject.SetActive(false);
        StopCoroutine(UpdateTimer());
    }

    /// <summary>
    /// 대기시간 타이머 업데이트
    /// </summary>
    private IEnumerator UpdateTimer()
    {
        int elapsedTime = 0;
        const float delay = 1.0f;

        while (true)
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

        matchTimeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }



}
