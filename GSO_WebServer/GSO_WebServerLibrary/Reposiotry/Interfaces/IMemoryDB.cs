using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Models.MemoryDB;

namespace GSO_WebServerLibrary.Reposiotry.Interfaces
{
    public interface IMemoryDB : IDisposable
    {
        /// <summary>
        /// 유저의 인증 데이터 저장
        /// </summary>
        public Task<WebErrorCode> RegisterAuthUserData(int uid, string userID, string accessToken, long expires);

        /// <summary>
        /// 유저의 인증 데이터 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveAuthUserData(int uid);

        /// <summary>
        /// 유저의 갱신 토큰 저장
        /// </summary>
        public Task<WebErrorCode> RegisterRefreshToken(int uid, string userID, string refreshToken);

        /// <summary>
        /// 유저의 갱신 토큰 삭제
        /// </summary>
        public Task<WebErrorCode> RemoveRefreshToken(int uid);

        /// <summary>
        /// 유저의 uid와 토큰이 존재하는지 확인
        /// </summary>
        public Task<(WebErrorCode, AuthUserDataInfo?)> ValidateAndGetUserData(int uid);

        /// <summary>
        /// 유저의 갱신 토큰이 존재하는지 확인
        /// </summary>
        public Task<(WebErrorCode, RefreshDataInfo?)> ValidateAndGetRefreshToken(int uid);

        /// <summary>
        /// 유저가 API을 실행하는 동안 락을 걸어준다
        /// </summary>
        public Task<WebErrorCode> RegisterLockAuthUserData(int uid);

        /// <summary>
        /// 유저가 API을 실행하였다면 락을 풀어준다
        /// </summary>
        public Task<WebErrorCode> RemoveLockAuthUserData(int uid);
    }
}
