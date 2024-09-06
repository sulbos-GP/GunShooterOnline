using Server.Docker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.RatingSystem;

namespace Server.Web.Service
{
    public class LobbyResource : WebResource
    {
        public LobbyResource(WebServer owner, string name) : base(owner, name)
        {

        }

        public async Task<MatchOutComeRes> PostMatchOutcome(Dictionary<int, MatchOutcomeInfo> outcomes)
        {

            Console.WriteLine("[MatchOutcome Lobby]");

            MatchOutComeReq request = new MatchOutComeReq
            {
                room_token = DockerUtil.GetContainerId(),
                outcomes = outcomes,
            };

            return await Owner.PostAsync<MatchOutComeRes>(Host, "Match/Outcome", request);
        }
    }
}
