using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using TMPro;
using static AuthorizeResource;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net.Http;

public class SignInUI : MonoBehaviour
{
    [SerializeField]
    private GameObject      mLoading;

    [SerializeField]
    private TextMeshProUGUI mResultText;

    [SerializeField]
    private Button          mSingInButton;

    private void Awake()
    {
        mSingInButton.onClick.AddListener(OnClickSignIn);
        SetActiveLoading(false);
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
        ResultMessage("���� �÷��� ���� �ڵ� ���� �õ�...");
        SetActiveLoading(true);
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// �ڵ� �α����� �ȵǾ��� ��� (���� ��Ȳ or �׽�Ʈ)
    /// �̰͵� ���� ������ �ƴ϶� �Ⱦ�
    /// </summary>
    public void OnClickSignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
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
                ResultMessage($"���� Ȯ�� ���� : {error}");
            }

        }
        else if (status == SignInStatus.InternalError)
        {
            //���� �÷��� �α��� ����
            ResultMessage("���� ���� ������ �߻��Ͽ����ϴ�.");
            SetActiveLoading(false);
        }
        else if (status == SignInStatus.Canceled)
        {
            //����Ϸ� ������ ���� ���
            ResultMessage("������ ��� �Ǿ����ϴ�.");
            SetActiveLoading(false);
        }

    }

    /// <summary>
    /// ������ �ִ� �������� Ȯ���� �����ڵ� ��û
    /// </summary>
    private void OnProcessAuthentication(AuthenticationRes response)
    {

        ResultMessage("���� �÷��� ���� �ڵ� ��û...");

        //������ �ִ� ������� False
        //���ο� ������� True
        bool forceRefreshToken = (response.error_code != 0);

        //��ū�� ������� ��ȸ�� Code�޾ƿ���
        PlayGamesPlatform.Instance.RequestServerSideAccess(forceRefreshToken, ProcessServerAuthCode);
    }

    /// <summary>
    /// �� ���� �α��� ��û
    /// </summary>
    private void ProcessServerAuthCode(string code)
    {
        if(code is null)
        {
            ResultMessage("���� �÷��� ���� �ڵ� ��û ����");
            SetActiveLoading(false);
            return;
        }

        try
        {
            ResultMessage("�α��� ��û �õ�...");

            SignInReq packet = new SignInReq()
            {
                user_id = PlayGamesPlatform.Instance.GetUserId(),
                server_code = code,
                service = "Google"
            };

            GsoWebService service = new GsoWebService();
            SingInRequest request = service.mAuthorizeResource.GetSignInRequest(packet);
            request.ExecuteAsync(ProcessAccessToken);
        }
        catch (HttpRequestException error)
        {
            ResultMessage($"������ ��û ���� : {error}");
        }

    }

    /// <summary>
    /// �� ���� �α��� ����
    /// </summary>
    private void ProcessAccessToken(SignInRes response)
    {

        if (response.error_code != 0)
        {
            ResultMessage($"�α��� ��û ���� : {response.error_description}");
            SetActiveLoading(false);
            return;
        }
        ResultMessage("�α��� ��û ����");

        //WebClientService �� �־��ֱ�
        //UserData�� ���������� ��� ���� ��
        {
            WebManager.Instance.mCredential = new WebClientCredential
            {
                uid = response.uid.ToString(),
                access_token = response.access_token,
                expires_in = response.expires_in,
                scope = response.scope,
                token_type = response.token_type
            };

            WebManager.Instance.mUserInfo = response.userData;
        }

        //�κ�� �̵�
        {
            SceneManager.LoadSceneAsync("LobbyScene");
        }

    }

    private void ResultMessage(string message)
    {
        mResultText.text = message;
        SystemLogManager.Instance.LogMessage(message);
    }
    
    private void SetActiveLoading(bool active)
    {
        mLoading.SetActive(active);
    }

}
