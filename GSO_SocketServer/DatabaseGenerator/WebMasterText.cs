using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class WebMasterText
    {
        // {0} ContextEnum
        // {1} ContextRegion
        // {2} GetContext
        // {3} LoadDatabaseContext
        // {4} GetDataContext
        public static string context =
            @"using WebCommonLibrary.Models.MasterDB;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{{

    public interface IMasterDataDB : IDisposable
    {{

        public DB_Version GetAppVersion();
        public DB_Version GetDataVersion();

        {0}
    
        {1}
    }}
}}";
    }
}
