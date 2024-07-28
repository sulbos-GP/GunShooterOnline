using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEditor.MemoryProfiler;

public abstract class ClientHub : MonoBehaviour
{
    protected HubConnection mConnection;

    protected abstract string mConnectionUrl { get; set; }

    void Start()
    {
        mConnection = new HubConnectionBuilder()
            .WithUrl(mConnectionUrl)
            .Build();

        SetOnRecivedFunc();

        SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결중...");
        mConnection.StartAsync().Wait();

        if(mConnection.State == HubConnectionState.Connected)
        {
            SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결되었습니다.");
            //SendCredential();
            OnConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결에 실패하였습니다.");
        }
        

    }

    private void OnDestroy()
    {
        SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결 해제중...");
        mConnection.StopAsync().Wait();

        if (mConnection.State == HubConnectionState.Disconnected)
        {
            SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결이 해제 되었습니다.");
            OnDisConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage("매치메이커 서버와 연결이 해제에 실패하였습니다.");
        }

    }

    private async void SendCredential()
    {
        var credential = WebManager.Instance.mCredential;
        await mConnection.InvokeAsync("RecvCredential", credential);
    }

    protected abstract void SetOnRecivedFunc();
    protected abstract void OnConnection();
    protected abstract void OnDisConnection();
}
