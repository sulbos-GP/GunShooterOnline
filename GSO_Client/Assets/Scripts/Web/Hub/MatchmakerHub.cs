using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
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
        string name = "��ġ����Ŀ";
        string url = Managers.EnvConfig.GetEnvironmentConfig().MatchmakerBaseUri + "/MatchmakerHub";

        this.SetHub(name, url);
    }

    protected override void SetOnRecivedFunc()
    {
        mConnection.On<string, MatchProfile>("S2C_MatchSuccess", S2C_MatchSuccess);
        mConnection.On<WebErrorCode>("S2C_MatchFailed", S2C_MatchFailed);
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
    /// ��Ī ����
    /// </summary>
    public void S2C_MatchSuccess(string client_id, MatchProfile response)
    {
        EnqueueDispatch(() =>
        {
            Managers.SystemLog.Message($"��ġ�� �����Ǿ����ϴ� {response.host_ip}:{response.host_port}");

            if (matchStateUI == null)
            {
                return;
            }
            UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

            state.SetStateText("��Ī �Ϸ�");
            state.StopTiemr();
            Destroy(matchStateUI);

            {
                if(Managers.EnvConfig.GetEnvironmentConfig().state == EEnvironmentState.Release)
                {
                    response.host_ip = "113.60.249.123";
                }
                else
                {
                    response.host_ip = "10.0.2.2";
                }

                Managers.Network.SettingConnection(response.host_ip, response.host_port, response.container_id);

                Managers.Scene.LoadScene(Define.Scene.Forest);

                ClientCredential credential = Managers.Web.Models.Credential;
                if (credential == null)
                {
                    return;
                }

                /* //Enter�Ҷ� uid �����ּ�
                 C_EnterGame c_EnterGame = new C_EnterGame();
                 //c_EnterGame.Name = "jish";
                 c_EnterGame.Credential = new CredentiaInfo()
                 {
                     Uid = credential.uid,
                 };

                 Managers.Network.Send(c_EnterGame);

                 */


                C_JoinServer c_JoinServer = new C_JoinServer();
                c_JoinServer.Name = "jish";
                c_JoinServer.Credential = new CredentiaInfo()
                {
                    Uid = credential.uid,
                };
                Managers.Network.Send(c_JoinServer);


                Debug.Log("������");
            }

            //Managers.Network.ConnectToGame(response.host_ip);
        });
    }

    /// <summary>
    /// ��Ī ���� �Ǵ� ���� ��� ����
    /// </summary>
    public void S2C_MatchFailed(WebErrorCode error)
    {
        EnqueueDispatch(() =>
        {
            Managers.SystemLog.Message($"��ġ�� ��ҵǾ����ϴ� {error.ToString()}");

            if (matchStateUI == null)
            {
                return;
            }
            UI_MatchState state = matchStateUI.GetComponentInChildren<UI_MatchState>();

            if(error == WebErrorCode.PopPlayersExitSuccess)
            {
                state.SetStateText("��Ī ��� �Ϸ�");
                Destroy(matchStateUI);
            }
            else if(error == WebErrorCode.PopPlayersJoinForced)
            {
                //��Ī�� ����Ϸ��� ������ �̹� ������ �������
            }
            else
            {

            }

        });
    }

    /// <summary>
    /// ��Ī ���� ��û
    /// </summary>
    public void OnJoinRequest(string name)
    {
        if(matchStatePrefab != null)
        {
            matchStatePrefab = Resources.Load<GameObject>("Prefabs/UI/Lobby/MatchState");
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
            state.SetStateText("��Ī ���� ��...");
            mIsProcessing = true;

            ClientCredential credential = Managers.Web.Models.Credential;
            if (credential == null)
            {
                return;
            }

            var header = new HeaderVerfiyPlayer
            {
                uid = credential.uid.ToString(),
                access_token = credential.access_token,
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
            state.SetStateText($"��Ī ���� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// ��Ī ���� ����
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

            state.SetStateText("��Ī ��û ����");

            state.StartTimer();

            state.SetStateText("���� ��Ī ��...");
        }
        else
        {
            state.SetStateText($"��Ī ���� ��û ���� : {response.error_code.ToString()}");
            Destroy(matchStateUI);
        }
    }

    /// <summary>
    /// ��Ī ��� ��û
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
            state.SetStateText("��Ī ��� ��...");
            mIsProcessing = true;

            ClientCredential credential = Managers.Web.Models.Credential;
            if (credential == null)
            {
                return;
            }

            var header = new HeaderVerfiyPlayer
            {
                uid = credential.uid.ToString(),
                access_token = credential.access_token,
            };

            var body = new CancleMatchReq
            {
                world = world,
                region = region
            };

            MatchmakerService service = new MatchmakerService();
            MatchCancleRequest request = service.mMatchmakerResource.GetMatchCancleRequest(header, body);
            request.ExecuteAsync(OnProcessMatchCancle);
        }
        catch (HttpRequestException error)
        {
            mIsProcessing = false;
            state.SetStateText($"��Ī ��� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// ��Ī ��� ����
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
            state.SetStateText("��Ī ��� ��û ����");
            state.StopTiemr();
            Destroy(matchStateUI);
        }
        else if(response.error_code == WebErrorCode.PopPlayersExitRequested)
        {
            //��Ī ť���� ���� �ɷ��־� ��û�� �صΰ� ���� ��Ȳ
            state.SetStateText("��Ī ��� ��ٸ��� ��...");
        }
        else
        {
            Managers.SystemLog.Message("��Ī ��� ��û ����");
            //Destroy(matchStateUI);
        }
    }

}
