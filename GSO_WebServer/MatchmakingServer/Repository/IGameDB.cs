using GSO_WebServerLibrary;
using MatchmakingServer.Models;

namespace MatchmakingServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<Tuple<WebErrorCode, Int64>> GetUID(String userid);
        public Task<Tuple<WebErrorCode, PlayerSkill?>> GetUserSkill(Int64 uid);
        //public Task<WebErrorCode> SetUserSkill(Int64 uid, Double mu, Double sigma);
    }
}
