using GooglePlayGames;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
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
        ClientCredential crediential = Managers.Web.Models.Credential;
        if (crediential == null)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
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
