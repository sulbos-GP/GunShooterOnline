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

        SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ������...");
        mConnection.StartAsync().Wait();

        if(mConnection.State == HubConnectionState.Connected)
        {
            SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ����Ǿ����ϴ�.");
            //SendCredential();
            OnConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ���ῡ �����Ͽ����ϴ�.");
        }
        

    }

    private void OnDestroy()
    {
        SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ���� ������...");
        mConnection.StopAsync().Wait();

        if (mConnection.State == HubConnectionState.Disconnected)
        {
            SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ������ ���� �Ǿ����ϴ�.");
            OnDisConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage("��ġ����Ŀ ������ ������ ������ �����Ͽ����ϴ�.");
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
