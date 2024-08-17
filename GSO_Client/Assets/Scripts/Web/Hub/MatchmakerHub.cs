using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using UnityEngine;

public class MatchmakerHub : ClientHub
{
    protected override string mConnectionUrl { get; set; } = "http://113.60.249.123:5200/MatchmakerHub";
    protected override string mConnectionName { get; set; } = "매치메이커";
    
    protected UI_Match mMatchUI;

    private void Awake()
    {
        mMatchUI        = GetComponent<UI_Match>();
    }

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<int>("S2C_VerfiyUser", S2C_VerfiyUser);
        mConnection.On<MatchProfile>("S2C_MatchComplete", S2C_MatchComplete);
    }

    protected override void OnConnection()
    {
        C2S_VerfiyUser();
        StartCoroutine(UpdateLatency());
    }

    protected override void OnDisConnection()
    {
        StopCoroutine(UpdateLatency());
    }

    private IEnumerator UpdateLatency()
    {
        while (mConnection.State == HubConnectionState.Connected)
        {
            //C2S_Ping();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public async void C2S_VerfiyUser()
    {
        var credential = Managers.Web.mCredential;
        int uid = int.Parse(credential.uid);
        string token = credential.access_token;
        await mConnection.InvokeAsync("C2S_VerfiyUser", uid, token);
    }

    public void S2C_MatchComplete(MatchProfile response)
    {
        EnqueueDispatch(() =>
        {
            SystemLogManager.Instance.LogMessage($"매치가 생성되었습니다 {response.host_ip}:{response.host_port}");

            mMatchUI.OnMatchComplete();

            //로컬
            response.host_ip = "116.41.116.247";

            Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

            //Managers.Network.ConnectToGame(response.host_ip);
        });
    }

    public void S2C_VerfiyUser(int error)
    {
        EnqueueDispatch(() =>
        {
            if (error == 0)
            {
                SystemLogManager.Instance.LogMessage($"매치메이커 인증에 성공하였습니다.");
            }
            else
            {
                SystemLogManager.Instance.LogMessage($"매치메이커 인증에 실패하였습니다.");
            }
        });
    }
}
