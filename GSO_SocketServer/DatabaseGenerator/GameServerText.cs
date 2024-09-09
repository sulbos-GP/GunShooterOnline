using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class GameServerText
    {
        // {0} ContextRegion
        // {1} GetContext
        // {2} LoadDatabaseContext
        // {3} GetDataContext
        // {4} GetDataListContext
        public static string context =
            @"using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Models.GameDB;

namespace Server.Database.Handler
{{
    
    public class DatabaseTable<T> where T : class
    {{

        protected Dictionary<int, T> datas = new Dictionary<int, T>();

        public void LoadTable(IEnumerable<T> table)
        {{
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (T data in table)
            {{
                object? obj = fields[0].GetValue(data);
                if (obj == null)
                {{
                    continue;
                }}
                datas.Add((int)obj, data);
            }}
        }}

        public T Get(int id)
        {{
            return datas[id];
        }}

        public Dictionary<int, T> GetList()
        {{
            return datas;
        }}
    }}

    public class DatabaseContext
    {{
	    #region DatabaseTables {0}
	    #endregion

        public DatabaseContext()
        {{
            
        }}

        {1}

        public async Task LoadDatabaseContext()
        {{ {2}
        }}

        private async Task<DatabaseTable<T>> LoadTable<T>(string name) where T : class
        {{
            DatabaseTable<T> table = new DatabaseTable<T>();
            var datas = await DatabaseHandler.MasterDB.LoadTable<T>(name);
            table.LoadTable(datas);
            return table;
        }}
        
        {3}

        {4}

    }}
}}";
    }
}
