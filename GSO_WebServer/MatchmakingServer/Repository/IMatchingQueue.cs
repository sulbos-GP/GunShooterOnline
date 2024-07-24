using CloudStructures.Structures;
using GSO_WebServerLibrary;
using Matchmaker.Models;

namespace Matchmaker.Repository
{
    public interface IMatchingQueue : IDisposable
    {
        public Task<WebErrorCode> PushAsync(long uid, double rating, double deviation);

        public Task<WebErrorCode> PopAsync(long uid);

        public Task<Tuple<WebErrorCode, List<MatchQueueInfo>?>> ScanPlayers();

        public Task<Tuple<WebErrorCode, MatchQueueInfo[]?>> FindMatch(double minRating, double maxRating, int number);

        public Task<Tuple<WebErrorCode, bool>> RemovePlayers(MatchQueueInfo[] players);

        public Task<Tuple<WebErrorCode, bool>> UpdatePlayer(MatchQueueInfo newInfo);
    }
}
