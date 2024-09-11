using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;
using System;
using TMPro;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDB;

public abstract class ClientHub : MonoBehaviour
{
    protected HubConnection mConnection;
    protected string mConnectionUrl { get; set; }
    protected string mConnectionName { get; set; }

    private readonly Queue<Action> mExecutionQueue = new Queue<Action>();

    protected void Start()
    {
        try
        {
            Init();

            CreateHubConnectionHandler();

            SetOnRecivedFunc();

            StartHub();
        }
        catch (Exception ex)
        {
            Managers.SystemLog.Message($"{mConnectionName}서버 HUB 연결 중 에러 발생 : {ex.Message}");
        }

    }

    protected void Update()
    {
        if (mExecutionQueue.Count > 0)
        {
            mExecutionQueue.Dequeue().Invoke();
        }
    }

    protected abstract void Init();

    private void CreateHubConnectionHandler()
    {
        Managers.SystemLog.Message($"{mConnectionName} 서버 빌드중...");

        ClientCredential credential = Managers.Web.credential;
        if (credential.access_token == string.Empty || credential.uid == 0)
        {
            throw new Exception("Client credential 정보가 없습니다.");
        }

        mConnection = new HubConnectionBuilder()
        .WithUrl(mConnectionUrl, options =>
        {
            options.Headers.Add("uid", credential.uid.ToString());
            options.Headers.Add("access_token", credential.access_token);

            options.SkipNegotiation = true;
            options.Transports = HttpTransportType.WebSockets;

        }).ConfigureLogging(options =>
        {
            options.SetMinimumLevel(LogLevel.Information);
            options.AddProvider(new HubLoggerProvider());
        }).Build();
    }

    protected void OnDestroy()
    {
        StoptHub();
    }

    protected void SetHub(string name, string url)
    {
        this.mConnectionName = name;
        this.mConnectionUrl = url;
    }

    public void StartHub()
    {
        Managers.SystemLog.Message($"{mConnectionName} 서버와 연결중...");
        mConnection.StartAsync().Wait();

        if (mConnection.State == HubConnectionState.Connected)
        {
            Managers.SystemLog.Message($"{mConnectionName} 서버와 연결되었습니다.");
            OnConnection();
        }
        else
        {
            Managers.SystemLog.Message($"{mConnectionName} 서버와 연결에 실패하였습니다.");
        }
    }

    public void StoptHub()
    {
        Managers.SystemLog.Message($"{mConnectionName} 서버와 연결 해제중...");

        if(mConnection == null)
        {
            return;
        }

        if(mConnection.State == HubConnectionState.Disconnected)
        {
            return;
        }

        mConnection.StopAsync().Wait();
        if (mConnection.State == HubConnectionState.Disconnected)
        {
            Managers.SystemLog.Message($"{mConnectionName} 서버와 연결이 해제 되었습니다.");
            OnDisConnection();
        }
        else
        {
            Managers.SystemLog.Message($"{mConnectionName} 서버와 연결이 해제에 실패하였습니다.");
        }

    }
    private async void C2S_VerfiyCredential()
    {
        var credential = Managers.Web.credential;
        await mConnection.InvokeAsync("VerfiyCredential", credential);
    }

    public void EnqueueDispatch(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        mExecutionQueue.Enqueue(action);
    }

    protected abstract void SetOnRecivedFunc();
    protected abstract void OnConnection();
    protected abstract void OnDisConnection();
}

public class HubLoggerProvider : ILoggerProvider
{

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new HubLogger();
    }

    public void Dispose()
    {
        
    }
}

public class HubLogger : Microsoft.Extensions.Logging.ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        //SystemLogManager.Instance.LogMessage($"Hub Error : {exception}");
    }
}