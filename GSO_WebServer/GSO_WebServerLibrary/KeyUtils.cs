using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary
{
    public static class KeyUtils
    {
        public enum EKey
        {
            UID,
            UidLock,
            REFRESH,
        }

        public static long GetUID(string key)
        {
            string[] parts = key.Split('_');
            if (parts.Length > 1)
            {
                string valueAfterUnderscore = parts[1];
                if (int.TryParse(valueAfterUnderscore, out int uid))
                {
                    return uid;
                }
            }

            return 0;
        }

        public static string MakeKey(EKey key, int uid)
        {
            return key.ToString() + "_" + uid.ToString();
        }

        public static string MakeKey(EKey key, string uid)
        {
            return key.ToString() + "_" + GetUID(uid).ToString();
        }

    }
}
