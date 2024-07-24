using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Matchmaker.Repository;
using Matchmaker.Models;
using GSO_WebServerLibrary;

namespace Matchmaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchmakingController : ControllerBase
    {
        private readonly IGameDB mGameDB;
        private readonly IMatchingQueue mMatchingQueue;

        public MatchmakingController(IGameDB gameDB, IMatchingQueue matchingQueue)
        {
            mGameDB = gameDB;
            mMatchingQueue = matchingQueue;
        }

        /// <summary>
        /// 매칭 시작
        /// </summary>
        [HttpPost]
        [Route("Join")]
        public async Task<JoinMatchMakingRes> Join([FromBody] JoinMatchMakingReq request)
        {

            //로그

            var response = new JoinMatchMakingRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error = WebErrorCode.IsNotValidModelState;
                return response;
            }

            //MySql에서 UID, Skill 얻기
            var uidResult = await mGameDB.GetUID(request.user_id);
            if(uidResult.Item1 != WebErrorCode.None)
            {
                return response;
            }

            var skillResult = await mGameDB.GetUserSkill(uidResult.Item2);
            if (skillResult.Item1 != WebErrorCode.None || skillResult.Item2 == null)
            {
                return response;
            }

            //TODO : UID를 이용하여 엑세스 토큰이 올바른지 확인


            //TODO : 없다면 RefreshToken API 사용


            //이미 큐에 들어와 있는지 확인 + 삽입
            var isQueue = await mMatchingQueue.PushAsync(uidResult.Item2, skillResult.Item2.rating, skillResult.Item2.deviation);
            if(isQueue != WebErrorCode.None)
            {
                return response;
            }

            response.error = WebErrorCode.None;
            return response;
        }

        /// <summary>
        /// 매칭 취소
        /// </summary>
        [HttpPost]
        [Route("Cancle")]
        public async Task<CancleMatchMakingRes> Cancle([FromBody] CancleMatchMakingReq request)
        {
            //로그

            var response = new CancleMatchMakingRes();

            if (!WebUtils.IsValidModelState(request))
            {
                response.error = WebErrorCode.IsNotValidModelState;
                return response;
            }

            //MySql에서 UID 얻기
            var uidResult = await mGameDB.GetUID(request.user_id);
            if (uidResult.Item1 != WebErrorCode.None)
            {
                response.error = uidResult.Item1;
                return response;
            }

            //TODO : UID를 이용하여 엑세스 토큰이 올바른지 확인

            //TODO : 없다면 RefreshToken API 사용

            //방이 잡혔는지 확인

            //Redis에 UID, Rating 제거
            var popResult = await mMatchingQueue.PopAsync(uidResult.Item2);
            if (popResult != WebErrorCode.None)
            {
                response.error = popResult;
                return response;
            }

            response.error = WebErrorCode.None;
            return response;
        }
    }
}
