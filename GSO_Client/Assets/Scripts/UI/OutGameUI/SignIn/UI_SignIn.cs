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
using NPOI.Util;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.IO;

public class SignInUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mResultText;

    [SerializeField]
    private Button          mSingInButton;

    private void Awake()
    {
        mSingInButton.onClick.AddListener(OnClickSignIn);
        mSingInButton.gameObject.SetActive(false);
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
            mSingInButton.gameObject.SetActive(true);
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
            ResultMessage($"���� �÷��� ���� ���� ����");

            string userId = PlayGamesPlatform.Instance.GetUserId();
            AuthenticationReq packet = new AuthenticationReq()
             {
                 user_id = userId,
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
            mSingInButton.gameObject.SetActive(true);
            ResultMessage("���� ���� ������ �߻��Ͽ����ϴ�.");
        }
        else if (status == SignInStatus.Canceled)
        {
            //����Ϸ� ������ ���� ���
            mSingInButton.gameObject.SetActive(true);
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

            var header = new HeaderCheackVersion
            {
                app = "1.0.0",
                data = "1.0.0",
            };

            var body = new SignInReq()
            {
                user_id = PlayGamesPlatform.Instance.GetUserId(),
                server_code = code,
                service = "Google"
            };

            GsoWebService service = new GsoWebService();
            SingInRequest request = service.mAuthorizeResource.GetSignInRequest(header, body);
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

        BetterStreamingAssets.Initialize();
        string[] files = BetterStreamingAssets.GetFiles("/", "*.xlsx", SearchOption.AllDirectories);
        ExcelReader.CopyExcel(files);
        //WebClientService �� �־��ֱ�
        //UserData�� ���������� ��� ���� ��
        {
            Managers.Web.Models.Credential = response.credential;
            Managers.Web.Models.Data = response.userData;
            Managers.Web.Models.DailyData = response.DailyLoads;
        }

        //�κ�� �̵�
        {
            Managers.Scene.LoadScene(Define.Scene.Shelter);
        }

    }

    private void ResultMessage(string message)
    {
        mResultText.text = message;
        Managers.SystemLog.Message(message);
    }
    
}
