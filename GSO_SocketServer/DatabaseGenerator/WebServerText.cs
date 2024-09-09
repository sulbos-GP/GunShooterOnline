using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class WebServerText
    {
        // {0} ContextEnum
        // {1} ContextRegion
        // {2} GetContext
        // {3} LoadDatabaseContext
        // {4} GetDataContext
        public static string context =
            @"using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using SqlKata.Execution;
using WebCommonLibrary.Models.MasterDB;
using WebCommonLibrary.Models.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;

namespace GSO_WebServerLibrary.Reposiotry.Define.MasterDB
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

        public bool IsValid()
        {{
            return datas.Count != 0;
        }}
    }}

    public partial class MasterDB : IMasterDB
    {{
	    #region DatabaseTable 
        private DB_Version appVersion = new DB_Version();
        private DB_Version dataVersion = new DB_Version();
        {0}
	    #endregion

        public DB_Version AppVersion
        {{
            get
            {{
                return appVersion;
            }}
        }}
            
        public DB_Version DataVersion
        {{
            get
            {{
                return dataVersion;
            }}
        }}

        {1}

        public async Task<bool> LoadMasterTables()
        {{
            appVersion = await LoadLatestAppVersion();
            dataVersion = await LoadLatestDataVersion();
            
            {2}

            return ValidateMasterData();
        }}

        private bool ValidateMasterData()
        {{

            if({3}
        appVersion == null ||
        dataVersion == null)
            {{
                return false;  
            }}

            return true;
        }}

        private async Task<DatabaseTable<T>> LoadTable<T>(string name) where T : class
        {{
            DatabaseTable<T> table = new DatabaseTable<T>();
            var datas = await mQueryFactory.Query(name).GetAsync<T>();
            table.LoadTable(datas);
            return table;
        }}

        public DB_Version GetAppVersion()
        {{
            return AppVersion;
        }}

        public DB_Version GetDataVersion()
        {{
            return DataVersion;
        }}

        {4}
    
        {5}
    }}
}}";
    }
}
