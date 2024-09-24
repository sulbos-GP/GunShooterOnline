using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemLogManager
{

    private GameObject systemLogPrefab = null; //UI ������

    private GameObject systemLogWindow = null; //ȭ�鿡 ���̴� UI

    private List<string> logMessages = new List<string>();

    /// <summary>
    /// �α� ������ ���� (�� �ε� ��)
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
    /// �α� ������ ����
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
    /// �α� ������Ʈ
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
    /// �α� �޼��� �Է�
    /// </summary>
    public void Message(string message)
    {
        logMessages.Add(message);
        UpdateLogDisplay();
    }

}
