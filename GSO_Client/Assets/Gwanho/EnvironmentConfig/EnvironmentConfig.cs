using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnvironmentState
{
    Standard,       //���÷� ����Ƽ���� �ٷ� ����    (���� ������ Ű�� �׽�Ʈ)
    Emulator,       //���÷� ���� �����            (�� ����, ���� ���� Ű�� �׽�Ʈ)  
    Android,        //AWS�� ���� �Ǵ� �ȵ���̵忡�� ����          
    Producation,    //��� ���� (�ӽ� ������ �ּ�)
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
                config = Resources.Load<EnvironmentConfig>("Standard");
                break;
            case EEnvironmentState.Emulator:
                config = Resources.Load<EnvironmentConfig>("Emulator");
                break;
            case EEnvironmentState.Android:
                config = Resources.Load<EnvironmentConfig>("Android");
                break;
            case EEnvironmentState.Producation:
                config = Resources.Load<EnvironmentConfig>("Producation");
                break;
            default:
                break;
        }
    }

    public EnvironmentConfig GetEnvironmentConfig()
    {
        return config;
    }
}