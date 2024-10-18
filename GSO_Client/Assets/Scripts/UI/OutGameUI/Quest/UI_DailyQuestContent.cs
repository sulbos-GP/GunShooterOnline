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

public class UI_DailyQuestContent : MonoBehaviour
{
    [SerializeField]
    private TMP_Text title;

    [SerializeField]
    private TMP_Text process;

    [SerializeField]
    private Image completeImage;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Button button;

    private FUserRegisterQuest questInfo;

    public void Awake()
    {
        title.text = "Unknown";
        process.text = $"( 0 / 0 )";
        slider.value = 0.0f;

        if (button != null)
        {
            button.onClick.AddListener(OnClickDailyQuest);
            button.interactable = false;
        }

        completeImage.enabled = false;

        ColorBlock colors = slider.colors;
        colors.normalColor = Color.red;
        slider.colors = colors;
    }

    public void OnClickDailyQuest()
    {

        button.interactable = false;

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

        var body = new DailyQuestReq
        {
            QuestId = questInfo.quest_id,
        };

        try
        {
            GsoWebService service = new GsoWebService();
            CompleteDailyQuestRequest request = service.mUserResource.GetCompleteDailyQuestRequest(header, body);
            request.ExecuteAsync(OnProcessCompleteDailyQuest);
        }
        catch (Exception ex)
        {
            Managers.SystemLog.Message($"[UI_DailyQuestContent] {ex.Message}");
        }
    }

    private void OnProcessCompleteDailyQuest(DailyQuestRes response)
    {
        if (response.error_code == WebErrorCode.None)
        {
            Managers.Web.Models.User = response.User;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.Currency);

            Managers.Web.Models.DailyQuestData = response.DailyQuset;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.DailyQuest);

            completeImage.enabled = true;
        }
    }

    public void UpdateDailyQuestData(FUserRegisterQuest quest)
    {
        this.questInfo = quest;
        var questBase = Data_master_quest_base.GetData(quest.quest_id);
        if(questBase == null)
        {
            Managers.SystemLog.Message($"[UIQuest.UI_DailyQuestContent.UpdateLevelRewardData] : {quest.quest_id}아이디가 존재하지 않습니다.");
            return;
        }

        title.text = questBase.title;
        process.text = $"( {quest.progress} / {questBase.target} )";
        slider.value = (float)quest.progress / (float)questBase.target;

        if(slider.value >= 1.0f)
        {
            ColorBlock colors = slider.colors;
            colors.normalColor = Color.green;
            slider.colors = colors;
        }

        if (true == quest.completed)
        {
            completeImage.enabled = true;
        }
        else
        {
            if (quest.progress == questBase.target)
            {
                button.interactable = true;
            }
        }
    }

}
