using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AuthorizeResource;

public class WebManager : MonoBehaviour
{
    private static WebManager instance = null;

    public WebClientCredential mCredential;

    //�ӽ�
    public DataLoadUserInfo mUserInfo;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        mUserInfo = new DataLoadUserInfo();
        mCredential = new WebClientCredential();
    }

    public static WebManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {

    }

    /// <summary>
    /// Application ó�� ���۽� ���ְ� ������ ������ ����
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        // Application ó�� ���۽� (True)
        // ��Ż False, ���� True
    }

    /// <summary>
    /// Application �߰��� ��Ż �Ǵ� ���� ����
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        //����
        if(pause)
        {

        }
        //��Ż
        else
        {

        }
    }

    /// <summary>
    /// Application�� ����Ǿ��� ���
    /// </summary>
    private void OnApplicationQuit()
    {
        SignOutReq packet = new SignOutReq()
        {
            //TODO : �������� �Ͼ �۾��� ����
            cause = "Player OnApplicationQuit"
        };

        GsoWebService service = new GsoWebService();
        SignOutRequest request = service.mAuthorizeResource.GetSignOutRequest(packet);
        request.ExecuteAsync(ProcessSignOut);
    }

    private void ProcessSignOut(SignOutRes response)
    {

    }

    private void OnDestroy()
    {

    }

}
