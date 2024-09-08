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
    LevelReward,
}

public class LobbyUIManager : MonoBehaviour
{

    private Dictionary<ELobbyUI, LobbyUI> UIList = new Dictionary<ELobbyUI, LobbyUI>();
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
            foreach ((ELobbyUI type, LobbyUI ui) in instance.UIList)
            {
                instance.UnRegisterUI(type);
            }
        }
    }

    public void RegisterUI(ELobbyUI type, LobbyUI ui)
    {
        if(false == UIList.TryAdd(type, ui))
        {
            Debug.Log($"{type.ToString()}로비 UI가 초기화에 실패하였습니다.");
        }

        //로비에 등록된 UI가 준비 되었는지 확인
        if (UIList.Count == Enum.GetValues(typeof(ELobbyUI)).Length - 1)
        {

        }
    }

    public void UnRegisterUI(ELobbyUI type)
    {
        UIList.Remove(type);
    }
    
    public void UpdateLobbyAllUI()
    {
        foreach ((ELobbyUI type, LobbyUI ui) in instance.UIList)
        {
            ui.UpdateUI();
        }
    }

    public void UpdateLobbyUI(ELobbyUI type)
    {
        UIList[type].UpdateUI();
    }

}
