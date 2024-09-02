using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebCommonLibrary.DTO.Authentication;
using WebCommonLibrary.DTO.User;
using static UserResource;

/// <summary>
/// 로비 서버 역할의 서버
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
/// 인증 관련 서비스
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

    public SingInRequest GetSignInRequest(SignInReq body)
    {
        return new SingInRequest(this.mService, body);
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

    //기존에 접속한적이 있는지?
    public class AuthenticationRequest : WebClientServiceRequest<AuthenticationRes>
    {
        public AuthenticationRequest(GsoWebService service, AuthenticationReq request)
        {
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/Authentication";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //로그인 요청
    public class SingInRequest : WebClientServiceRequest<SignInRes>
    {
        public SingInRequest(GsoWebService service, SignInReq body)
        {
            this.mFromBody = body;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/SignIn";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //로그아웃 요청
    public class SignOutRequest : WebClientServiceRequest<SignOutRes>
    {
        public SignOutRequest(GsoWebService service, HeaderVerfiyPlayer header, SignOutReq request)
        {
            this.mFromBody = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/SignOut";
            this.mMethod = ERequestMethod.POST;
        }
    }

    //로그아웃 요청
    public class RefreshTokenRequest : WebClientServiceRequest<RefreshTokenRes>
    {
        public RefreshTokenRequest(GsoWebService service, RefreshTokenReq request)
        {
            this.mFromBody = null;
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/Authorize/RefreshToken";
            this.mMethod = ERequestMethod.POST;
        }
    }

}

/// <summary>
/// 유저 관련 서비스
/// </summary>
public class UserResource
{
    private readonly GsoWebService mService;

    public UserResource(GsoWebService service)
    {
        mService = service;
    }

    /// <summary>
    /// 닉네임 변경 요청
    /// </summary>
    public SetNicknameRequest GetSetNicknameRequest(HeaderVerfiyPlayer header, SetNicknameReq body)
    {
        return new SetNicknameRequest(this.mService, header, body);
    }


    public class SetNicknameRequest : WebClientServiceRequest<SetNicknameRes>
    {
        public SetNicknameRequest(GsoWebService service, HeaderVerfiyPlayer header, SetNicknameReq request)
        {
            this.mFromBody = header.ToDictionary();
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/api/User/SetNickname";
            this.mMethod = ERequestMethod.POST;
        }
    }
}

/// <summary>
/// 로드 관련 서비스
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
/// 게임 관련 서비스
/// </summary>

public class GameResource
{
    private readonly GsoWebService mService;

    public GameResource(GsoWebService service)
    {
        mService = service;
    }

    /// <summary>
    /// 빠른 정보 불러오기
    /// </summary>
    //public QuickInfoRequest GetQuickInfoRequest(HeaderVerfiyPlayer header, QuickInfoReq body)
    //{
    //    return new QuickInfoRequest(this.mService, header, body);
    //}


    //public class QuickInfoRequest : WebClientServiceRequest<QuickInfoRes>
    //{
    //    public QuickInfoRequest(GsoWebService service, HeaderVerfiyPlayer header, QuickInfoReq request)
    //    {
    //        this.mFromBody = header.ToDictionary();
    //        this.mFromBody = request;
    //        this.mEndPoint = service.mBaseUrl + "/api/Game/QuickInfo";
    //        this.mMethod = ERequestMethod.POST;
    //    }
    //}

}


/// <summary>
/// 장비 관련 서비스
/// </summary>