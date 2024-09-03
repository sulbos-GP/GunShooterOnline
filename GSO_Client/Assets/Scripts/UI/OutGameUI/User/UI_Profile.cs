using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebCommonLibrary.Models.GameDB;

public class UI_Profile : LobbyUI
{

    protected override ELobbyUI type => ELobbyUI.Profile;

    [SerializeField]
    private TMP_Text level;

    [SerializeField]
    private TMP_Text nickname;

    public override void UpdateUI()
    {
        UserInfo profile = Managers.Web.user.UserInfo;
        this.level.text      = (profile.experience / 100).ToString();
        this.nickname.text   = profile.nickname;
    }
}
