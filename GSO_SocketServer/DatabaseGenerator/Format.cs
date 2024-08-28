using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class Format
    {
        // {0} ContextEnum
        // {1} ContextRegion
        // {2} GetContext
        // {3} LoadDatabaseContext
        // {4} GetDataContext
        public static string context =
            @"using Server.Database.Data;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Server.Database.Handler
{{
    
    //**** Context 확인용 ****//
    public enum EDatabaseTable
    {{ {0}
    }}

    public class DatabaseContext
    {{
	    #region DatabaseTable {1}
	    #endregion

        public DatabaseContext()
        {{
            
        }}

        {2}

        public async Task LoadDatabaseContext()
        {{ {3}
        }}
        
        {4}

    }}
}}";

        // {0} 테이블 이름
        // {1} 대문자 이름
        public static string ContextEnum =
            @"
        [Description (""{0}"")]
        {1},
";

        // {0} 데이터 구조체
        // {1} 소문자 이름
        public static string ContextRegion =
            @"
        private DatabaseTable<{0}> {1} = new DatabaseTable<{0}>();";

        // {0} 데이터 구조
        // {1} 대문자 이름
        // {2} 소문자 이름
        public static string GetContext =
            @"
        public DatabaseTable<{0}> {1}
        {{
            get
            {{
                return {2};
            }}
        }}
            ";

        // {0} 대문자 이름
        // {1} 데이터 구조
        // {2} 테이블 이름
        public static string LoadDatabaseContext =
            @"
            {0}.LoadTable(await DatabaseHandler.MasterDB.LoadTable<{1}>(""{2}""));";

        // {0} 데이터 구조체
        // {1} 대문자 이름
        public static string GetDataContext =
            @"
        public {0} Get{1}(int id)
        {{
            return {1}.Get(id);
        }}
            ";

    }
}
