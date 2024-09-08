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
    private Button profileButton;

    [SerializeField]
    private GameObject profileObject;

    [SerializeField]
    private Button levelRewardButton;

    [SerializeField]
    private GameObject LevelRewardObject;

    public void Awake()
    {
        profileButton.onClick.AddListener(OnProfileButton);
        levelRewardButton.onClick.AddListener(OnLevelRewardButton);
    }

    public void OnProfileButton()
    {

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
        UserInfo profile = Managers.Web.user.UserInfo;

        this.nickname.text = profile.nickname;

        int curLevel                = ((profile.experience / 100) + 1);
        this.level.text             = curLevel.ToString();
        this.nextLevel.text         = (curLevel + 1).ToString();

        int curExperience           = (profile.experience % 100) / 100;
        this.experience.text        = $"{curExperience} / {(curLevel + 1) * 100}";
        this.experienceBar.value    = curExperience;

        //�ӽ�
        //������ �����͵����ͺ��̽� �����ͼ� ���� ������ ������ ������ �ҷ�����
        var reward = new DB_RewardLevel
        {
            reward_id = 10001,
            level = 1,
            experience = 100,
            name = "1000���",
            icon = "IconS_goldbar"
        };

        nextRewardIcon.sprite = Resources.Load<Sprite>($"Sprite/Item/{reward.icon}");

    }
}
