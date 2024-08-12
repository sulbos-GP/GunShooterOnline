using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UserInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI uid;

    [SerializeField]
    private TextMeshProUGUI player_id;

    [SerializeField]
    private TextMeshProUGUI nickname;

    [SerializeField]
    private TextMeshProUGUI service;

    [SerializeField]
    private TextMeshProUGUI create_dt;

    [SerializeField]
    private TextMeshProUGUI recent_login_dt;

    void Start()
    {
        
    }

    void Update()
    {
        UserInfo info = Managers.Web.mUserInfo.UserInfo;
        uid.text = info.uid.ToString();
        player_id.text = info.player_id;
        nickname.text = info.nickname;
        service.text = info.service;
        create_dt.text = info.create_dt.ToString();
        recent_login_dt.text = info.recent_login_dt.ToString();
    }
}
