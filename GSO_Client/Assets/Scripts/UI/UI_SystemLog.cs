using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SystemLog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text logText;

    [SerializeField]
    private ScrollRect logScrollView;

    [SerializeField]
    private Button logButton;

    public void Awake()
    {
        logButton.onClick.AddListener(OnClickLogView);
    }

    public void OnClickLogView()
    {
        var obj = logScrollView.gameObject;
        if (obj.activeSelf == true)
        {
            obj.SetActive(false);
        }
        else
        {
            logScrollView.verticalNormalizedPosition = 0.0f;
            obj.SetActive(true);
        }

    }

    public void UpdateLog(string log)
    {
        logText.text = log;
        //logScrollView.verticalNormalizedPosition = 0.0f;
    }

    public void CloseWindow()
    {
        var obj = logScrollView.gameObject;
        obj.SetActive(false);
    }
}
