using GooglePlayGames;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.Error;
using static AuthorizeResource;

public class UI_SingOut : MonoBehaviour
{
    [SerializeField]
    private Button singOutBtn;

    private void Awake()
    {
        singOutBtn.onClick.AddListener(OnClickSingOut);
    }

    private void OnClickSingOut()
    {
        var header = new HeaderVerfiyPlayer
        {
            uid = Managers.Web.mCredential.uid,
            access_token = Managers.Web.mCredential.access_token,
        };

        var packet = new SignOutReq()
        {
            cause = "게임 나가기"
        };

        GsoWebService service = new GsoWebService();
        SignOutRequest request = service.mAuthorizeResource.GetSignOutRequest(header, packet);
        request.ExecuteAsync(OnProcessSingOut);
    }

    private void OnProcessSingOut(SignOutRes response)
    {

        if (response.error_code == WebErrorCode.None)
        {
            Application.Quit();
        }

    }

}
