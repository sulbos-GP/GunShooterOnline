using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum ERequestMethod
{
    None,
    GET,
    POST
}
public abstract class WebClientServiceRequest<TResponse>
{
    protected Dictionary<string, string> mFromHeader = null;
    protected object mFromBody = null;
    protected string mEndPoint = null;
    protected ERequestMethod mMethod = ERequestMethod.None;
    protected Action<TResponse> mAction = null;

    public void ExecuteAsync(Action<TResponse> callback)
    {
        CheckNotNull(callback);
        mAction = callback;

        CheckNotNull(mFromBody);

        CheckNotNull(mEndPoint);

        CheckMethodNotNone(mMethod);

        Managers.Instance.StartCoroutine(CoExecuteAsync(callback));
    }

    private void ReExecuteAsync()
    {
        Managers.Instance.StartCoroutine(CoExecuteAsync(mAction));
    }

    private IEnumerator CoExecuteAsync(Action<TResponse> callback)
    {

        SystemLogManager.Instance.LogMessage($"웹 요청 [{mEndPoint}]");

        Managers.Web.GetUI().Show(EWebUI.Loading);

        string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(mFromBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);

        UnityWebRequest request = new UnityWebRequest(mEndPoint, mMethod.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (mFromHeader != null)
        {
            foreach (var (key, value) in mFromHeader)
            {
                request.SetRequestHeader(key, value);
            }
        }

        yield return request.SendWebRequest();

        Managers.Web.GetUI().Hide(EWebUI.Loading);

        if (request.result == UnityWebRequest.Result.Success)
        {
            TResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);
            callback.Invoke(response);
        }
        else if (request.responseCode == 401)
        {
            RequestRefreshToken();
        }
        else
        {
            SystemLogManager.Instance.LogMessage($"웹 요청 실패 : {request.error}");
            throw new NotImplementedException($"웹 요청 실패 : {request.error}");
        }

    }

    public void RequestRefreshToken()
    {
        var body = new RefreshTokenReq()
        {
            uid = Convert.ToInt32(Managers.Web.mCredential.uid),
        };

        GsoWebService service = new GsoWebService();
        var request = service.mAuthorizeResource.GetRefreshTokenRequest(body);
        request.ExecuteAsync(OnProcessRefreshToken);
    }

    public void OnProcessRefreshToken(RefreshTokenRes response)
    {
        if (response.error_code == 0)
        {
            Managers.Web.mCredential.access_token = response.access_token;

            ReExecuteAsync();
        }
        else
        {
            throw new NotImplementedException($"토큰 재인증 실패");
        }
    }

    public void CheckNotNull<T>(T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("Request method is none");
        }
    }

    private void CheckMethodNotNone(ERequestMethod method)
    {
        if (method == ERequestMethod.None)
        {
            throw new ArgumentNullException("Request method is none");
        }
    }

}
