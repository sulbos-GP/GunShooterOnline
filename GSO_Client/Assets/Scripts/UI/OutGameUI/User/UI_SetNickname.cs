using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ubiety.Dns.Core;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Model.GameDB;
using static AuthorizeResource;
using static UserResource;

public class UI_SetNickname : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField input;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_Text description;

    private void Awake()
    {
        input.onValueChanged.AddListener(OnChangeNicknameValue);
        button.onClick.AddListener(OnClickSetNickname);
        description.enabled = false;
    }

    public void Start()
    {
        string nickname = Managers.Web.user.UserInfo.nickname;
        this.gameObject.SetActive((nickname == string.Empty));
    }

    public void OnChangeNicknameValue(string value)
    {
        description.enabled = false;
    }

    public void OnClickSetNickname()
    {
        Debug.Log("OnClickSetNickname");

        string nickName = input.text;
        if (nickName == string.Empty)
        {
            ShowDescription("�г����� ������ �� �����ϴ�.");
            return;
        }

        if(2 > nickName.Length || nickName.Length > 10)
        {
            ShowDescription("�г����� �ּ� 2�ڿ��� 10�ڱ��� �Է��� �� �ֽ��ϴ�.");
            return;
        }

        ClientCredential crediential = Managers.Web.credential;
        var header = new HeaderVerfiyPlayer
        {
            uid = crediential.uid.ToString(),
            access_token = crediential.access_token,
        };

        var body = new SetNicknameReq
        {
            new_nickname = nickName,
        };

        ShowDescription("�г��� ��û Ȯ����");

        try
        {
            GsoWebService service = new GsoWebService();
            SetNicknameRequest request = service.mUserResource.GetSetNicknameRequest(header, body);
            request.ExecuteAsync(OnProcessSetNickname);
        }
        catch (Exception ex)
        {
            ShowDescription($"Error : {ex.ToString()}");
        }

    }

    public void OnProcessSetNickname(SetNicknameRes response)
    {
        if(response.error_code == WebErrorCode.None)
        {
            Managers.Web.user.UserInfo.nickname = response.nickname;
            LobbyUIManager.Instance.UpdateLobbyUI(ELobbyUI.Profile);
            this.gameObject.SetActive(false);
        }
        else
        {
            ShowDescription(response.error_description);
        }
    }

    public void ShowDescription(string text)
    {
        description.enabled = true;
        description.text = text;
    }


}
