using Server.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Handler
{

    public class DatabaseTable<T> where T : class
    {

        protected Dictionary<int, T> Datas = new Dictionary<int, T>();

        public void LoadTable(IEnumerable<T> datas)
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (T data in datas)
            {
                int id = (int)fields[0].GetValue(data);
                Datas.Add(id, data);
            }
        }

        public T Get(int id)
        {
            return Datas[id];
        }

    }
}
