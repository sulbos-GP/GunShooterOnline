using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Game;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;
using static GameResource;
using static UserResource;

public class UI_LevelRewardData : MonoBehaviour
{


    [SerializeField]
    private TMP_Text    levelText;

    [SerializeField]
    private TMP_Text    experienceText;

    [SerializeField]
    private Image       rewardIcon;

    [SerializeField]
    private TMP_Text    rewardText;

    [SerializeField]
    private Button      receivedButton;

    [SerializeField]
    private TMP_Text    receivedText;

    public void Awake()
    {
        receivedButton.onClick.AddListener(OnClickReceived);
    }

    public void OnClickReceived()
    {
        ClientCredential crediential = Managers.Web.credential;
        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new ReceivedLevelRewardReq
        {
            level = Convert.ToInt32(levelText.text)
        };

        try
        {
            GsoWebService service = new GsoWebService();
            ReceivedLevelRewardRequest request = service.mGameResource.GetReceivedLevelRewardRequest(header, body);
            request.ExecuteAsync(OnProcessReceivedLevelReward);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error : {ex.ToString()}");
        }
    }

    public void OnProcessReceivedLevelReward(ReceivedLevelRewardRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {
            Managers.Web.user.LevelReward = response.LevelReward;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.LevelReward);
        }
    }

    public void InitLevelRewardData(Data_master_reward_level reward)
    {
        levelText.text          = reward.level.ToString();
        experienceText.text     = reward.experience.ToString();
        rewardIcon.sprite       = Resources.Load<Sprite>($"Sprite/Item/{reward.icon}");
        rewardText.text         = reward.name.ToString();
        receivedText.text       = "Å‰µæ";
        receivedButton.interactable = false;
    }

    public void UpdateLevelRewardData(UserLevelReward reward)
    {
        if(reward.received == true)
        {
            receivedText.text = "¹ÞÀ½";
        }
        else
        {
            receivedText.text = "Å‰µæ";
        }
        receivedButton.interactable = !(reward.received);
    }
}
