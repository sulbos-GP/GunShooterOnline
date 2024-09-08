using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

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

    public void InitLevelRewardData(DB_RewardLevel reward)
    {
        levelText.text          = reward.level.ToString();
        experienceText.text     = reward.experience.ToString();
        //rewardIcon = 
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
