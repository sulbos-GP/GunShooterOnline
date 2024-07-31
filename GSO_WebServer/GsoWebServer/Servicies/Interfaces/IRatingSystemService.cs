

using GSO_WebServerLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.Interfaces
{
    public interface IRatingSystemService : IDisposable
    {
        public UserSkillInfo UpdatePlayerRating(UserSkillInfo skill, Dictionary<int, Tuple<UserSkillInfo, MatchOutcomeInfo>> matches);
    }
}
