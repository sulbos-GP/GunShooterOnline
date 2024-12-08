using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.DTO.Game;
using static UserResource;

/// <summary>
/// �κ� ���� ������ ����
/// </summary>
public class GsoWebService : WebClientService
{

    public AuthorizeResource mAuthorizeResource;
    public UserResource mUserResource;
    public GameResource mGameResource;

    public GsoWebService()
    {
        this.mBaseUrl = Managers.EnvConfig.GetEnvironmentConfig().CenterBaseUri;
        mAuthorizeResource = new AuthorizeResource(this);
        mUserResource = new UserResource(this);
        mGameResource = new GameResource(this);
    }

}

/// <summary>
/// ���� ���� ����
/// </summary>
public class AuthorizeResource
{
    private readonly GsoWebService mService;

    public AuthorizeResource(GsoWebService service)
    {
        mService = service;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// 

    public AuthenticationRequest GetAuthenticationRequest(AuthenticationReq packet)
    {
        return new AuthenticationRequest(this.mService, packet);
    }

    public SingInRequest GetSignInRequest(HeaderCheackVersion header, SignInReq body)
    {
        return new SingInRequest(this.mService, header, body);
    }

    public SignOutRequest GetSignOutRequest(HeaderVerfiyPlayer header, SignOutReq body)
    {
        return new SignOutRequest(this.mService, header, body);
    }

    public RefreshTokenRequest GetRefreshTokenRequest(RefreshTokenReq body)
    {
        return new RefreshTokenRequest(this.mService, body);
    }

    /// <summary>
    /// Requset
    /// </summary>

    //������ ���������� �ִ���?
    public class AuthenticationRequest : WebClientServiceRequest<AuthenticationRes>
    {
        public AuthenticationRequest(GsoWebService service, AuthenticationReq request)
        {
            this.mFromHeader = null;
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/Authentication";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //�α��� ��û
    public class SingInRequest : WebClientServiceRequest<SignInRes>
    {
        public SingInRequest(GsoWebService service, HeaderCheackVersion header, SignInReq body)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = body;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/SignIn";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //�α׾ƿ� ��û
    public class SignOutRequest : WebClientServiceRequest<SignOutRes>
    {
        public SignOutRequest(GsoWebService service, HeaderVerfiyPlayer header, SignOutReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/SignOut";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //�α׾ƿ� ��û
    public class RefreshTokenRequest : WebClientServiceRequest<RefreshTokenRes>
    {
        public RefreshTokenRequest(GsoWebService service, RefreshTokenReq request)
        {
            this.mFromHeader = null;
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/RefreshToken";
            this.mMethod = ERequestMethod.POST;
        }
    }

}

/// <summary>
/// ���� ���� ����
/// </summary>
public class UserResource
{
    private readonly GsoWebService mService;

    public UserResource(GsoWebService service)
    {
        mService = service;
    }

    /// <summary>
    /// �г��� ���� ��û
    /// </summary>
    public SetNicknameRequest GetSetNicknameRequest(HeaderVerfiyPlayer header, SetNicknameReq body)
    {
        return new SetNicknameRequest(this.mService, header, body);
    }

    public class SetNicknameRequest : WebClientServiceRequest<SetNicknameRes>
    {
        public SetNicknameRequest(GsoWebService service, HeaderVerfiyPlayer header, SetNicknameReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/Nickname/Update";
            this.mMethod = ERequestMethod.POST;
        }
    }

    /// <summary>
    /// ��Ÿ������ ��������
    /// </summary>
    public LoadMetadataRequest GetLoadMetadataRequest(HeaderVerfiyPlayer header, LoadMetadataReq body)
    {
        return new LoadMetadataRequest(this.mService, header, body);
    }

    public class LoadMetadataRequest : WebClientServiceRequest<LoadMetadataRes>
    {
        public LoadMetadataRequest(GsoWebService service, HeaderVerfiyPlayer header, LoadMetadataReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/Metadata/Load";
            this.mMethod = ERequestMethod.POST;
        }
    }

    /// <summary>
    /// Ƽ�� ������Ʈ
    /// </summary>
    public UpdateTicketRequest GetUpdateTicketRequest(HeaderVerfiyPlayer header, UpdateTicketReq body)
    {
        return new UpdateTicketRequest(this.mService, header, body);
    }

    public class UpdateTicketRequest : WebClientServiceRequest<UpdateTicketRes>
    {
        public UpdateTicketRequest(GsoWebService service, HeaderVerfiyPlayer header, UpdateTicketReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/Ticket/Update";
            this.mMethod = ERequestMethod.POST;
        }
    }

    /// <summary>
    /// ���ϸ� ������Ʈ
    /// </summary>
    public UpdateDailyTaskRequest GetUpdateDailyTaskRequest(HeaderVerfiyPlayer header, DailyTaskReq body)
    {
        return new UpdateDailyTaskRequest(this.mService, header, body);
    }

    public class UpdateDailyTaskRequest : WebClientServiceRequest<DailyTaskRes>
    {
        public UpdateDailyTaskRequest(GsoWebService service, HeaderVerfiyPlayer header, DailyTaskReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/DailyTask/Update";
            this.mMethod = ERequestMethod.POST;
        }
    }

    /// <summary>
    /// ���ϸ� ������Ʈ
    /// </summary>
    public CompleteDailyQuestRequest GetCompleteDailyQuestRequest(HeaderVerfiyPlayer header, DailyQuestReq body)
    {
        return new CompleteDailyQuestRequest(this.mService, header, body);
    }

    public class CompleteDailyQuestRequest : WebClientServiceRequest<DailyQuestRes>
    {
        public CompleteDailyQuestRequest(GsoWebService service, HeaderVerfiyPlayer header, DailyQuestReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/DailyQuest/Complete";
            this.mMethod = ERequestMethod.POST;
        }
    }
}

/// <summary>
/// �ε� ���� ����
/// </summary>
 
public class LoadResource
{
    private readonly GsoWebService mService;

    public LoadResource(GsoWebService service)
    {
        mService = service;
    }


}

/// <summary>
/// ���� ���� ����
/// </summary>

public class GameResource
{
    private readonly GsoWebService mService;

    public GameResource(GsoWebService service)
    {
        mService = service;
    }

    /// <summary>
    /// 
    /// </summary>
    public LoadStorageRequest GetLoadStorageRequest(HeaderVerfiyPlayer header, LoadStorageReq body)
    {
        return new LoadStorageRequest(this.mService, header, body);
    }

    public class LoadStorageRequest : WebClientServiceRequest<LoadStorageRes>
    {
        public LoadStorageRequest(GsoWebService service, HeaderVerfiyPlayer header, LoadStorageReq body)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = body;
            this.mEndPoint = service.mBaseUrl + "/api/Game/Storage/Load";
            this.mMethod = ERequestMethod.POST;
        }
    }

    public ResetStorageRequest GetResetStorageRequest(HeaderVerfiyPlayer header, ResetStorageReq body)
    {
        return new ResetStorageRequest(this.mService, header, body);
    }

    public class ResetStorageRequest : WebClientServiceRequest<ResetStorageRes>
    {
        public ResetStorageRequest(GsoWebService service, HeaderVerfiyPlayer header, ResetStorageReq body)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = body;
            this.mEndPoint = service.mBaseUrl + "/api/Game/Storage/Reset";
            this.mMethod = ERequestMethod.POST;
        }
    }

    public ReceivedLevelRewardRequest GetReceivedLevelRewardRequest(HeaderVerfiyPlayer header, ReceivedLevelRewardReq body)
    {
        return new ReceivedLevelRewardRequest(this.mService, header, body);
    }
    public class ReceivedLevelRewardRequest : WebClientServiceRequest<ReceivedLevelRewardRes>
    {
        public ReceivedLevelRewardRequest(GsoWebService service, HeaderVerfiyPlayer header, ReceivedLevelRewardReq request)
        {
            this.mFromHeader = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Game/Received/LevelReward";
            this.mMethod = ERequestMethod.POST;
        }
    }

}

