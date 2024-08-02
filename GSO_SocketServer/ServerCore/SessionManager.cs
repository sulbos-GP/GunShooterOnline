using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ServerCore
{
    public class SessionManager
    {
        int mCurSessionId = 0;
        Dictionary<int, Session> mSessions = new Dictionary<int, Session>();

        object _lock = new object();



        public void FlushSend()
        {
            lock (_lock)
            {
                foreach (var session in mSessions.Values)
                {
                    session.FlushSend();
                }
            }
        }

        public int GetSessionCount()
        {
            lock (_lock)
            {
                return mSessions.Count;
            }
        }

        public Session Insert(Session session)
        {
            lock (_lock)
            {
                int id = mCurSessionId++;

                session.mSessionManager = this;
                mSessions.Add(id, session);

                return session;
            }
        }

        public Session Find(int id)
        {
            lock (_lock)
            {
                Session session = null;
                mSessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(Session session)
        {
            lock (_lock)
            {
                session.mSessionManager = null;
                mSessions.Remove(session.mId);
            }
        }
    }
}
