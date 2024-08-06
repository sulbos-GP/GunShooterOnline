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
            ULock,
        }

        public static long GetUID(string key)
        {
            string[] parts = key.Split('_');
            if (parts.Length > 1)
            {
                string valueAfterUnderscore = parts[1];
                if (long.TryParse(valueAfterUnderscore, out long result))
                {
                    return result;
                }
            }

            return 0;
        }

        public static string MakeKey(EKey key, long uid)
        {
            return key.ToString() + "_" + uid.ToString();
        }

        public static string MakeKey(EKey key, string uid)
        {
            return key.ToString() + "_" + GetUID(uid).ToString();
        }

    }
}
