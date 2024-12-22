using System;
using System.Diagnostics.CodeAnalysis;
using ServerCore;

namespace ServerCore
{

    internal struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; //실행시간
        public IJob job; //action

        public int CompareTo([AllowNull] JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        private readonly object _lock = new object();
        private readonly PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();

        public void Push(IJob job, int tickAfter = 0)
        {
            var jobElement = new JobTimerElem();
            jobElement.execTick = Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }

        public void Clear()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_pq.Count == 0)
                        return;

                    _pq.Pop();
                }

            }
        }

        public void Flush()
        {
            while (true)
            {
                var now = Environment.TickCount;
                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        return;

                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    _pq.Pop();
                }

                jobElement.job.Execute();
            }
        }

       
    }
}
