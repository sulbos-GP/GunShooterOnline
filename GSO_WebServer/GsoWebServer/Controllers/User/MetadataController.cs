using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCommonLibrary.DTO.User;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using GsoWebServer.DTO;

namespace GsoWebServer.Controllers.User
{
    [Route("api/User/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IAuthenticationService mAuthenticationService;
        private readonly IGameService mGameService;

        public MetadataController(IAuthenticationService auth, IGameService game)
        {
            mAuthenticationService = auth;
            mGameService = game;
        }

        [HttpPost]
        [Route("Load")]
        public async Task<LoadMetadataRes> Load([FromHeader] HeaderDTO header, [FromBody] LoadMetadataReq request)
        {
            Console.WriteLine($"[user.Metadata.Load] uid:{header.uid}");

            var response = new LoadMetadataRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            var (error, metadata) = await mGameService.GetMetadataInfo(header.uid);
            if (error != WebErrorCode.None || metadata == null)
            {
                response.error_code = error;
                return response;
            }

            response.metadata = metadata;
            response.error_code = WebErrorCode.None;
            return response;
        }
    }

}
