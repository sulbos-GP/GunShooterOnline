using Server.Docker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.DTO.Performance;
using WebCommonLibrary.Error;

namespace Server.Web.Service
{
    public class LobbyResource : WebResource
    {
        public LobbyResource(WebServer owner, string name) : base(owner, name)
        {

        }

        /// <summary>
        /// 플레이어의 개인 기록에 대한 평가 반영
        /// </summary>
        public async Task<PlayerStatsRes> PostPlayerStats(int uid, MatchOutcome outcome)
        {


            Console.WriteLine("[Post PlayerStats to lobby]");

            PlayerStatsReq request = new PlayerStatsReq
            {
                room_token = DockerUtil.GetContainerId(),
                uid = uid,
                outcome = outcome,
            };
            return await Owner.PostAsync<PlayerStatsRes>(Host, "Performance/PlayerStats", request);
        }

        /// <summary>
        /// 전체 게임에서 플레이어의 레이팅 평가 반영
        /// </summary>
        public async Task<PlayerRatingRes> PostPlayerRating(Dictionary<int, MatchOutcome> outcomes)
        {

            Console.WriteLine("[Post PlayerRating to lobby]");

            PlayerRatingReq request = new PlayerRatingReq
            {
                room_token = DockerUtil.GetContainerId(),
                outcomes = outcomes,
            };

            return await Owner.PostAsync<PlayerRatingRes>(Host, "Performance/PlayerRating", request);
        }
    }
}
