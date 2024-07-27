using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��ġ����ŷ ���� ���� API
/// </summary>
public class MatchmakerService : WebClientService
{

    public MatchmakerResource mMatchmakerResource;

    public MatchmakerService()
    {
        this.mBaseUrl = "http://10.0.2.2:5200/api";
        mMatchmakerResource = new MatchmakerResource(this);
    }

}

/// <summary>
/// ��Ī ���� Resource
/// </summary>
public class MatchmakerResource
{
    private readonly MatchmakerService mService;

    public MatchmakerResource(MatchmakerService service)
    {
        mService = service;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// 

    public MatchJoinRequest GetMatchJoinRequest(AuthenticationReq packet)
    {
        return new MatchJoinRequest(this.mService, packet);
    }

    public MatchCancleRequest GetMatchCancleRequest(SignInReq packet)
    {
        return new MatchCancleRequest(this.mService, packet);
    }

    /// <summary>
    /// Requset
    /// </summary>

    //������ ���������� �ִ���?
    public class MatchJoinRequest : WebClientServiceRequest<AuthenticationRes>
    {
        public MatchJoinRequest(MatchmakerService service, AuthenticationReq request)
        {
            this.mFromHeader = new HeaderDTO
            {
                uid = WebManager.Instance.mCredential.uid,
                access_token = WebManager.Instance.mCredential.access_token,
            };

            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/Matchmaker/Join";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //�α��� ��û
    public class MatchCancleRequest : WebClientServiceRequest<SignInRes>
    {
        public MatchCancleRequest(MatchmakerService service, SignInReq request)
        {
            this.mFromHeader = new HeaderDTO
            {
                uid = WebManager.Instance.mCredential.uid,
                access_token = WebManager.Instance.mCredential.access_token,
            };

            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/Matchmaker/Cancle";
            this.mMethod = ERequestMethod.POST;
        }
    }
}

