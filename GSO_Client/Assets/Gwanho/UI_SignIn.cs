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
    /// 자동 로그인 (권장)
    /// </summary>
    private void AutoSignIn()
    {
        mLoadingBackground.enabled = true;
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    /// <summary>
    /// 자동 로그인이 안되었을 경우 (에러 상황)
    /// </summary>
    public void OnClickSignIn()
    {
        mLoadingBackground.enabled = true;
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
            //토큰을 얻기위한 일회용 Code받아오기
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, ProcessServerAuthCode);
        }
        else if (status == SignInStatus.InternalError)
        {
            //구글 플레이 로그인 에러
            mLoadingBackground.enabled = false;
            mResultText.text = "An internal error occurred.";
        }
        else if (status == SignInStatus.Canceled)
        {
            //모바일로 실행을 안할 경우
            //TODO : PC에서 무시하고 진행할 수 있도록
            mLoadingBackground.enabled = false;
            mResultText.text = "The sign in was canceled.";
        }
    }

    /// <summary>
    /// 웹 서버 로그인 요청
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
    /// 웹 서버 로그인 응답
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

        //로비로 이동 (임시)
        //WebClientService 값 넣어주기
        //UserData는 지속적으로 들고 있을 것
        WebManager.Instance.mCredential = new WebClientCredential()
        {
            uid = response.uid,
            access_token = response.access_token,
            expires_in = response.expires_in,
            scope = response.scope,
            token_type = response.token_type
        };

        WebManager.Instance.mUserInfo = response.userData;

        //로비로 이동
        SceneManager.LoadScene("LobbyScene");

    }
}
