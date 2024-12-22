using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using Google.Protobuf.Protocol;
using TMPro;
using Ubiety.Dns.Core;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : BaseScene
{
    public enum EConnectMode
    {
        LAN,    //자신의 컴퓨터에 서버를 열은 경우
        WAN,    //다른 사람 서버에 접속하고 싶은 경우
        AWS     //AWS 서버에 접속하고 싶은 경우
    }
    public EConnectMode ConnectMode = EConnectMode.LAN;

    public string Ip = "";

    [SerializeField]
    public Button PlayButton;

    protected override void Init()
    {
        base.Init();

        string[] files = null;

#if UNITY_EDITOR
        files = null;

#elif UNITY_ANDROID
        BetterStreamingAssets.Initialize();
        files = BetterStreamingAssets.GetFiles("/", "*.xlsx", SearchOption.AllDirectories);
#endif 

        switch (ConnectMode)
        {
            case EConnectMode.LAN:
#if UNITY_EDITOR
                Ip = "127.0.0.1";

#elif UNITY_ANDROID
                Ip = "10.0.2.2";
#endif
                break;
            case EConnectMode.WAN:
                Ip = "113.60.249.123";
                break;
            case EConnectMode.AWS:
                Ip = "13.209.221.183";
                break;
            default:

                break;
        }

        PlayButton.interactable = false;

        ExcelReader.CopyExcel(files);
        SceneType = Define.Scene.Lobby;
        Screen.SetResolution(1920, 1080, false);

        Managers.Instance.StartCoroutine(WaitForConnection());

    }

    IEnumerator WaitForConnection()
    {
        const int MaxRetryConnect = 10;
        int retryCount = 0;
        while (retryCount < MaxRetryConnect)
        {

            yield return new WaitForSeconds(2.0f);

            retryCount++;
            if(false == Managers.Network.SettingConnection(Ip, 7777, "SomeConnectionKey"))
            {
                Managers.SystemLog.Message($"서버 접속에 실패하여 재시도 합니다. {MaxRetryConnect - retryCount}");
                PlayButton.interactable = false;
            }
            else
            {
                Managers.SystemLog.Message("서버 접속에 성공하였습니다.");
                PlayButton.interactable = true;
                break;
            }
        }
    }

    public override void Clear()
    {
    }

    //IEnumerator testc()
    //{
    //    while (true)
    //    {
    //        Debug.Log(Time.time);
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}

    /*public void DataUpdate(S_LobbyPlayerInfo lobbyPlayerInfo)
    {
        var profileTexts = Profile.GetComponentsInChildren<TextMeshProUGUI>();
        profileTexts.First(t => t.name == "PlayerName").text = lobbyPlayerInfo.Profile.Name; //PlayerName


        var temp = lobbyPlayerInfo.Profile.Coins.ToList();
        var coinTexts = Coins.GetComponentsInChildren<TextMeshProUGUI>();
        if (temp.Count != coinTexts.Count())
            return;
        for (var i = 0; i < temp.Count; i++) coinTexts[i].text = temp[i].ToString();

        Debug.Log(lobbyPlayerInfo.Profile.CharacterID);
    }*/

    public void GamePlay()
    {
        //Managers.Scene.LoadScene(Define.Scene.Forest);

        Managers.SystemLog.Message("서버 참여 요청");

        C_JoinServer c_JoinServer = new C_JoinServer();
        c_JoinServer.Name = "Player";
        /*c_JoinServer.Credential = new CredentiaInfo()
        {
            Uid = credential.uid,
        };*/
        Managers.Network.Send(c_JoinServer);

        //C_JoinServer c_JoinServer = new C_JoinServer();
        //c_JoinServer.Name = "jish";
        ////c_JoinServer.credential
        //Managers.Network.Send(c_JoinServer);

        //Debug.Log("Send C_JoinServer in LobbyScene"); 

        /*
        C_EnterGame c_EnterGame = new C_EnterGame();
        c_EnterGame.Name = "jish";

        Managers.Network.Send(c_EnterGame);
        Debug.Log("Send c_EnterGame");*/

    }
}