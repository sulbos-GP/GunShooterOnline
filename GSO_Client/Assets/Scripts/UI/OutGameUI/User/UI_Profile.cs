using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using WebCommonLibrary.Models.GameDB;

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
    private ProgressBar experienceBar;

    [SerializeField]
    private Image nextRewardIcon;

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
        this.experience.text        = curExperience.ToString();
        this.experienceBar.value    = curExperience;

        //������ �����͵����ͺ��̽� �����ͼ� ���� ������ ������ ������ �ҷ�����
    }
}
