using System;
using System.Collections.Generic;

namespace ServerCore
{

    public class JobSerializer
    {
        private bool _flush; //실행 중인지?
        private readonly Queue<IJob> _jobQueue = new Queue<IJob>();
        private readonly object _lock = new object();
        private readonly JobTimer jobtimer = new JobTimer();

        //pushAfter ------------------------------------------------------------------------------------------
        public IJob PushAfter(int tickAfter, Action action)
        {
            return PushAfter(tickAfter, new Job(action));
        }

        public IJob PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1)
        {
            return PushAfter(tickAfter, new Job<T1>(action, t1));
        }

        public IJob PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2)
        {
            return PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2));
        }

        public IJob PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            return PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3));
        }


        public IJob PushAfter(int tickAfter, IJob job)
        {
            jobtimer.Push(job, tickAfter);
            return job;
        }

        //push -----------------------------------------------------------------------------------------------


        public void Push(Action action)
        {
            Push(new Job(action));
        }

        public void Push<T1>(Action<T1> action, T1 t1)
        {
            Push(new Job<T1>(action, t1));
        }

        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Push(new Job<T1, T2>(action, t1, t2));
        }

        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            Push(new Job<T1, T2, T3>(action, t1, t2, t3));
        }
       

        public void Push<T1, T2, T3, T4>(Action<T1, T2, T3, T4 > action, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Push(new Job<T1, T2, T3, T4>(action, t1, t2, t3, t4));
        }

        public void Push<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            Push(new Job<T1, T2, T3, T4, T5>(action, t1, t2, t3, t4, t5));
        }

        public void Push<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            Push(new Job<T1, T2, T3, T4, T5, T6>(action, t1, t2, t3, t4, t5, t6));
        }

        public void Push<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            Push(new Job<T1, T2, T3, T4, T5, T6, T7>(action, t1, t2, t3, t4, t5, t6, t7));
        }

        public void Push<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            Push(new Job<T1, T2, T3, T4, T5, T6, T7, T8>(action, t1, t2, t3, t4, t5, t6, t7, t8));
        }

        public void Push(IJob job)
        {
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
            }
        }


        public void Clear()
        {
            jobtimer.Clear();

            lock (_lock)
                _jobQueue.Clear();
        }

        public void Flush()
        {
            jobtimer.Flush();

            while (true)
            {
                var job = Pop();
                if (job == null)
                    return;

                job.Execute();
            }
        }

        private IJob Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }

                return _jobQueue.Dequeue();
            }
        }



    }
}
