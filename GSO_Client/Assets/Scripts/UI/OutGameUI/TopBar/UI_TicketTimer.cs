using NPOI.SS.Formula.Functions;
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
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);

            DateTime next = profile.recent_ticket_dt.AddMinutes(GameDefine.WAIT_TICKET_MINUTE);

            if (countdownTicketTimer != null)
            {
                StopCoroutine(countdownTicketTimer);
            }
            countdownTicketTimer = StartCoroutine(DoTicketTimer(next));
        }
    }

    public override void OnRegister()
    {
        
    }

    public override void OnUnRegister()
    {
        if (countdownTicketTimer != null)
        {
            StopCoroutine(countdownTicketTimer);
        }
    }

    private IEnumerator DoTicketTimer(DateTime next)
    {

        TimeSpan timeRemaining = TimeSpan.Zero;
        do
        {
            DateTime now = DateTime.UtcNow;
            timeRemaining = next - now;
            string formatTime = timeRemaining.ToString(@"mm\:ss");
            timerText.text = formatTime;

            yield return new WaitForSeconds(1f); //1초

        } while (timeRemaining > TimeSpan.Zero);
        //while (timeRemaining > TimeSpan.Zero); 실제
        //while (timeRemaining <= TimeSpan.Zero); 테스트

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
            Managers.SystemLog.Message($"[TicketTimer] got ticket.");

            if(response.User.ticket < GameDefine.MAX_TICKET)
            {
                LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.TicketTimer);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else if(response.error_code == WebErrorCode.TicketRemainingTime)
        {
            DateTime next = DateTime.UtcNow.AddSeconds(response.RemainingTime);
            countdownTicketTimer = StartCoroutine(DoTicketTimer(next));

            Managers.SystemLog.Message($"[TicketTimer] [{response.RemainingTime}]seconds before you get a ticket");
        }
        else if(response.error_code == WebErrorCode.TicketAlreadyMax)
        {
            Managers.SystemLog.Message($"[TicketTimer] Tickets hit the limit");
        }
        else
        {
            Managers.SystemLog.Message($"[TicketTimer] Unknown error code {response.error_code.ToString()}");
        }
    }
}
