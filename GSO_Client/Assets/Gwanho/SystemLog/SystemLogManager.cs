using Doozy.Runtime.UIElements.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemLogManager : MonoBehaviour
{

    private static SystemLogManager instance = null;

    private TextMeshProUGUI mSystemLogText;
    private ScrollRect      mSystemLogScrollView;
    private Button          mSystemLogButton;
    private List<string>    mSystemLogMessages = new List<string>();

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    public static SystemLogManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (mSystemLogButton != null)
        {
            mSystemLogButton.onClick.RemoveListener(OnToggleSystemLog);
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mSystemLogText == null)
        {
            GameObject button = GameObject.Find("SystemLogButton");
            if (button != null)
            {
                if(mSystemLogButton != null)
                {
                    mSystemLogButton.onClick.RemoveListener(OnToggleSystemLog);
                }

                mSystemLogButton = button.GetComponent<Button>();
                mSystemLogButton.onClick.AddListener(OnToggleSystemLog);
            }

            GameObject text = GameObject.Find("SystemLogText");
            if (text != null)
            {
                mSystemLogText = text.GetComponent<TextMeshProUGUI>();
                mSystemLogText.color = Color.yellow;
            }

            GameObject scrollView = GameObject.Find("SystemLogScrollView");
            if (text != null)
            {
                mSystemLogScrollView = scrollView.GetComponent<ScrollRect>();
                UpdateLogDisplay();
                mSystemLogScrollView.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateLogDisplay()
    {
        if (mSystemLogText != null)
        {
            mSystemLogText.text = string.Join("\n", mSystemLogMessages.ToArray());
            mSystemLogScrollView.verticalNormalizedPosition = 0.0f;
        }
    }

    private void OnToggleSystemLog()
    {
        if(mSystemLogText != null)
        {
            mSystemLogScrollView.gameObject.SetActive(!mSystemLogScrollView.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// 로그 메세지 입력
    /// </summary>
    public void LogMessage(string message)
    {
        mSystemLogMessages.Add(message);
        UpdateLogDisplay();
    }

}
