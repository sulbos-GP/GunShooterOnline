using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemLogManager
{

    private GameObject systemLogPrefab = null; //UI 프리펩

    private GameObject systemLogWindow = null; //화면에 보이는 UI

    private List<string> logMessages = new List<string>();

    /// <summary>
    /// 로그 윈도우 생성 (씬 로드 시)
    /// </summary>
    public void CreateSystemWindow()
    {

        if (systemLogPrefab == null)
        {
            systemLogPrefab = Resources.Load<GameObject>("Prefabs/UI/SystemLog/SystemLogWindow");
        }

        if(systemLogWindow != null)
        {
            DestroySystemWindow();
        }

        if (systemLogPrefab != null)
        {
            var canvas = Managers.FindObjectOfType<Canvas>();
            systemLogWindow = Managers.Instantiate(systemLogPrefab, canvas.transform);
            UI_SystemLog system = systemLogWindow.GetComponentInChildren<UI_SystemLog>();

            UpdateLogDisplay();

            system.CloseWindow();
        }
    }

    /// <summary>
    /// 로그 윈도우 삭제
    /// </summary>
    public void DestroySystemWindow()
    {
        logMessages.Clear();
        if (systemLogWindow != null)
        {
            Managers.Destroy(systemLogWindow);
        }
    }

    /// <summary>
    /// 로그 업데이트
    /// </summary>
    private void UpdateLogDisplay()
    {
        if(systemLogWindow != null)
        {
            string log = string.Join("\n", logMessages.ToArray());
            UI_SystemLog system = systemLogWindow.GetComponentInChildren<UI_SystemLog>();
            system.UpdateLog(log);
        }
    }

    /// <summary>
    /// 로그 메세지 입력
    /// </summary>
    public void Message(string message)
    {
        logMessages.Add(message);
        UpdateLogDisplay();
    }

}
