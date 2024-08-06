using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.Models.Config;
using Microsoft.Extensions.Options;

namespace GsoWebServer.Servicies.Google
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
