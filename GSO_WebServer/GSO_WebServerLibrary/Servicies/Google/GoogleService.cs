using Microsoft.Extensions.Options;
using GSO_WebServerLibrary.Servicies.Google.Interface;
using GSO_WebServerLibrary.Config;

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
