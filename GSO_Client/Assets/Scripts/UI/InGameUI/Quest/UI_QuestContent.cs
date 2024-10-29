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

public class UI_QuestContent : MonoBehaviour
{
    [SerializeField]
    private TMP_Text title;

    [SerializeField]
    private TMP_Text process;

    [SerializeField]
    private Image completeImage;

    [SerializeField]
    private Slider slider;

    public void Awake()
    {
        title.text = "Unknown";
        process.text = $"( 0 / 0 )";
        slider.value = 0.0f;

        completeImage.enabled = false;

        ColorBlock colors = slider.colors;
        colors.normalColor = Color.red;
        slider.colors = colors;
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

    public void UpdateQuestData(int quest_id, int progress, bool complete)
    {
        var questBase = Data_master_quest_base.GetData(quest_id);
        if(questBase == null)
        {
            Managers.SystemLog.Message($"[UIQuest.UI_DailyQuestContent.UpdateLevelRewardData] : {quest_id}아이디가 존재하지 않습니다.");
            return;
        }

        title.text = questBase.title;
        process.text = $"( {progress} / {questBase.target} )";
        slider.value = (float)progress / (float)questBase.target;

        if(slider.value >= 1.0f)
        {
            ColorBlock colors = slider.colors;
            colors.normalColor = Color.green;
            slider.colors = colors;
        }

        if (true == complete)
        {
            completeImage.enabled = true;
        }

    }

}
