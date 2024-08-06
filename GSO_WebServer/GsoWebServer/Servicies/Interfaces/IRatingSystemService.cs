

using GsoWebServer.Models.GameDB;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IRatingSystemService : IDisposable
    {
        public Dictionary<long, SkillInfo> UpdatePlayerRatings(List<long> uids, List<SkillInfo> skills, List<MatchOutcomeInfo> outcomes);
    }
}
