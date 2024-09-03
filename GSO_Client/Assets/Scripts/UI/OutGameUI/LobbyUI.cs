using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LobbyUI : MonoBehaviour
{

    protected abstract ELobbyUI type { get; }

    public void Start()
    {
        LobbyUIManager.Instance.RegisterUI(type, this);
    }

    public void OnDestroy()
    {
        LobbyUIManager.Instance.UnRegisterUI(type);
    }

    public abstract void UpdateUI();
}
