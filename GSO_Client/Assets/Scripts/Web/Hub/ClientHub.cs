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

public abstract class ClientHub : MonoBehaviour
{
    protected HubConnection mConnection;
    protected string mConnectionUrl { get; set; }
    protected string mConnectionName { get; set; }

    private readonly Queue<Action> mExecutionQueue = new Queue<Action>();

    protected void Start()
    {

        Init();

        CreateHubConnectionHandler();

        SetOnRecivedFunc();

        StartHub();

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
        try
        {
            SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버 빌드중...");

            string accessToken = Managers.Web.credential.access_token;
            if (accessToken == string.Empty)
            {
                throw new Exception("Access Token이 Empty입니다.");
            }

            mConnection = new HubConnectionBuilder()
            .WithUrl(mConnectionUrl, options =>
            {

                options.AccessTokenProvider = () => Task.FromResult(accessToken);
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;

            }).ConfigureLogging(options =>
            {
                options.SetMinimumLevel(LogLevel.Information);
                options.AddProvider(new HubLoggerProvider());
            }).Build();

        }
        catch (Exception ex)
        {
            SystemLogManager.Instance.LogMessage($"서버 빌드중 에러 발생 :  {ex}");
        }
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
        SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결중...");
        mConnection.StartAsync().Wait();

        if (mConnection.State == HubConnectionState.Connected)
        {
            SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결되었습니다.");
            //SendCredential();
            OnConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결에 실패하였습니다.");
        }
    }

    public void StoptHub()
    {
        SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결 해제중...");
        mConnection.StopAsync().Wait();

        if (mConnection.State == HubConnectionState.Disconnected)
        {
            SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결이 해제 되었습니다.");
            OnDisConnection();
        }
        else
        {
            SystemLogManager.Instance.LogMessage($"{mConnectionName} 서버와 연결이 해제에 실패하였습니다.");
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