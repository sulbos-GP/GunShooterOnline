using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotifyUI : MonoBehaviour
{
    public static NotifyUI instance;
    private TextMeshProUGUI _title;
    private TextMeshProUGUI _content;

    public void Awake()
    {
        if(instance == null)
            instance = this;
        Init();
    }

    private void Init()
    {
        _title = gameObject.transform.Find("Title/Bg/Text (TMP)").GetComponent<TextMeshProUGUI>();
        _content = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetTitle(string title)
    {
        _title.text = title;
    }

    public void SetContent(string content)
    {
        _content.text = content;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
