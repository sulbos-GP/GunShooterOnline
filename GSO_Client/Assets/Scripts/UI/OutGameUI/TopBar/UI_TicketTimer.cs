using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;
using static UserResource;

public class UI_TicketTimer : LobbyUI
{

    protected override ELobbyUI type => ELobbyUI.TicketTimer;


    [SerializeField]
    private TMP_Text timerText;

    private Coroutine countdownTicketTimer;

    public void Awake()
    {

    }

    public override void InitUI()
    {
        CheackTicket();
    }

    public override void UpdateUI()
    {
        CheackTicket();
    }

    private void CheackTicket()
    {
        FUser profile = Managers.Web.Models.User;
        if (profile == null)
        {
            return;
        }

        if (profile.ticket >= GameDefine.MAX_TICKET)
        {
            if (countdownTicketTimer != null)
            {
                StopCoroutine(countdownTicketTimer);
            }
        }

        TimeSpan timeDifference = profile.recent_ticket_dt - DateTime.UtcNow;
        int totalWaitSeconds = GameDefine.WAIT_TICKET_SECOND - (int)timeDifference.TotalSeconds;

        countdownTicketTimer = StartCoroutine(DoTicketTimer(totalWaitSeconds));
    }

    public override void OnRegister()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnUnRegister()
    {
        if (countdownTicketTimer != null)
        {
            StopCoroutine(countdownTicketTimer);
        }
    }

    private IEnumerator DoTicketTimer(int seconds)
    {
        while (seconds > 0)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            timerText.text = string.Format("{0:00}:{1:00}", minutes, remainingSeconds);

            yield return new WaitForSeconds(1f);
            seconds--;
        }

        PostUpdateTicket();
    }

    private void PostUpdateTicket()
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

        var body = new UpdateTicketReq
        {

        };

        try
        {
            GsoWebService service = new GsoWebService();
            UpdateTicketRequest request = service.mUserResource.GetUpdateTicketRequest(header, body);
            request.ExecuteAsync(OnProcessUpdateTicket);
        }
        catch (Exception ex)
        {
            Managers.SystemLog.Message($"[TicketTimer] {ex.Message}");
        }
    }

    private void OnProcessUpdateTicket(UpdateTicketRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {
            Managers.Web.Models.User = response.User;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.Currency);
            Managers.SystemLog.Message($"[TicketTimer] 티켓을 얻었습니다.");
            this.gameObject.SetActive(false);
        }
        else if(response.error_code == WebErrorCode.TicketRemainingTime)
        {
            countdownTicketTimer = StartCoroutine(DoTicketTimer(response.RemainingTime));
            Managers.SystemLog.Message($"[TicketTimer] 티켓을 얻기 까지 [{response.RemainingTime}]sec 오차가 있습니다.");
        }
        else if(response.error_code == WebErrorCode.TicketAlreadyMax)
        {
            Managers.SystemLog.Message($"[TicketTimer] 티켓이 이미 한계입니다.");
        }
        else
        {
            Managers.SystemLog.Message($"[TicketTimer] Unknown error code {response.error_code.ToString()}");
        }
    }
}
