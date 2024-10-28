using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class Format
    {
        // {0} 테이블 이름
        public static string ContextEnum =
            @"
        {0},
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

        // {0} 소문자 이름
        // {1} 데이터 구조
        // {2} 테이블 이름
        public static string LoadDatabaseContext =
            @"
            {0} = await LoadTable<{1}>(""{2}"");";

        // {0} 데이터 구조체
        // {1} 대문자 이름
        public static string GetDataContext =
            @"
        public {0} Get{1}(int id)
        {{
            return {1}.Get(id);
        }}
            ";

        // {0} 데이터 구조체
        // {1} 대문자 이름
        public static string GetDataListContext =
            @"
        public Dictionary<int, {0}> Get{1}List()
        {{
            return {1}.GetList();
        }}
            ";

        public static string GetDataFuncContext =
    @"
        public {0} Get{1}(int id);";

        // {0} 데이터 구조체
        // {1} 대문자 이름
        public static string GetDataListFuncContext =
            @"
        public Dictionary<int, {0}> Get{1}List();";

        // {0} 소문자 이름
        public static string GetValidateContext =
            @"
        {0} == null ||";


      












    }
}
