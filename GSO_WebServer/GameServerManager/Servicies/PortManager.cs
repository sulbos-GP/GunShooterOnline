using Docker.DotNet.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace GameServerManager.Servicies
{
    public class PortManager
    {
        private static SpinLock mSpinLock = new SpinLock();
        private ConcurrentQueue<int> mPortQueue = new ConcurrentQueue<int>();

        public PortManager(int start, int end) 
        {
            for (int i = start; i < end; i++)
            {
                mPortQueue.Enqueue(i);
            }
        }

        public void PushPort(int port)
        {
            mPortQueue.Enqueue(port);
        }

        public int PopPort()
        {
            if (mPortQueue.TryDequeue(out int port))
            {
                return port;
            }
            else
            {
                return -1;
            }
        }
    }
}
