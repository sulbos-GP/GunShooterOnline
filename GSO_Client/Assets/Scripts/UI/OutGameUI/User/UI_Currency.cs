using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebCommonLibrary.Models.GameDB;

public class UI_Currency : LobbyUI
{

    protected override ELobbyUI type => ELobbyUI.Currency;

    [SerializeField]
    private TMP_Text ticket;

    [SerializeField]
    private TMP_Text money;

    [SerializeField]
    private TMP_Text gacha;

    public override void InitUI()
    {

    }

    public override void UpdateUI()
    {
        UserInfo info = Managers.Web.user.UserInfo;
        if(info == null)
        {
            return;
        }

        ticket.text = $"{info.ticket} / 10";
        money.text  = info.money.ToString();
        gacha.text  = info.gacha.ToString();

    }

    public override void OnRegister()
    {

    }

    public override void OnUnRegister()
    {

    }
}
