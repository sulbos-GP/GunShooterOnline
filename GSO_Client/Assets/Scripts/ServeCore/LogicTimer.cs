﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ServerCore
{
    public class LogicTimer
    {
        public const float mFramesPerSecond = 60.0f;
        public const float mFixedDelta = 1.0f / mFramesPerSecond;

        private double mAccumulator;
        private long mLastTime;

        private readonly Stopwatch mStopwatch;
        private readonly Action mAction;

        public float LerpAlpha => (float)mAccumulator / mFixedDelta;

        public LogicTimer(Action action)
        {
            mStopwatch = new Stopwatch();
            mAction = action;
        }

        public void Start()
        {
            mLastTime = 0;
            mAccumulator = 0.0;
            mStopwatch.Restart();
        }

        public void Stop()
        {
            mStopwatch.Stop();
        }

        public void Update()
        {
            long elapsedTicks = mStopwatch.ElapsedTicks;
            mAccumulator += (double)(elapsedTicks - mLastTime) / Stopwatch.Frequency;
            mLastTime = elapsedTicks;

            while (mAccumulator >= mFixedDelta)
            {
                mAction();
                mAccumulator -= mFixedDelta;
            }
        }
    }
}
