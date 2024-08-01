using GSO_WebServerLibrary.DTO.Match;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.Match;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameServerManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        /// <summary>
        /// 가능한 매칭이 있는지 확인
        /// </summary>
        [HttpPost]
        [Route("FetchMatch")]
        public async Task<FetchMatchRes> FetchMatch([FromBody] FetchMatchReq request)
        {
            var response = new FetchMatchRes();

            var profile = new MatchProfile();
            profile.match_id = "TEST_TOKEN";
            profile.host = "127.0.0.1";
            profile.port = 7777;

            response.error_code = WebErrorCode.None;
            response.match_profile = profile;

            return response;
        }
    }
}
