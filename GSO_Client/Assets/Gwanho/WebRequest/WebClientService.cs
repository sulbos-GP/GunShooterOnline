using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

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

public abstract class WebClientService
{
    public string mBaseUrl { get; set; }
}

public abstract class WebClientServiceRequest<TResponse>
{
    protected Dictionary<string, string> mFromHeader = null;
    protected object mFromBody = null;
    protected string mEndPoint = null;
    protected ERequestMethod mMethod = ERequestMethod.None;

    public void ExecuteAsync(Action<TResponse> callback)
    {
        CheckNotNull(callback);

        CheckNotNull(mFromBody);

        CheckNotNull(mEndPoint);

        CheckMethodNotNone(mMethod);

        Managers.Instance.StartCoroutine(CoExecuteAsync(callback));
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
            foreach(var (key, value) in mFromHeader)
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
        else
        {
            SystemLogManager.Instance.LogMessage($"웹 요청 실패 : {request.error}");
            throw new NotImplementedException($"웹 요청 실패 : {request.error}");
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
