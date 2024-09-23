
#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace WebCommonLibrary.Reposiotry
{
    public class DbTable<TValue> : IEnumerable<KeyValuePair<int, TValue>> where TValue : class, new()
    {
        private Dictionary<int, TValue>? _data = null;

        public DbTable(Dictionary<int, TValue>? data)
        {
            this._data = data;
        }

        public bool IsValid()
        {
            return _data != null;
        }

        public TValue Find(int key)
        {
            return (_data != null) ? this._data[key] : new TValue();
        }

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
        {
            return (_data != null) ? this._data.GetEnumerator() : new Dictionary<int, TValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
            