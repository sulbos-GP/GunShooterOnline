using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class MatchmakerHub : ClientHub
{
    protected override string mConnectionUrl { get; set; } = "http://localhost:5200/MatchmakerHub";
    protected LatencyManager mLatencyManager = new LatencyManager();

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<long>("S2C_Pong", S2C_Pong);
        mConnection.On<long, long>("S2C_MatchingComplete", S2C_MatchingComplete);
    }

    protected override void OnConnection()
    {
        StartCoroutine(Send());
    }

    protected override void OnDisConnection()
    {
        StopCoroutine(Send());
    }

    private IEnumerator Send()
    {
        while (mConnection.State == HubConnectionState.Connected)
        {
            C2S_Ping();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private async void C2S_Ping()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await mConnection.InvokeAsync("C2S_Ping", timestamp, mLatencyManager.GetAverageLatency());
    }

    private void S2C_Pong(long timestamp)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - timestamp;
        mLatencyManager.AddLatency(latency);
        SystemLogManager.Instance.LogMessage($"Latency: {latency} ms, AvgLatency: {mLatencyManager.GetAverageLatency()} ms");
    }

    private void S2C_MatchingComplete(long timestamp, long dd)
    {

    }

}
