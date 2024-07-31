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

    //임시
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
    /// Application 처음 시작시 해주고 싶은거 있으면 여기
    /// </summary>
    private void OnApplicationFocus(bool focus)
    {
        // Application 처음 시작시 (True)
        // 이탈 False, 복귀 True
    }

    /// <summary>
    /// Application 중간에 이탈 또는 복귀 여부
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        //복귀
        if(pause)
        {

        }
        //이탈
        else
        {

        }
    }

    /// <summary>
    /// Application이 종료되었을 경우
    /// </summary>
    private void OnApplicationQuit()
    {
        SignOutReq packet = new SignOutReq()
        {
            //TODO : 마지막에 일어난 작업을 적음
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
