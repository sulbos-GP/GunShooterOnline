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
    /// 자동 로그인 (권장)
    /// </summary>
    private void AutoSignIn()
    {
        ResultMessage("구글 플레이 서비스 자동 인증 시도...");
        SetActiveLoading(true);
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// 자동 로그인이 안되었을 경우 (에러 상황 or 테스트)
    /// 이것도 권장 사항이 아니라서 안씀
    /// </summary>
    public void OnClickSignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }

    /// <summary>
    /// 수동 로그아웃 (미지원)
    /// </summary>
    private void OnClickSignOut()
    {
        //미지원
    }

    /// <summary>
    /// 인증 처리
    /// </summary>
    private void ProcessAuthentication(SignInStatus status)
    {

        if (status == SignInStatus.Success)
        {
            ResultMessage("구글 플레이 서비스 인증 성공");

            AuthenticationReq packet = new AuthenticationReq()
            {
                user_id = PlayGamesPlatform.Instance.GetUserId(),
                service = "Google"
            };

            try
            {

                ResultMessage("유저 확인 요청...");

                GsoWebService service = new GsoWebService();
                AuthenticationRequest request = service.mAuthorizeResource.GetAuthenticationRequest(packet);
                request.ExecuteAsync(OnProcessAuthentication);
            }
            catch (HttpRequestException error)
            {
                ResultMessage($"유저 확인 실패 : {error}");
            }

        }
        else if (status == SignInStatus.InternalError)
        {
            //구글 플레이 로그인 에러
            ResultMessage("인증 도중 에러가 발생하였습니다.");
            SetActiveLoading(false);
        }
        else if (status == SignInStatus.Canceled)
        {
            //모바일로 실행을 안할 경우
            ResultMessage("인증이 취소 되었습니다.");
            SetActiveLoading(false);
        }

    }

    /// <summary>
    /// 기존에 있던 유저인지 확인후 서버코드 요청
    /// </summary>
    private void OnProcessAuthentication(AuthenticationRes response)
    {

        ResultMessage("구글 플레이 서버 코드 요청...");

        //기존에 있던 유저라면 False
        //새로운 유저라면 True
        bool forceRefreshToken = (response.error_code != 0);

        //토큰을 얻기위한 일회용 Code받아오기
        PlayGamesPlatform.Instance.RequestServerSideAccess(forceRefreshToken, ProcessServerAuthCode);
    }

    /// <summary>
    /// 웹 서버 로그인 요청
    /// </summary>
    private void ProcessServerAuthCode(string code)
    {
        if(code is null)
        {
            ResultMessage("구글 플레이 서버 코드 요청 실패");
            SetActiveLoading(false);
            return;
        }

        try
        {
            ResultMessage("로그인 요청 시도...");

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
            ResultMessage($"르그인 요청 실패 : {error}");
        }

    }

    /// <summary>
    /// 웹 서버 로그인 응답
    /// </summary>
    private void ProcessAccessToken(SignInRes response)
    {

        if (response.error_code != 0)
        {
            ResultMessage($"로그인 요청 실패 : {response.error_description}");
            SetActiveLoading(false);
            return;
        }
        ResultMessage("로그인 요청 성공");

        //WebClientService 값 넣어주기
        //UserData는 지속적으로 들고 있을 것
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

        //로비로 이동
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
