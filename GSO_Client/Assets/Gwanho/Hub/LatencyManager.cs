using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatencyManager : MonoBehaviour
{
    private List<float> mLatencies = new List<float>();
    private int mLatencyCapcity = 100;

    public void AddLatency(float latency)
    {
        mLatencies.Add(latency);
        if (mLatencies.Count > mLatencyCapcity)
        {
            mLatencies.RemoveAt(0);
        }
    }

    public long GetAverageLatency()
    {
        if (mLatencies.Count == 0)
        {
            return 0;
        }

        float sum = 0f;
        foreach (float latency in mLatencies)
        {
            sum += latency;
        }

        return Convert.ToInt64(sum / mLatencies.Count);
    }

}
