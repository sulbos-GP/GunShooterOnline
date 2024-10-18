using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Models.GameDatabase;
using static UserResource;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Error;

public class UI_DailyQuest : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.DailyQuest;

    private Dictionary<int, GameObject> contents = new Dictionary<int, GameObject>();

    [SerializeField]
    private TMP_Text dailyTime;

    private Coroutine countdownTicketTimer;

    [SerializeField]
    private GameObject questContentPrefab;

    [SerializeField]
    private Transform contentParent;

    public void Awake()
    {
        dailyTime.text = "Unkonwn";
    }

    private IEnumerator DoTicketTimer()
    {
        TimeSpan timeRemaining = TimeSpan.Zero;
        do
        {
            
            DateTime now = DateTime.UtcNow;
            DateTime next = now.Date.AddDays(1);
            timeRemaining = next - now;
            string formatTime = timeRemaining.ToString(@"hh\:mm");
            dailyTime.text = formatTime;

            yield return new WaitForSeconds(60f); //1분

        } while (timeRemaining > TimeSpan.Zero);
        //while (timeRemaining > TimeSpan.Zero); 실제
        //while (timeRemaining <= TimeSpan.Zero); 테스트

        PostUpdateDailyQuest();
    }

    private void PostUpdateDailyQuest()
    {
        ClientCredential crediential = Managers.Web.Models.Credential;
        if (crediential == null)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new DailyTaskReq
        {

        };

        try
        {
            GsoWebService service = new GsoWebService();
            UpdateDailyTaskRequest request = service.mUserResource.GetUpdateDailyTaskRequest(header, body);
            request.ExecuteAsync(OnProcessUpdateDailyQuest);
        }
        catch (Exception ex)
        {
            Managers.SystemLog.Message($"[UI_DailyQuest] {ex.Message}");
        }
    }

    private void OnProcessUpdateDailyQuest(DailyTaskRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {
            Managers.Web.Models.DailyData = response.DailyLoads;

            InitUI();

            LobbyUIManager.Instance.UpdateLobbyAllUI();
        }
    }

    public override void InitUI()
    {

        if (countdownTicketTimer != null)
        {
            StopCoroutine(countdownTicketTimer);
        }
        countdownTicketTimer = StartCoroutine("DoTicketTimer");

        var quests = Managers.Web.Models.DailyQuestData;
        if(quests == null )
        {
            return;
        }

        contents.Clear();
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var quest in quests)
        {
            GameObject prefab = Instantiate(questContentPrefab, contentParent);
            contents.Add(quest.quest_id, prefab);
        }
    }

    public override void UpdateUI()
    {
        var quests = Managers.Web.Models.DailyQuestData;
        if (quests == null)
        {
            return;
        }

        foreach (var quest in quests)
        {
            contents.TryGetValue(quest.quest_id, out var gameObject);
            if(gameObject == null)
            {
                continue;
            }

            gameObject.GetComponentInChildren<UI_DailyQuestContent>().UpdateDailyQuestData(quest);
        }
    }

    public override void OnRegister()
    {
        
    }

    public override void OnUnRegister()
    {
        if (countdownTicketTimer != null)
        {
            StopCoroutine(countdownTicketTimer);
        }
    }
}
