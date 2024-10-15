using System.IO;
using System.Linq;
using System.Net;
using Google.Protobuf.Protocol;
using TMPro;
using Ubiety.Dns.Core;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public bool DebugMode;

    [SerializeField] private GameObject Store;

    [SerializeField] private GameObject Profile;

    [SerializeField] private GameObject Coins;



    // public string Ip = "127.0.0.1";
    public string Ip = "10.0.2.2";

    protected override void Init()
    {
        base.Init();
#if UNITY_EDITOR
        Ip = "127.0.0.1";
        string[] files = null;

#elif UNITY_ANDROID
        Ip = "10.0.2.2";
         BetterStreamingAssets.Initialize();
        string[] files = BetterStreamingAssets.GetFiles("/", "*.xlsx", SearchOption.AllDirectories);
#endif
        if(DebugMode)
            Ip = "113.60.249.123";

        ExcelReader.CopyExcel(files);
        SceneType = Define.Scene.Lobby;
        Screen.SetResolution(1920, 1080, false);
        Debug.Log("신 초기화 로비");
        //Managers.Network.ConnectToGame("ec2-3-36-85-125.ap-northeast-2.compute.amazonaws.com");

        Managers.Network.SettingConnection(Ip, 7777, "SomeConnectionKey");
        //Managers.Network.ConnectToGame();
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



        C_JoinServer c_JoinServer = new C_JoinServer();
        c_JoinServer.Name = "jish";
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