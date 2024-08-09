using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using UnityEngine;

public class MatchmakerHub : ClientHub
{
    protected override string mConnectionUrl { get; set; } = "http://10.0.2.2:5200/MatchmakerHub";
    //protected override string mConnectionUrl { get; set; } = "http://127.0.0.1:5200/MatchmakerHub";
    protected override string mConnectionName { get; set; } = "매치메이커";
    
    protected LatencyManager mLatencyManager;
    protected UI_Match mMatchUI;

    private void Awake()
    {
        mLatencyManager = GetComponent<LatencyManager>();
        mMatchUI        = GetComponent<UI_Match>();
    }

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<long>("S2C_Pong", S2C_Pong);
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
        var credential = WebManager.Instance.mCredential;
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
            response.host_ip = "127.0.0.1";

            Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

            Managers.Network.ConnectToGame(response.host_ip);
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

    private async void C2S_Ping()
    {
        int uid = int.Parse(WebManager.Instance.mCredential.uid);
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await mConnection.InvokeAsync("C2S_Ping", uid, timestamp, mLatencyManager.GetAverageLatency());
    }

    private void S2C_Pong(long timestamp)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - timestamp;
        mLatencyManager.AddLatency(latency);
    }

}
