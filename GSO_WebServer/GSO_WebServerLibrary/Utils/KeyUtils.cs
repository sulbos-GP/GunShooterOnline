using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Utils
{
    public static class KeyUtils
    {

        private static string mSeparator = "-";

        public enum EKey
        {
            UID,
            UIDLock,
            REFRESH,
            MATCH,
            MATCHLock,
            SESSION,
            SESSIONLock,
        }

        public static string GetValue(string key)
        {
            return key.Substring(key.IndexOf(mSeparator) + 1);
        }

        //임시
        public static int GetUID(string key)
        {
            return Convert.ToInt32(key.Substring(key.IndexOf(mSeparator) + 1));
        }

        public static string MakeKey(EKey type, int value)
        {
            return type.ToString() + mSeparator + value.ToString();
        }

        public static string MakeKey(EKey type, string value)
        {
            return type.ToString() + mSeparator + value;
        }

    }
}
