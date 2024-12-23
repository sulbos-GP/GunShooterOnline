//using GooglePlayGames;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using static AuthorizeResource;
using static UserResource;

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

        var body = new LoadMetadataReq()
        {

        };

        GsoWebService service = new GsoWebService();
        LoadMetadataRequest request = service.mUserResource.GetLoadMetadataRequest(header, body);
        request.ExecuteAsync(OnLoadMetadataResponse);
    }

    public void OnLoadMetadataResponse(LoadMetadataRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {
            Managers.Web.Models.Metadata = response.metadata;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.Metadata);

            metaDataObject.SetActive(true);
        }
        else
        {
            Managers.SystemLog.Message($"Metadata를 불러오는데 실패하였습니다. [{response.error_code}]");
        }
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
        FUser profile = Managers.Web.Models.User;
        if (profile == null)
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
