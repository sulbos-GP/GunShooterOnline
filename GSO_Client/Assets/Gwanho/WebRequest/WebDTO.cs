using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderDTO
{
    public string uid { get; set; } = string.Empty;
    public string access_token { get; set; } = string.Empty;
}

public class SignInReq
{
    public string user_id { get; set; } = string.Empty;
    public string server_code { get; set; } = string.Empty;
    public string service { get; set; } = string.Empty;
}

public class SignInRes
{
    public short error { get; set; }

    public int uid { get; set; } = 0;

    public string access_token { get; set; } = string.Empty;

    public long expires_in { get; set; } = 0;

    public string scope { get; set; } = string.Empty;

    public string token_type { get; set; } = string.Empty;

    public DataLoadUserInfo userData { get; set; }
}


public class SetNicknameReq
{
    public string new_nickname { get; set; } = string.Empty;
}

public class SetNicknameRes
{
    public short error { get; set; }
    public string nickname { get; set; } = string.Empty;
}