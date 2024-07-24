using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// �κ� ���� ������ ����
/// </summary>
public class GsoWebService : WebClientService
{

    public AuthorizeResource mAuthorizeResource;
    public UserResource mUserResource;

    public GsoWebService()
    {
        this.mBaseUrl = "http://10.0.2.2:5000/api";
        mAuthorizeResource = new AuthorizeResource(this);
        mUserResource = new UserResource(this);
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

    public AuthenticationRequest GetAuthenticationRequest(SignInReq packet)
    {
        return new AuthenticationRequest(this.mService, packet);
    }

    //���� ��û
    public class AuthenticationRequest : WebClientServiceRequest<SignInRes>
    {
        public AuthenticationRequest(GsoWebService service, SignInReq request)
        {
            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/Authorize/SignIn";
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
    public SetNicknameRequest GetSetNicknameRequest(SetNicknameReq packet)
    {
        return new SetNicknameRequest(this.mService, packet);
    }


    public class SetNicknameRequest : WebClientServiceRequest<SetNicknameRes>
    {
        public SetNicknameRequest(GsoWebService service, SetNicknameReq request)
        {
            this.mFromHeader = new HeaderDTO
            {
                uid = WebManager.Instance.mCredential.uid,
                access_token = WebManager.Instance.mCredential.access_token,
            };

            this.mFromBody = request;
            this.mEndPoint = service.mBaseUrl + "/User/SetNickname";
            this.mMethod = ERequestMethod.POST;
        }
    }
}
