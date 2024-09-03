using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//임시 나중에 DLL로 모델 가져올 예정

public class LobbyUIData
{
    public LobbyUI_Profile profile;
}

public class LobbyUI_Profile
{
    public int     level = 0;
    public string  nickname = "Dummy";
}