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

    public GsoWebService()
    {
        this.mBaseUrl = "http://10.0.2.2:5000/api";
        mAuthorizeResource = new AuthorizeResource(this);
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
            this.mFrom = request;
            this.mEndPoint = service.mBaseUrl + "/Authorize/SignIn";
            this.mMethod = ERequestMethod.POST;
        }
    }

}