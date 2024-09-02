using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using TMPro;
using static AuthorizeResource;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net.Http;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.Error;

public class SignInUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mResultText;

    [SerializeField]
    private Button          mSingInButton;

    private void Awake()
    {
        mSingInButton.onClick.AddListener(OnClickSignIn);
    }

    void Start()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        AutoSignIn();
    }

    /// <summary>
    /// �ڵ� �α��� (����)
    /// </summary>
    private void AutoSignIn()
    {
        try
        {
            ResultMessage("���� �÷��� ���� �ڵ� ���� �õ�...");
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        catch (Exception error)
        {
            ResultMessage($"�ڵ� ���� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// �ڵ� �α����� �ȵǾ��� ��� (���� ��Ȳ or �׽�Ʈ)
    /// �̰͵� ���� ������ �ƴ϶� �Ⱦ�
    /// </summary>
    public void OnClickSignIn()
    {
        try
        {
            ResultMessage("���� �÷��� ���� ���� ���� �õ�...");
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
        }
        catch (Exception error)
        {
            ResultMessage($"���� ���� ���� : {error.Message}");
        }
    }

    /// <summary>
    /// ���� �α׾ƿ� (������)
    /// </summary>
    private void OnClickSignOut()
    {
        //������
    }

    /// <summary>
    /// ���� ó��
    /// </summary>
    private void ProcessAuthentication(SignInStatus status)
    {

        if (status == SignInStatus.Success)
        {
            ResultMessage("���� �÷��� ���� ���� ����");

            AuthenticationReq packet = new AuthenticationReq()
            {
                user_id = PlayGamesPlatform.Instance.GetUserId(),
                service = "Google"
            };

            try
            {

                ResultMessage("���� Ȯ�� ��û...");

                GsoWebService service = new GsoWebService();
                AuthenticationRequest request = service.mAuthorizeResource.GetAuthenticationRequest(packet);
                request.ExecuteAsync(OnProcessAuthentication);
            }
            catch (HttpRequestException error)
            {
                ResultMessage($"���� Ȯ�� ���� : {error.Message}");
            }

        }
        else if (status == SignInStatus.InternalError)
        {
            //���� �÷��� �α��� ����
            ResultMessage("���� ���� ������ �߻��Ͽ����ϴ�.");
        }
        else if (status == SignInStatus.Canceled)
        {
            //����Ϸ� ������ ���� ���
            ResultMessage("������ ��� �Ǿ����ϴ�.");
        }

    }

    /// <summary>
    /// ������ �ִ� �������� Ȯ���� �����ڵ� ��û
    /// </summary>
    private void OnProcessAuthentication(AuthenticationRes response)
    {

        try
        {
            ResultMessage("���� �÷��� ���� �ڵ� ��û...");

            //������ �ִ� ������� False
            //���ο� ������� True
            bool forceRefreshToken = (response.error_code != WebErrorCode.None);

            //��ū�� ������� ��ȸ�� Code�޾ƿ���
            PlayGamesPlatform.Instance.RequestServerSideAccess(forceRefreshToken, ProcessServerAuthCode);
        }
        catch (Exception error)
        {
            ResultMessage($"���� �ڵ� ��û ���� : {error.Message}");
        }
    }

    /// <summary>
    /// �� ���� �α��� ��û
    /// </summary>
    private void ProcessServerAuthCode(string code)
    {
        if(code is null)
        {
            ResultMessage("���� �÷��� ���� �ڵ� ��û ����");
            return;
        }

        try
        {
            ResultMessage("�α��� ��û �õ�...");

            var body = new SignInReq()
            {
                user_id = PlayGamesPlatform.Instance.GetUserId(),
                server_code = code,
                service = "Google"
            };

            GsoWebService service = new GsoWebService();
            SingInRequest request = service.mAuthorizeResource.GetSignInRequest(body);
            request.ExecuteAsync(ProcessAccessToken);
        }
        catch (HttpRequestException error)
        {
            ResultMessage($"�α��� ��û ���� : {error.Message}");
        }

    }

    /// <summary>
    /// �� ���� �α��� ����
    /// </summary>
    private void ProcessAccessToken(SignInRes response)
    {

        if (response.error_code != WebErrorCode.None)
        {
            ResultMessage($"�α��� ��û ���� : {response.error_description}");
            return;
        }
        else
        {
            ResultMessage("�α��� ��û ����");
        }

        //WebClientService �� �־��ֱ�
        //UserData�� ���������� ��� ���� ��
        {
            Managers.Web.mCredential = new WebClientCredential
            {
                uid = Convert.ToString(response.uid),
                access_token = response.access_token,
                expires_in = response.expires_in,
                scope = response.scope,
                token_type = response.token_type
            };

            Managers.Web.mUserInfo = response.userData;
        }

        //�κ�� �̵�
        {
            SceneManager.LoadSceneAsync("Shelter");
        }

    }

    private void ResultMessage(string message)
    {
        mResultText.text = message;
        SystemLogManager.Instance.LogMessage(message);
    }
    
}
