using GooglePlayGames.OurUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public enum ERequestMethod
{
    None,
    GET,
    POST
}

public class WebClientCredential
{
    public string uid { get; set; } = string.Empty;

    public string access_token { get; set; } = string.Empty;

    public long expires_in { get; set; } = 0;

    public string scope { get; set; } = string.Empty;

    public string token_type { get; set; } = string.Empty;
}

public abstract class WebClientService : MonoBehaviour
{
    public string mBaseUrl { get; set; }

}

public abstract class WebClientServiceRequest<TResponse>
{
    protected HeaderDTO mFromHeader = null;
    protected object mFromBody = null;
    protected string mEndPoint = null;
    protected ERequestMethod mMethod = ERequestMethod.None;

    public void ExecuteAsync(Action<TResponse> callback)
    {
        Misc.CheckNotNull(callback);

        Misc.CheckNotNull(mFromBody);

        Misc.CheckNotNull(mEndPoint);

        CheckMethodNotNone(mMethod);

        WebManager.Instance.StartCoroutine(CoExecuteAsync(callback));
    }

    private IEnumerator CoExecuteAsync(Action<TResponse> callback)
    {
        string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(mFromBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);

        UnityWebRequest request = new UnityWebRequest(mEndPoint, mMethod.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if(mFromHeader != null)
        {
            request.SetRequestHeader("uid", mFromHeader.uid);
            request.SetRequestHeader("access_token", mFromHeader.access_token);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            TResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);
            callback.Invoke(response);
        }
        else
        {
            Debug.Log($"WebClientRequest failed: {request.error}");
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
