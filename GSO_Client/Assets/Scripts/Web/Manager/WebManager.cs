using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AuthorizeResource;

public class WebManager
{

    private WebUIFactory mUIFactory;

    public WebClientCredential mCredential;

    //�ӽ�
    public DataLoadUserInfo mUserInfo;

    private void Awake()
    {
        mUserInfo = new DataLoadUserInfo();
        mCredential = new WebClientCredential();

        mUIFactory = Managers.Instance.gameObject.AddComponent<WebUIFactory>();
    }

    private void Start()
    {

    }

    public WebUIFactory GetUI()
    {
        return mUIFactory;
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

        var header = new HeaderVerfiyPlayer
        {
            uid = mCredential.uid,
            access_token = mCredential.access_token,
        };

        var body = new SignOutReq()
        {
            //TODO : �������� �Ͼ �۾��� ����
            cause = "Player OnApplicationQuit"
        };

        GsoWebService service = new GsoWebService();
        SignOutRequest request = service.mAuthorizeResource.GetSignOutRequest(header, body);
        request.ExecuteAsync(ProcessSignOut);
    }

    private void ProcessSignOut(SignOutRes response)
    {

    }

    private void OnDestroy()
    {

    }

}
