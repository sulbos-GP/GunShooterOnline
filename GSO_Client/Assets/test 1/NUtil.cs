using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class NUtil
{
    #region Yield

    public static readonly YieldInstruction yieldFixed = new WaitForFixedUpdate();
    public static readonly YieldInstruction yieldFrame = new WaitForEndOfFrame();
    private static Dictionary<float, YieldInstruction> _yieldSec = new Dictionary<float, YieldInstruction>();

    public static YieldInstruction GetYieldSec(float sec)
    {
        if (!_yieldSec.ContainsKey(sec))
            _yieldSec.Add(sec, new WaitForSeconds(sec));
        return _yieldSec[sec];
    }

    #endregion

    #region StringChange

    public readonly static StringBuilder sb = new StringBuilder(1000);
    private const char QUOTE = ',';

    // List를 구분자를 통해 문자열로 전환.
    public static string ListToStr(List<int> list)
    {
        if (list.Count == 0)
            return string.Empty;

        sb.Clear();
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            sb.Append(list[i]);
            sb.Append(QUOTE);
        }

        sb.Remove(sb.Length - 1, 1);

        string str = sb.ToString();
        sb.Clear();
        return str;
    }

    // 문자열을 List로 전환.
    public static void StrToList(string str, ref List<int> list)
    {
        if (string.IsNullOrEmpty(str))
            return;

        list.Clear();
        string[] strArr = str.Split(QUOTE);
        int count = strArr.Length;
        for (int i = 0; i < count; i++)
            list.Add(int.Parse(strArr[i]));
    }

    // List를 구분자를 통해 문자열로 전환.
    public static string ListToStr(List<long> list)
    {
        if (list.Count == 0)
            return string.Empty;

        sb.Clear();
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            sb.Append(list[i]);
            sb.Append(QUOTE);
        }

        sb.Remove(sb.Length - 1, 1);

        string str = sb.ToString();
        sb.Clear();
        return str;
    }

    #endregion

    #region Random

    // 랜덤 bool
    public static bool Random()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    // 랜덤 bool
    public static bool Random(int count)
    {
        return UnityEngine.Random.Range(0, count) == 0;
    }

    // 백분율 랜덤 bool (ratio = 0~1)
    public static bool Random(float ratio)
    {
        return UnityEngine.Random.Range(0f, 1f) <= ratio;
    }

    #endregion

    #region Camera
    public static Vector3 WorldToNormalizedViewportPoint(this Camera camera, Vector3 point)
    {
        // Use the default camera matrix to normalize XY, 
        // but Z will be distance from the camera in world units
        point = camera.WorldToViewportPoint(point);

        if (camera.orthographic)
        {
            // Convert world units into a normalized Z depth value
            // based on orthographic projection
            point.z = (2 * (point.z - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) - 1f;
        }
        else
        {
            // Convert world units into a normalized Z depth value
            // based on perspective projection
            point.z = ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane))
                + (1 / point.z) * (-2 * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane));
        }

        return point;
    }

    public static Vector3 NormalizedViewportToWorldPoint(this Camera camera, Vector3 point)
    {
        if (camera.orthographic)
        {
            // Convert normalized Z depth value into world units
            // based on orthographic projection
            point.z = (point.z + 1f) * (camera.farClipPlane - camera.nearClipPlane) * 0.5f + camera.nearClipPlane;
        }
        else
        {
            // Convert normalized Z depth value into world units
            // based on perspective projection
            point.z = ((-2 * camera.farClipPlane * camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) /
                (point.z - ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)));
        }

        // Use the default camera matrix which expects normalized XY but world unit Z 
        return camera.ViewportToWorldPoint(point);
    }
    #endregion
}