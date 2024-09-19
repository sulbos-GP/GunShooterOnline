using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebCommonLibrary.Models.GameDB;

public class UI_Notice : LobbyUI
{
    protected override ELobbyUI type => ELobbyUI.Notice;

    [SerializeField]
    private GameObject noticePrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private Button toggleButton;

    public void Awake()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(OnClickToggle);
        }
    }

    public void OnClickToggle()
    {
        bool isActive = this.gameObject.activeSelf;
        this.gameObject.SetActive(!isActive);
    }
    public override void InitUI()
    {

    }

    public override void UpdateUI()
    {

    }

    public override void OnRegister()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnUnRegister()
    {

    }
}
