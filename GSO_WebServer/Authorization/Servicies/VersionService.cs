using Authorization.Servicies.Interfaces;
using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using Microsoft.Extensions.Options;

namespace Authorization.Servicies
{
    public class VersionService : IVersionServicie
    {
        private readonly IMasterDB mMasterDB;

        public VersionService(IMasterDB masterDB)
        {
            mMasterDB = masterDB;
        }

        public void Dispose()
        {

        }

        public async Task<Version?> GetLatestAppVersion()
        {
            try
            {
                var version = await mMasterDB.LoadLatestAppVersion();
                return new Version(version.major, version.minor, version.patch);
            }
            catch
            {
                return null;
            }
        }


        public async Task<Version?> GetLatestDataVersion()
        {
            try
            {
                var version = await mMasterDB.LoadLatestDataVersion();
                return new Version(version.major, version.minor, version.patch);
            }
            catch
            {
                return null;
            }
        }

    }
}
