using UnityEngine;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static AuthorizeResource;

public class WebManager
{
    private ClientCredential credential = new ClientCredential();
    public DataLoadUserInfo user = new DataLoadUserInfo();

    private void Awake()
    {

    }

    public ClientCredential Credential
    {
        get
        {
            if(credential.uid == 0 || credential.access_token == string.Empty)
            {
                return null;
            }

            return credential;
        }
        set
        {
            credential = value;
        }
    }

    /// <summary>
    /// Application이 종료되었을 경우
    /// </summary>
    /// <param name="cause">마지막에 일어난 작업을 적음</param>
    public void OnSignOut(string cause)
    {
        ClientCredential crediential = Managers.Web.credential;
        if(crediential.uid == 0 || crediential.access_token == string.Empty)
        {
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new SignOutReq()
        {
            cause = cause
        };

        GsoWebService service = new GsoWebService();
        SignOutRequest request =service.mAuthorizeResource.GetSignOutRequest(header, body);
        request.ExecuteAsync(ProcessSignOut);
    }

    private void ProcessSignOut(SignOutRes response)
    {
        if(response.error_code != WebErrorCode.None)
        {
            Application.Quit();
        }
    }
}
