using Google.Protobuf.Protocol;
using UnityEngine;

public class GameScene : BaseScene
{
    //UI_GameScene _sceneUI;
    public Canvas Canvas;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Forest;
        Canvas = FindObjectOfType<Canvas>();
        //Managers.Map.LoadMap(9);

        Screen.SetResolution(1980, 1080, true);

        //_sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Debug.Log("초기화");
        //Managers.Network.ConnectToGame();

    }

   

    public override void Clear()
    {
    }
}