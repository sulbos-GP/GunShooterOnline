using GSO_WebServerLibrary;
using GsoWebServer.DTO;
using GsoWebServer.Models.GameDB;
using GsoWebServer.Models.Statistic;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace StatisticServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingSystemController : ControllerBase
    {
        private readonly IGameService mGameService;
        private readonly IRatingSystemService mRatingSystem;

        public RatingSystemController(IGameService gameservice, IRatingSystemService ratingSystem)
        {
            mGameService = gameservice;
            mRatingSystem = ratingSystem;
        }

        /// <summary>
        /// 매칭에 관한 결과
        /// </summary>
        [HttpPost]
        [Route("MatchOutcome")]
        public async Task<MatchOutComeRes> MatchOutcome([FromHeader] HeaderDTO header, [FromBody] MatchOutComeReq request)
        {

            var response = new MatchOutComeRes();
            if (!WebUtils.IsValidModelState(request))
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            Dictionary<int, Tuple<UserSkillInfo, MatchOutcomeInfo>> matches = new Dictionary<int, Tuple<UserSkillInfo, MatchOutcomeInfo>>();
            if (request.outcomes == null)
            {
                response.error_code = WebErrorCode.IsNotValidModelState;
                response.error_description = "";
                return response;
            }

            foreach (var match in request.outcomes)
            {
                int uid = match.Key;
                var outcome = match.Value;

                var (error, skill) = await mGameService.GetSkillInfo(uid);
                if (error != WebErrorCode.None || skill == null)
                {
                    response.error_code = WebErrorCode.TEMP_ERROR;
                    response.error_description = "";
                    return response;
                }

                matches.Add(uid, new(skill, outcome));


                (error, var metadata) = await mGameService.GetMetadataInfo(uid);
                if (error == WebErrorCode.None && metadata != null)
                {

                    metadata.total_games    += 1;
                    metadata.kills          += outcome.kills;
                    metadata.deaths         += outcome.death;
                    metadata.damage         += outcome.damage;
                    metadata.farming        += outcome.farming;
                    metadata.escape         += outcome.escape;
                    metadata.survival_time  += outcome.survival_time;

                    await mGameService.UpdateUserMetadata(uid, metadata);
                }
            }

            foreach(var match in matches)
            {
                UserSkillInfo newSkill = mRatingSystem.UpdatePlayerRating(match.Value.Item1, matches);
                await mGameService.UpdateUserSkill(match.Key, newSkill);
            }

            response.error_code = WebErrorCode.None;
            return response;
        }
    }
}
