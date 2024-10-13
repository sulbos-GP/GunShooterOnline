using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Utils;
using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.Servicies.Matching;
using System.Text.RegularExpressions;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.GameDatabase;
using WebCommonLibrary.Models.GameDB;

namespace GsoWebServer.Servicies.RatingSystem
{
    public class PlayerPerformanceService : IPlayerPerformanceService
    {

        private readonly IGameDB mGameDB;
        public PlayerPerformanceService(IGameDB gameDB)
        {
            mGameDB = gameDB;
        }

        public void Dispose()
        {

        }

        public async Task<WebErrorCode> UpdatePlayerStats(int uid, MatchOutcome outcome)
        {
            try
            {
                var metadata = await mGameDB.GetUserMetadataByUid(uid);
                if (metadata == null)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                metadata.total_games += 1;
                metadata.kills += outcome.kills;
                metadata.deaths += outcome.death;
                metadata.damage += outcome.damage;
                metadata.farming += outcome.farming;
                metadata.escape += outcome.escape;
                metadata.survival_time += outcome.survival_time;

                var result = await mGameDB.UpdateUserMetadata(uid, metadata);
                if(result == 0)
                {
                    return WebErrorCode.TEMP_ERROR;
                }

                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdatePlayerStats] : {ex.Message}");
                return WebErrorCode.TEMP_Exception;
            }
        }

        public int CalculateExperience(MatchOutcome outcome)
        {
            try
            {
                Glicko2 glicko2 = new Glicko2();
                return glicko2.CalculateExperience(outcome);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CalculateExperience] : {ex.Message}");
                return 0;
            }
        }

        public async Task<WebErrorCode> UpdatePlayerRating(Dictionary<int, MatchOutcome> outcomes)
        {

            var transaction = mGameDB.GetConnection().BeginTransaction();
            try
            {
                Dictionary<int, FUserSkill> skills = new Dictionary<int, FUserSkill>();
                foreach ((int uid, MatchOutcome outcome) in outcomes)
                {
                    var skill = await mGameDB.GetUserSkillByUid(uid, transaction);
                    if (skill == null)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }
                    skills.Add(uid, skill);
                }

                foreach ((int uid, MatchOutcome outcome) in outcomes)
                {
                    Glicko2 glicko2 = new Glicko2();
                    FUserSkill newSkill = glicko2.UpdatePlayerRating(uid, skills, outcomes);

                    var result = await mGameDB.UpdateUserSkill(uid, newSkill, transaction);
                    if (result == 0)
                    {
                        return WebErrorCode.TEMP_ERROR;
                    }
                }

                transaction.Commit();
                return WebErrorCode.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdatePlayerStats] : {ex.Message}");
                transaction.Rollback();
                return WebErrorCode.TEMP_Exception;
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
