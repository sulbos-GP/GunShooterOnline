using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public const int deltaTimeMode = 0;
    public const int unscaledDeltaTimeMode = 1;

    private float deltaTime = 0.0f;

    private int curMode;

    private void Awake()
    {
        curMode = 0;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(curMode == 0)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; //짧은 시간에 확튀는 fps 무시
        }
        else if(curMode == 1) 
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f; //일반적인 fps
        }
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        int fontSize = Screen.height / 25;

        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = fontSize,
            normal = { textColor = Color.white }
        };
        Rect rect = new Rect(20, 10, 200, 80);
        string text = string.Format("{0:0.} FPS", fps);
        GUI.Label(rect, text, style);
    }

    [ContextMenu("Default Mode")]
    public void ChangeDefaultMode()
    {
        curMode = deltaTimeMode;
    }

    [ContextMenu("Unscaled Mode")]
    public void ChangeUnscaledMode()
    {
        curMode = unscaledDeltaTimeMode;
    }
}
