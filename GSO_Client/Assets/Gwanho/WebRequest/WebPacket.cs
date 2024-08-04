using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class DataLoadUserInfo
{
    public UserInfo UserInfo { get; set; }
    public UserSkillInfo SkillInfo { get; set; }
    public UserMetadataInfo MetadataInfo { get; set; }
}

public class UserInfo
{
    public int uid { get; set; } = 0;
    public string player_id { get; set; } = string.Empty;
    public string nickname { get; set; } = string.Empty;
    public string service { get; set; } = string.Empty;
    public DateTime create_dt { get; set; } = DateTime.MinValue;
    public DateTime recent_login_dt { get; set; } = DateTime.MinValue;
}

public class UserSkillInfo
{
    public double rating { get; set; } = 0.0;
    public double deviation { get; set; } = 0.0;
    public double volatility { get; set; } = 0.0;
}

public class UserMetadataInfo
{
    public int total_games { get; set; } = 0;
    public int kills { get; set; } = 0;
    public int deaths { get; set; } = 0;
    public int damage { get; set; } = 0;
    public int farming { get; set; } = 0;
    public int escape { get; set; } = 0;
    public int survival_time { get; set; } = 0;
}