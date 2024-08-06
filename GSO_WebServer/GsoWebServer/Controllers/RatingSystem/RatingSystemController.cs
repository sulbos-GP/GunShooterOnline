using GSO_WebServerLibrary;
using GsoWebServer.Models.Statistic;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StatisticServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingSystemController : ControllerBase
    {
        private readonly IGameDB mGameDB_Skills;
        private readonly IRatingSystemService mRatingSystem;

        public RatingSystemController(IGameDB gameDB, IRatingSystemService ratingSystem)
        {
            mGameDB_Skills = gameDB;
            mRatingSystem = ratingSystem;
        }

        /// <summary>
        /// 매칭에 관한 결과
        /// </summary>
        //[HttpPost]
        //[Route("MatchOutcome")]
        //public async Task<MatchOutComeRes> MatchOutcome([FromBody] MatchOutComeReq request)
        //{

        //    var response = new MatchOutComeRes();
        //    if (!WebUtils.IsValidModelState(request))
        //    {
        //        response.error = WebErrorCode.IsNotValidModelState;
        //        return response;
        //    }

        //    if(request.user_id == null || request.user_match_outcome == null)
        //    {
        //        response.error = WebErrorCode.IsNotValidModelState;
        //        return response;
        //    }

        //    Console.WriteLine($"[MatchOutcome] outcome:{request.user_id.Count}");

        //    //유저아디이와 토큰 확인
        //    List<string> user_ids = request.user_id;

        //    foreach (var user_id in user_ids)
        //    {
        //        var Skills = await mGameDB_Skills.SelectPlayerSkills(user_id);
        //    }

        //    List<MatchOutcome> outcomes = request.user_match_outcome;

        //    mRatingSystem.UpdatePlayerRatings(uids, outcomes);
        //}
    }
}
