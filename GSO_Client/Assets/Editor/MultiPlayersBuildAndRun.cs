
#if UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
public class MultiPlayersBuildAndRun
{
    [MenuItem("Tools/Run Multiplayer/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }
    [MenuItem("Tools/Run Multiplayer/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);

    }
    [MenuItem("Tools/Run Multiplayer/4 Playesr")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);

    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        for (int i = 0; i < playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + i.ToString() + '/' + GetProjectName() + i.ToString() + ".exe",
               BuildTarget.StandaloneOSX, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName() //c:/jish/
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }


    static string[] GetScenePaths() // assets/scenes/game.unity
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;

        }

        return scenes;

    }

}
#endif