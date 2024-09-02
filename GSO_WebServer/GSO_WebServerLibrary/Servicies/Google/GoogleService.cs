using Microsoft.Extensions.Options;
using WebCommonLibrary.Config;
using GSO_WebServerLibrary.Servicies.Interfaces;

namespace GSO_WebServerLibrary.Servicies.Google
{
    public partial class GoogleService : IGoogleService
    {
        private readonly IOptions<GoogleConfig> mGoogleConfig;

        public GoogleService(IOptions<GoogleConfig> googleConfig)
        {
            mGoogleConfig = googleConfig;
        }

        public void Dispose()
        {

        }
    }
}
