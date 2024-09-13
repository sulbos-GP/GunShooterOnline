using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDB;

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
    protected int retry = 1;

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
        if(retry++ < 3)
        {
            Managers.Instance.StartCoroutine(CoExecuteAsync(mAction));
        }
    }

    private IEnumerator CoExecuteAsync(Action<TResponse> callback)
    {

        Managers.SystemLog.Message($"웹 요청 [{mEndPoint}]");

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

        if (request.result == UnityWebRequest.Result.Success)
        {
            TResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);
            callback.Invoke(response);
        }
        else if (request.responseCode == 401)
        {
            //토큰 다시 발행
            RefreshTokenRes response = Newtonsoft.Json.JsonConvert.DeserializeObject<RefreshTokenRes>(request.downloadHandler.text);
            OnProcessRefreshToken(response);
        }
        else if (request.responseCode == 426)
        {
            //버전 업그레이드 필요
        }
        else
        {
            Managers.SystemLog.Message($"웹 요청 실패 : {request.error}");
            throw new NotImplementedException($"웹 요청[{mEndPoint}] 실패 : {request.error}");
        }

    }

    public void OnProcessRefreshToken(RefreshTokenRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {

            Managers.Web.Models.Credential = new ClientCredential
            {
                uid = response.uid,
                access_token = response.access_token,
                expires_in = response.expires_in,
                scope = response.scope,
                token_type = response.token_type,
            };

            ReExecuteAsync();
        }
    }

    public void OnProcessUpgradeVersion(UpgradeVersionRes response)
    {
        if (response.error_code == WebErrorCode.None)
        {
            
            
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
