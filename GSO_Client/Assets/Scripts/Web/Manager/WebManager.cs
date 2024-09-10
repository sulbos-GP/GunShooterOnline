using UnityEngine;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.DataLoad;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;
using static AuthorizeResource;

public class WebManager
{
    public ClientCredential credential = new ClientCredential();
    public DataLoadUserInfo user = new DataLoadUserInfo();

    private void Awake()
    {

    }

    /// <summary>
    /// Application�� ����Ǿ��� ���
    /// </summary>
    /// <param name="cause">�������� �Ͼ �۾��� ����</param>
    public void OnSignOut(string cause)
    {
        ClientCredential crediential = Managers.Web.credential;
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
