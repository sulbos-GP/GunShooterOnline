using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWebUI
{
    None,
    Loading,
}

public class WebUIFactory : MonoBehaviour
{

    private Dictionary<EWebUI, GameObject> mWebUIs;

    private void Awake()
    {
        CreateUI(EWebUI.Loading, "UI/Web/UI_RequestLoading");
    }

    private void CreateUI(EWebUI ui, string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab != null)
        {
            mWebUIs.Add(ui, Instantiate(prefab));
        }
    }

    public void Show(EWebUI ui)
    {
        mWebUIs.TryGetValue(ui, out GameObject prefab);
        if (prefab != null)
        {
            prefab.SetActive(true);
        }
    }

    public void Hide(EWebUI ui)
    {
        mWebUIs.TryGetValue(ui, out GameObject prefab);
        if (prefab != null)
        {
            prefab.SetActive(false);
        }
    }
}