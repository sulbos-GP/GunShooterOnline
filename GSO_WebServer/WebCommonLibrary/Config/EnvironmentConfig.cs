
namespace WebCommonLibrary.Config
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
