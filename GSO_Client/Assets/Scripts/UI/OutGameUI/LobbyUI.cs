using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LobbyUI : MonoBehaviour
{

    protected abstract ELobbyUI type { get; }

    public void Start()
    {

        InitUI();

        UpdateUI();

        LobbyUIManager.Instance.RegisterUI(type, this);

        OnRegister();
    }

    public void OnDestroy()
    {
        //Debug.Log($"[LobbyUI.{type.ToString()}] is destroy");
        //OnUnRegister();
    }

    /// <summary>
    /// 처음에 초기화 해야하는 UI 부분
    /// </summary>
    public abstract void InitUI();

    /// <summary>
    /// 지속적으로 업데이트 해야하는 UI 부분
    /// </summary>
    public abstract void UpdateUI();

    /// <summary>
    /// 등록 이후에 해야하는 부분
    /// </summary>
    public abstract void OnRegister();

    /// <summary>
    /// 등록 해제 이후에 해야하는 부분
    /// </summary>
    public abstract void OnUnRegister();
}
