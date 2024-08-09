using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Config
{
    public class EnvironmentConfig
    {
        public DatabaseConfig mDatabase { get; set; } = new DatabaseConfig();
        public EndPointConfig mEndPoint {  get; set; } = new EndPointConfig();
    }

    public class LocalConfig : EnvironmentConfig
    {

    }

    public class AwsConfig : EnvironmentConfig
    {

    }
}
