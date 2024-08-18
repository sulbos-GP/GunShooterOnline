
/// <summary>
/// 매치메이킹 서버 관련 API
/// </summary>
public class MatchmakerService : WebClientService
{

    public MatchmakerResource mMatchmakerResource;

    public MatchmakerService()
    {

        this.mBaseUrl = Managers.EnvConfig.GetEnvironmentConfig().MatchmakerBaseUri;

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

    public MatchJoinRequest GetMatchJoinRequest(HeaderVerfiyPlayer header, MatchmakerJoinReq body)
    {
        return new MatchJoinRequest(this.mService, header, body);
    }

    public MatchCancleRequest GetMatchCancleRequest(HeaderVerfiyPlayer header, MatchmakerCancleReq packet)
    {
        return new MatchCancleRequest(this.mService, header, packet);
    }

    /// <summary>
    /// Requset
    /// </summary>

    //매칭 참여
    public class MatchJoinRequest : WebClientServiceRequest<MatchmakerJoinRes>
    {
        public MatchJoinRequest(MatchmakerService service, HeaderVerfiyPlayer header, MatchmakerJoinReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Matchmaker/Join";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //매칭 취소
    public class MatchCancleRequest : WebClientServiceRequest<MatchmakerCancleRes>
    {
        public MatchCancleRequest(MatchmakerService service, HeaderVerfiyPlayer header, MatchmakerCancleReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Matchmaker/Cancle";
            this.mMethod = ERequestMethod.POST;
        }
    }
}

