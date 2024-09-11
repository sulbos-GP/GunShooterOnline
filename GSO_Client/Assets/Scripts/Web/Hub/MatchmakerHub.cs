using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Net.Http;
using UnityEngine;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.Match;
using static MatchmakerResource;

public class MatchmakerHub : ClientHub
{
    private GameObject matchStatePrefab;
    private GameObject matchStateUI;

    private string world;
    private string region;

    private bool mIsProcessing = false;

    private void Awake()
    {
        matchStatePrefab = Resources.Load<GameObject>("Prefabs/UI/Lobby/MatchState");
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

    /// <summary>
    /// 매칭 성공
    /// </summary>
    //public void OnMatchComplete()
    //{
    //    mMatchButton.interactable = false;
    //    mMatchStateText.text = "매칭 완료";

    //    mMatchTimeText.gameObject.SetActive(false);
    //    mMatchButton.gameObject.SetActive(false);
    //    StopTiemr();
    //}
    public void S2C_MatchComplete(string client_id, MatchProfile response)
    {
        EnqueueDispatch(() =>
        {
            Managers.SystemLog.Message($"매치가 생성되었습니다 {response.host_ip}:{response.host_port}");

            if (matchStateUI == null)
            {
                return;
            }
            UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

            state.SetStateText("매칭 완료");
            state.StopTiemr();
            Destroy(matchStateUI);

            {
                //승현
                response.host_ip = "113.60.249.123";

                //로컬 애뮬
                //response.host_ip = "10.0.2.2";

                Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

                Managers.Scene.LoadScene(Define.Scene.Forest);

                //Enter할때 uid 보내주셈
                C_EnterGame c_EnterGame = new C_EnterGame();
                c_EnterGame.Name = "jish";
                c_EnterGame.Credential = new CredentiaInfo()
                {
                    Uid = Managers.Web.credential.uid
                };

                Managers.Network.Send(c_EnterGame);
                Debug.Log("접속중");
            }

            //Managers.Network.ConnectToGame(response.host_ip);
        });
    }

    /// <summary>
    /// 매칭 참여 요청
    /// </summary>
    public void OnJoinRequest(string name)
    {
        if(matchStatePrefab != null)
        {
            return;
        }

        if(mIsProcessing == true)
        {
            return;
        }
        world = name;
        region = "asia";

        var canvas = Managers.FindObjectOfType<Canvas>();
        matchStateUI = Managers.Instantiate(matchStatePrefab, canvas.transform);
        UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

        try
        {
            state.SetStateText("매칭 참여 중...");
            mIsProcessing = true;

            ClientCredential crediential = Managers.Web.credential;
            var header = new HeaderVerfiyPlayer
            {
                uid = crediential.uid.ToString(),
                access_token = crediential.access_token,
            };

            var body = new JoinMatchReq
            {
                world = world,
                region = region
            };

            MatchmakerService service = new MatchmakerService();
            MatchJoinRequest request = service.mMatchmakerResource.GetMatchJoinRequest(header, body);
            request.ExecuteAsync(OnProcessMatchJoin);
        }
        catch (HttpRequestException error)
        {
            mIsProcessing = false;
            state.SetStateText($"매칭 참여 실패 : {error.Message}");
        }
    }

    /// <summary>
    /// 매칭 참여 응답
    /// </summary>
    private void OnProcessMatchJoin(JoinMatchRes response)
    {
        mIsProcessing = false;

        if (matchStateUI == null)
        {
            return;
        }
        UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

        if (response.error_code == WebErrorCode.None)
        {

            state.SetStateText("매칭 요청 성공");

            state.StartTimer();

            state.SetStateText("게임 매칭 중...");
        }
        else
        {
            state.SetStateText("매칭 참여 요청 실패");
        }
    }

    /// <summary>
    /// 매칭 취소 요청
    /// </summary>
    public void OnCancleRequeset()
    {
        if (matchStateUI == null)
        {
            return;
        }
        UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

        try
        {
            state.SetStateText("매칭 취소 중...");
            mIsProcessing = true;

            ClientCredential credential = Managers.Web.credential;
            var header = new HeaderVerfiyPlayer
            {
                uid = credential.uid.ToString(),
                access_token = credential.access_token,
            };

            var body = new CancleMatchReq
            {
                world = world,
                region = name
            };

            MatchmakerService service = new MatchmakerService();
            MatchCancleRequest request = service.mMatchmakerResource.GetMatchCancleRequest(header, body);
            request.ExecuteAsync(OnProcessMatchCancle);
        }
        catch (HttpRequestException error)
        {
            mIsProcessing = false;
            state.SetStateText($"매칭 취소 실패 : {error.Message}");
        }
    }

    /// <summary>
    /// 매칭 취소 응답
    /// </summary>
    private void OnProcessMatchCancle(CancleMatchRes response)
    {
        mIsProcessing = false;

        if (matchStateUI == null)
        {
            return;
        }
        UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

        if (response.error_code == WebErrorCode.None)
        {
            state.SetStateText("매칭 취소 요청 성공");
            state.StopTiemr();
            Destroy(matchStateUI);
        }
        else
        {
            Managers.SystemLog.Message("매칭 취소 요청 실패");
        }
    }

}
