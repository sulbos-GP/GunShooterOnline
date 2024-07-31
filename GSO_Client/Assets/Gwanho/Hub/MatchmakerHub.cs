using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using UnityEngine;

public class MatchmakerHub : ClientHub
{
    protected override string mConnectionUrl { get; set; } = "http://localhost:5200/MatchmakerHub";
    //protected override string mConnectionUrl { get; set; } = "http://10.0.2.2:5200/MatchmakerHub";
    protected override string mConnectionName { get; set; } = "매치메이커";
    protected LatencyManager mLatencyManager;

    private void Awake()
    {
        GameObject obj = new GameObject("LatencyManagerObject");
        mLatencyManager = obj.AddComponent<LatencyManager>();
    }

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<long>("S2C_Pong", S2C_Pong);
        mConnection.On<MatchProfile>("S2C_MatchComplete", S2C_MatchComplete);
    }

    protected override void OnConnection()
    {
        C2S_ConnectMatchHub();
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

    private async void C2S_ConnectMatchHub()
    {
        int uid = 101;//= int.Parse(WebManager.Instance.mCredential.uid);
        await mConnection.InvokeAsync("C2S_ConnectMatchHub", uid);
    }

    private async void C2S_Ping()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await mConnection.InvokeAsync("C2S_Ping", 101, timestamp, mLatencyManager.GetAverageLatency());
    }

    private void S2C_Pong(long timestamp)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - timestamp;
        mLatencyManager.AddLatency(latency);
    }

    private void S2C_MatchComplete(MatchProfile profile)
    {
        SystemLogManager.Instance.LogMessage($"매치가 생성되었습니다 {profile.host}:{profile.port}");
    }

    public class MatchProfile
    {
        public string match_id { get; set; } = string.Empty;
        public string host { get; set; } = string.Empty;
        public short port { get; set; } = 0;
    }

}
