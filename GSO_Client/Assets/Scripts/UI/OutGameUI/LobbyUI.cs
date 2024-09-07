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

    }

    public void OnDestroy()
    {
        LobbyUIManager.Instance.UnRegisterUI(type);
    }

    /// <summary>
    /// ó���� �ʱ�ȭ �ؾ��ϴ� UI �κ�
    /// </summary>
    public abstract void InitUI();

    /// <summary>
    /// ���������� ������Ʈ �ؾ��ϴ� UI �κ�
    /// </summary>
    public abstract void UpdateUI();
}
