using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDB;

public class UI_Profile : LobbyUI
{

    protected override ELobbyUI type => ELobbyUI.Profile;

    [SerializeField]
    private TMP_Text nickname;

    [SerializeField]
    private TMP_Text level;

    [SerializeField]
    private TMP_Text nextLevel;

    [SerializeField]
    private TMP_Text experience;

    [SerializeField]
    private Image nextRewardIcon;

    [SerializeField]
    private Slider experienceBar;

    [SerializeField]
    private Button metaDataButton;

    [SerializeField]
    private GameObject metaDataObject;

    [SerializeField]
    private Button levelRewardButton;

    [SerializeField]
    private GameObject LevelRewardObject;

    public void Awake()
    {
        metaDataButton.onClick.AddListener(OnProfileButton);
        levelRewardButton.onClick.AddListener(OnLevelRewardButton);
    }

    public void OnProfileButton()
    {
        metaDataObject.SetActive(true);
        LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.Metadata);
    }

    public void OnLevelRewardButton()
    {
        LevelRewardObject.SetActive(true);
        LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.LevelReward);
    }

    public override void InitUI()
    {

    }

    public override void UpdateUI()
    {
        UserInfo profile = Managers.Web.Models.User;
        if(profile == null)
        {
            return;
        }

        this.nickname.text = profile.nickname;

        int curLevel                = (profile.experience < 100) ? 1 : profile.experience / 100;
        int nextLevel               = curLevel + 1;
        this.level.text             = curLevel.ToString();
        this.nextLevel.text         = $"다음 보상 랭크 : {nextLevel}";

        int curExperience           = (profile.experience % 100);
        this.experience.text        = $"{curExperience} / 100";
        this.experienceBar.value    = (curExperience == 0) ? 0 : curExperience / 100;

        foreach (var reward in Data_master_reward_level.AllData())
        {
            if(reward.Value.level == nextLevel)
            {
                nextRewardIcon.sprite = Resources.Load<Sprite>($"Sprite/Item/{reward.Value.icon}");
                break;
            }
        }
    }

    public override void OnRegister()
    {
        
    }

    public override void OnUnRegister()
    {

    }
}
