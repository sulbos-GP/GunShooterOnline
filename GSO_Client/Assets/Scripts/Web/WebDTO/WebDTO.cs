using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnityEngine;


///////////////////////////////////
///                             ///
///           공통 DTO          ///
///                             ///
///////////////////////////////////

public abstract class HeaderDTO
{
    public abstract Dictionary<string, string> ToDictionary();
}

public class HeaderCheackVersion : HeaderDTO
{
    public string app { get; set; } = string.Empty;
    public string data { get; set; } = string.Empty;

    public override Dictionary<string, string> ToDictionary()
    {
        var header = new Dictionary<string, string>();
        header["AppVersion"] = app;
        header["MasterDataVersion"] = data;
        return header;
    }
}

public class HeaderVerfiyPlayer : HeaderDTO
{
    public string uid { get; set; } = string.Empty;
    public string access_token { get; set; } = string.Empty;

    public override Dictionary<string, string> ToDictionary()
    {
        var header = new Dictionary<string, string>();
        header["uid"] = uid;
        header["access_token"] = access_token;
        return header;
    }
}

/////////////////////////////////////
/////                             ///
/////         로비(인증) DTO       ///
/////                             ///
/////////////////////////////////////
//public class SignInReq
//{
//    public string user_id { get; set; } = string.Empty;
//    public string server_code { get; set; } = string.Empty;
//    public string service { get; set; } = string.Empty;
//}

//public class AuthenticationReq
//{
//    public string user_id { get; set; } = string.Empty;
//    public string service { get; set; } = string.Empty;
//}

//public class AuthenticationRes : ErrorCodeDTO
//{
//}

//public class SignInRes : ErrorCodeDTO
//{
//    public int uid { get; set; } = 0;

//    public string access_token { get; set; } = string.Empty;

//    public long expires_in { get; set; } = 0;

//    public string scope { get; set; } = string.Empty;

//    public string token_type { get; set; } = string.Empty;

//    public DataLoadUserInfo userData { get; set; }
//}

//public class SignOutReq
//{
//    public string cause { get; set; } = string.Empty;
//}

//public class SignOutRes : ErrorCodeDTO
//{

//}

//public class RefreshTokenReq
//{
//    public int uid { get; set; } = 0;
//}

//public class RefreshTokenRes : ErrorCodeDTO
//{
//    public int uid { get; set; } = 0;
//    public string access_token { get; set; } = string.Empty;

//    public long expires_in { get; set; } = 0;

//    public string scope { get; set; } = string.Empty;

//    public string token_type { get; set; } = string.Empty;
//}

/////////////////////////////////////
/////                             ///
/////         로비(유저) DTO       ///
/////                             ///
/////////////////////////////////////
//public class SetNicknameReq
//{
//    public string new_nickname { get; set; } = string.Empty;
//}

//public class SetNicknameRes : ErrorCodeDTO
//{
//    public string nickname { get; set; } = string.Empty;
//}

/////////////////////////////////////
/////                             ///
/////       매치메이커 DTO         ///
/////                             ///
/////////////////////////////////////
//public class MatchmakerJoinReq
//{
//    public string world { get; set; } = string.Empty;
//    public string region { get; set; } = string.Empty;
//}

//public class MatchmakerJoinRes : ErrorCodeDTO
//{
//}

//public class MatchmakerCancleReq
//{
//    public string world { get; set; } = string.Empty;
//    public string region { get; set; } = string.Empty;
//}

//public class MatchmakerCancleRes : ErrorCodeDTO
//{
//}


/////////////////////////////////////
/////                             ///
/////           게임 DTO          ///
/////                             ///
/////////////////////////////////////
