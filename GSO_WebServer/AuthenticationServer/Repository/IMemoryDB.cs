using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Options;

namespace AuthenticationServer.Repository
{
    public interface IMemoryDB : IDisposable
    {
        public Task<ErrorCode> RegistUserAsync(long uid, long expires, string accessToken, string refreshToken);
    }
}
