using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 매치메이킹 서버 관련 API
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
/// 매칭 관련 Resource
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

    //기존에 접속한적이 있는지?
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

    //로그인 요청
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

