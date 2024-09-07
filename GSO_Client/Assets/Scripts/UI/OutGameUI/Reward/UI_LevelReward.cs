using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebCommonLibrary.Models.GameDB;

public class UI_LevelReward : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.LevelReward;

    public override void InitUI()
    {
        
    }

    public override void UpdateUI()
    {
        UserInfo profile = Managers.Web.user.UserInfo;

    }
}
