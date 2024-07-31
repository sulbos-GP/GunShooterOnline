using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.Match;
using Matchmaker.Models;

namespace Matchmaker.Service.Interfaces
{
    public interface IGameServerManagerService : IDisposable
    {
        /// <summary>
        /// 매칭정보에 따른 방이 있는지 확인하고 보내준다
        /// </summary>
        public Task<(WebErrorCode, MatchProfile?)> FetchMatchInfo();
    }
}
