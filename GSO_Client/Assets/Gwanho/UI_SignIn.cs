using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static AuthorizeResource;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInUI : MonoBehaviour
{

    [SerializeField]
    private Image mLoadingBackground;

    [SerializeField]
    private TextMeshProUGUI mResultText;

    private void Awake()
    {
        mLoadingBackground.enabled = false;
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
        mLoadingBackground.enabled = true;
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// �ڵ� �α����� �ȵǾ��� ��� (���� ��Ȳ)
    /// </summary>
    public void OnClickSignIn()
    {
        mLoadingBackground.enabled = true;
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
            //��ū�� ������� ��ȸ�� Code�޾ƿ���
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, ProcessServerAuthCode);
        }
        else if (status == SignInStatus.InternalError)
        {
            //���� �÷��� �α��� ����
            mLoadingBackground.enabled = false;
            mResultText.text = "An internal error occurred.";
        }
        else if (status == SignInStatus.Canceled)
        {
            //����Ϸ� ������ ���� ���
            //TODO : PC���� �����ϰ� ������ �� �ֵ���
            mLoadingBackground.enabled = false;
            mResultText.text = "The sign in was canceled.";
        }
    }

    /// <summary>
    /// �� ���� �α��� ��û
    /// </summary>
    private void ProcessServerAuthCode(string code)
    {

        Debug.Log($"[ProcessServerAuthCode] : {code}");

        SignInReq packet = new SignInReq()
        {
            user_id = PlayGamesPlatform.Instance.GetUserId(),
            server_code = code,
            service = "Google"
        };

        GsoWebService service = new GsoWebService();
        AuthenticationRequest request = service.mAuthorizeResource.GetAuthenticationRequest(packet);
        request.ExecuteAsync(ProcessAccessToken);
    }

    /// <summary>
    /// �� ���� �α��� ����
    /// </summary>
    private void ProcessAccessToken(SignInRes response)
    {
        mLoadingBackground.enabled = false;

        if(response.error == 0)
        {
            mResultText.text = "The operation was successful.";
        }
        else
        {
            mResultText.text = "The operation was failure.";
        }

        //�κ�� �̵� (�ӽ�)
        //WebClientService �� �־��ֱ�
        //UserData�� ���������� ��� ���� ��
        WebManager.Instance.mCredential = new WebClientCredential()
        {
            uid = response.uid,
            access_token = response.access_token,
            expires_in = response.expires_in,
            scope = response.scope,
            token_type = response.token_type
        };

        WebManager.Instance.mUserInfo = response.userData;

        //�κ�� �̵�
        SceneManager.LoadScene("LobbyScene");

    }
}
