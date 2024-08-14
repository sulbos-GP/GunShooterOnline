using System.Linq;
using System.Net;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public bool DebugMode;

    [SerializeField] private GameObject Store;

    [SerializeField] private GameObject Profile;

    [SerializeField] private GameObject Coins;


    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Forest;
        Screen.SetResolution(1920, 1080, false);
        Debug.Log("신 초기화 로비");
        //Managers.Network.ConnectToGame("ec2-3-36-85-125.ap-northeast-2.compute.amazonaws.com");

        Managers.Network.SettingConnection("127.0.0.1",7777, "SomeConnectionKey");
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
        Managers.Scene.LoadScene(Define.Scene.Forest);



        C_EnterGame c_EnterGame = new C_EnterGame();
        c_EnterGame.Name = "jish";

        Managers.Network.Send(c_EnterGame);
        Debug.Log("Send c_EnterGame");

    }
}