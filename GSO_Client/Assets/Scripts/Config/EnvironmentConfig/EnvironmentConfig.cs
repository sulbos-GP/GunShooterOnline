using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnvironmentState
{
    Standard,       //���÷� ����Ƽ���� �ٷ� ����    (���� ������ Ű�� �׽�Ʈ)
    Emulator,       //���÷� ���� �����            (�� ����, ���� ���� Ű�� �׽�Ʈ)  
    Android,        //AWS�� ���� �Ǵ� �ȵ���̵忡�� ����          
    Release,        //��� ���� (�ӽ� ������ �ּ�)
}

[CreateAssetMenu(fileName = "EnvironmentConfig", menuName = "Configuration/EnvironmentConfig")]
public class EnvironmentConfig : ScriptableObject
{
    //���� ����
    public string GameServerIp = null;
    public string GameServerPort = null;

    //�� ����
    public string AuthorizationBaseUri = null;          //���� ����
    public string CenterBaseUri = null;                 //�߾� ����
    public string MatchmakerBaseUri = null;             //��ġ����Ŀ ����
    public string GameServerManagerBaseUri = null;      //���� �Ŵ��� ����
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

        SystemLogManager.Instance.LogMessage($"Enviroment setting [{state.ToString()}]");

        SystemLogManager.Instance.LogMessage($"Enviroment setting [{config.CenterBaseUri}]");
    }

    public EnvironmentConfig GetEnvironmentConfig()
    {
        return config;
    }
}