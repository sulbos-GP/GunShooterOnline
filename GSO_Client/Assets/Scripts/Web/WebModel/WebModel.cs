using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//임시 나중에 DLL로 모델 가져올 예정

public class WebClientCredential
{
    public string uid { get; set; } = string.Empty;

    public string access_token { get; set; } = string.Empty;

    public long expires_in { get; set; } = 0;

    public string scope { get; set; } = string.Empty;

    public string token_type { get; set; } = string.Empty;
}