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
    /// ó���� �ʱ�ȭ �ؾ��ϴ� UI �κ�
    /// </summary>
    public abstract void InitUI();

    /// <summary>
    /// ���������� ������Ʈ �ؾ��ϴ� UI �κ�
    /// </summary>
    public abstract void UpdateUI();

    /// <summary>
    /// ��� ���Ŀ� �ؾ��ϴ� �κ�
    /// </summary>
    public abstract void OnRegister();

    /// <summary>
    /// ��� ���� ���Ŀ� �ؾ��ϴ� �κ�
    /// </summary>
    public abstract void OnUnRegister();
}
