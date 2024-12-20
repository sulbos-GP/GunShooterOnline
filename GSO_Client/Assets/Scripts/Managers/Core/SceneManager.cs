using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SceneManagerEx
{
    public BaseScene CurrentScene => Object.FindObjectOfType<BaseScene>();

    public void LoadScene(Define.Scene type)
    {
        //Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
    }
    
    public void UnLoadScene(Define.Scene type)
    {
        //Managers.Clear();

        SceneManager.UnloadSceneAsync(GetSceneName(type));
    }

    private string GetSceneName(Define.Scene type)
    {
        var name = Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        if (CurrentScene != null)
        {
            CurrentScene.Clear();
        }
    }
}