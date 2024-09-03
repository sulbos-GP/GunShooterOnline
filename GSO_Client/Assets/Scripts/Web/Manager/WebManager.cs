using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Model.GameDB;

public class WebManager
{

    private WebUIFactory mUIFactory;

    public ClientCredential credential;
    public DataLoadUserInfo user;

    private void Awake()
    {
        user = new DataLoadUserInfo();
        credential = new ClientCredential();

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

        //var header = new HeaderVerfiyPlayer
        //{
        //    uid = mCredential.uid,
        //    access_token = mCredential.access_token,
        //};

        //var body = new SignOutReq()
        //{
        //    //TODO : 마지막에 일어난 작업을 적음
        //    cause = "Player OnApplicationQuit"
        //};

        //GsoWebService service = new GsoWebService();
        //SignOutRequest request = service.mAuthorizeResource.GetSignOutRequest(header, body);
        //request.ExecuteAsync(ProcessSignOut);
    }

    private void ProcessSignOut(SignOutRes response)
    {

    }

    private void OnDestroy()
    {

    }

}
