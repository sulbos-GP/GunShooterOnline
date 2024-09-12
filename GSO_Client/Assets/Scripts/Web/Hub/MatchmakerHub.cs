using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using UnityEngine;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.Match;

public class MatchmakerHub : ClientHub
{
    
    protected UI_Match mMatchUI;

    private void Awake()
    {
        mMatchUI        = GetComponent<UI_Match>();
    }

    protected override void Init()
    {
        string name = "매치메이커";
        string url = Managers.EnvConfig.GetEnvironmentConfig().MatchmakerBaseUri + "/MatchmakerHub";

        this.SetHub(name, url);
    }

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<string, MatchProfile>("S2C_MatchComplete", S2C_MatchComplete);
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
        ClientCredential credential = Managers.Web.Models.Credential;
        if (credential == null)
        {
            return;
        }

        int uid = credential.uid;
        string token = credential.access_token;
        await mConnection.InvokeAsync("C2S_VerfiyUser", uid, token);
    }

    public void S2C_MatchComplete(string client_id, MatchProfile response)
    {
        EnqueueDispatch(() =>
        {
            Managers.SystemLog.Message($"매치가 생성되었습니다 {response.host_ip}:{response.host_port}");

            mMatchUI.OnMatchComplete();

            //승현
            response.host_ip = "113.60.249.123";

            //로컬 애뮬
            //response.host_ip = "10.0.2.2";

            Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

            Managers.Scene.LoadScene(Define.Scene.Forest);

            ClientCredential credential = Managers.Web.Models.Credential;
            if (credential == null)
            {
                return;
            }

            //Enter할때 uid 보내주셈
            C_EnterGame c_EnterGame = new C_EnterGame();
            c_EnterGame.Name = "jish";
            c_EnterGame.Credential = new CredentiaInfo()
            {
                Uid = credential.uid
            };

            Managers.Network.Send(c_EnterGame);
            Debug.Log("접속중");

            //Managers.Network.ConnectToGame(response.host_ip);
        });
    }

    public void S2C_VerfiyUser(int error)
    {
        EnqueueDispatch(() =>
        {
            if (error == 0)
            {
                Managers.SystemLog.Message($"매치메이커 인증에 성공하였습니다.");
            }
            else
            {
                Managers.SystemLog.Message($"매치메이커 인증에 실패하였습니다.");
            }
        });
    }
}
