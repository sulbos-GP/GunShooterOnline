using GSO_WebServerLibrary;
using GsoWebServer.Models.MemoryDB;
using static Google.Apis.Requests.RequestError;

namespace GsoWebServer.Reposiotry.Interfaces
{
    public interface IMemoryDB : IDisposable
    {
        /// <summary>
        /// 유저의 인증 데이터 저장
        /// </summary>
        public Task<WebErrorCode> RegisterAuthUserData(Int32 uid, String userID, String accessToken, Int64 expires);

        /// <summary>
        /// 유저의 인증 데이터 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveAuthUserData(Int32 uid);

        /// <summary>
        /// 유저의 갱신 토큰 저장
        /// </summary>
        public Task<WebErrorCode> RegisterRefreshToken(Int32 uid, String userID, String refreshToken);

        /// <summary>
        /// 유저의 갱신 토큰 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveRefreshToken(Int32 uid);

        /// <summary>
        /// 유저의 uid와 토큰이 존재하는지 확인
        /// </summary>
        public Task<(WebErrorCode, AuthUserDataInfo?)> ValidateAndGetUserData(Int32 uid);

        /// <summary>
        /// 유저의 갱신 토큰이 존재하는지 확인
        /// </summary>
        public Task<(WebErrorCode, RefreshDataInfo?)> ValidateAndGetRefreshToken(Int32 uid);

        /// <summary>
        /// 유저가 API을 실행하는 동안 락을 걸어준다
        /// </summary>
        public Task<WebErrorCode> RegisterLockAuthUserData(Int32 uid);

        /// <summary>
        /// 유저가 API을 실행하였다면 락을 풀어준다
        /// </summary>
        public Task<WebErrorCode> RemoveLockAuthUserData(Int32 uid);
    }
}
