using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnvironmentState
{
    Standard,       //로컬로 유니티에서 바로 실행    (게임 서버만 키고 테스트)
    Emulator,       //로컬로 에뮬 실행시            (웹 서버, 게임 서버 키고 테스트)  
    Android,        //AWS로 에뮬 또는 안드로이드에서 실행          
    Release,        //출시 버전 (임시 승현이 주소)
}

[CreateAssetMenu(fileName = "EnvironmentConfig", menuName = "Configuration/EnvironmentConfig")]
public class EnvironmentConfig : ScriptableObject
{
    public EEnvironmentState state = EEnvironmentState.Standard;
    //소켓 서버
    public string GameServerIp = null;
    public string GameServerPort = null;

    //웹 서버
    public string AuthorizationBaseUri = null;          //인증 서버
    public string CenterBaseUri = null;                 //중앙 서버
    public string MatchmakerBaseUri = null;             //매치메이커 서버
    public string GameServerManagerBaseUri = null;      //게임 매니저 서버

    public string LobbyName = null;                     //로비 이름
}

public class EnvironmentSetting
{

    private EnvironmentConfig config;

    public void InitEnviromentSetting(EEnvironmentState state)
    {
        switch (state)
        {
            case EEnvironmentState.Standard:
                config = Resources.Load<EnvironmentConfig>("Environment/Standard");
                break;
            case EEnvironmentState.Emulator:
                config = Resources.Load<EnvironmentConfig>("Environment/Emulator");
                break;
            case EEnvironmentState.Android:
                config = Resources.Load<EnvironmentConfig>("Environment/Android");
                break;
            case EEnvironmentState.Release:
                config = Resources.Load<EnvironmentConfig>("Environment/Release");
                break;
            default:
                break;
        }
        config.state = state;
    }

    public EnvironmentConfig GetEnvironmentConfig()
    {
        return config;
    }
}