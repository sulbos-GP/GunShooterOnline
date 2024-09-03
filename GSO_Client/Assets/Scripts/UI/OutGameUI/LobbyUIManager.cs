using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using WebCommonLibrary.DTO.DataLoad;

public enum ELobbyUI
{
    None,
    Profile,
}

public class LobbyUIManager : MonoBehaviour
{

    private Dictionary<ELobbyUI, LobbyUI> LobbyUIList = new Dictionary<ELobbyUI, LobbyUI>();
    private static LobbyUIManager instance = new();

    public static LobbyUIManager Instance
    {
        get
        {
            Init();
            return instance;
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Start()
    {

    }

    public static void Init()
    {
        if (instance == null)
        {
            var manager = GameObject.Find("@LobbyUIManager");
            if (manager == null)
            {
                return;
            }
            instance = manager.GetComponent<LobbyUIManager>();
        }
    }

    public void OnDestroy()
    {
        if(instance != null)
        {
            foreach ((ELobbyUI type, LobbyUI ui) in instance.LobbyUIList)
            {
                instance.UnRegisterUI(type);
            }
        }
    }

    public void RegisterUI(ELobbyUI type, LobbyUI ui)
    {
        LobbyUIList.TryAdd(type, ui);
        LobbyUIList[type].UpdateUI();
    }

    public void UnRegisterUI(ELobbyUI type)
    {
        LobbyUIList.Remove(type);
    }
    
    public void UpdateLobbyAllUI()
    {
        foreach ((ELobbyUI type, LobbyUI ui) in instance.LobbyUIList)
        {
            ui.UpdateUI();
        }
    }

    public void UpdateLobbyUI(ELobbyUI type)
    {
        LobbyUIList[type].UpdateUI();
    }

}
