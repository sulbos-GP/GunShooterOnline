using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AuthorizeResource;
using static UserResource;

public class UI_Nickname : MonoBehaviour
{
    [SerializeField]
    private GameObject nicknameWindow;

    [SerializeField]
    private string inputText;

    [SerializeField]
    private TMP_InputField nicknameInputField;

    [SerializeField]
    private TMP_Text nicknameDescription;

    private void Awake()
    {
        nicknameInputField.onValueChanged.AddListener(OnChangeNicknameValue);
    }

    void Start()
    {
        nicknameDescription.enabled = false;

        string nickname = Managers.Web.mUserInfo.UserInfo.nickname;
        nicknameWindow.SetActive(nickname == string.Empty);

    }

    private void Update()
    {
    }

    public void OnChangeNicknameValue(string value)
    {
        nicknameDescription.enabled = false;
        inputText = value;
    }

    public void OnClickSetNickname()
    {
        Debug.Log("OnClickSetNickname");

        if (inputText == string.Empty)
        {
            ShowDescription("�г����� ������ �� �����ϴ�.");
            return;
        }

        if(2 > inputText.Length || inputText.Length > 10)
        {
            ShowDescription("�г����� �ּ� 2�ڿ��� 10�ڱ��� �Է��� �� �ֽ��ϴ�.");
            return;
        }

        var header = new HeaderVerfiyPlayer
        {
            uid = Managers.Web.mCredential.uid,
            access_token = Managers.Web.mCredential.access_token,
        };

        var body = new SetNicknameReq
        {
            new_nickname = inputText,
        };

        GsoWebService service = new GsoWebService();
        SetNicknameRequest request = service.mUserResource.GetSetNicknameRequest(header, body);
        request.ExecuteAsync(OnProcessSetNickname);

        ShowDescription("�г��� ��û Ȯ����");
    }

    public void OnProcessSetNickname(SetNicknameRes response)
    {
        if(response.error_code == 0)
        {
            Managers.Web.mUserInfo.UserInfo.nickname = response.nickname;
            nicknameWindow.SetActive(false);
        }
        else
        {
            ShowDescription(response.error_description);
        }
    }

    public void ShowDescription(string text)
    {
        nicknameDescription.enabled = true;

        nicknameDescription.text = text;
    }


}
