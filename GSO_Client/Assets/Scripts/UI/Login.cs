using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
    
    private Button button;
    private TMP_InputField textPro;

    private void Awake()
    {
        textPro = gameObject.GetComponentInChildren<TMP_InputField>();
        button = gameObject.GetComponentInChildren<Button>();

        button.onClick.AddListener(LoginBtn);
    }

    public void Init()
    {
        button.onClick.AddListener(LoginBtn);
    }

    public void LoginBtn()
    {
        Debug.Log("Requstskill");
        Managers.Network.ConnectToGame(ip: textPro.text);
    }
}
