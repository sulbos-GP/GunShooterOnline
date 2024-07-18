using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Lobby;

    private void Awake()
    {
        Init();
    }


    protected virtual void Init()
    {
        var obj = FindObjectOfType(typeof(EventSystem));

        Debug.Log("test");
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}