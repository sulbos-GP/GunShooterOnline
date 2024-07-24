using GSO_WebServerLibrary;
using GsoWebServer.DTO.Middleware;
using GsoWebServer.Models.MemoryDB;
using GsoWebServer.Reposiotry.Interfaces;
using GsoWebServer.Reposiotry.NoSQL;
using GsoWebServer.Servicies.Interfaces;
using System.Text.Json;

namespace GsoWebServer.Middleware
{
    public class CheckUserAuth
    {
        readonly IAuthenticationService authenticationService;
        readonly IMemoryDB mMemoryDB;
        readonly RequestDelegate mNext;

        public CheckUserAuth(RequestDelegate next, IMemoryDB memoryDb, IAuthenticationService authcationService)
        {
            authenticationService = authcationService;
            mMemoryDB = memoryDb;
            mNext = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            //경로가 null일 경우
            if(context.Request.Path.Value == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                return;
            }

            //로그인은 제외
            var formString = context.Request.Path.Value;
            if (formString.EndsWith("/SignIn", StringComparison.OrdinalIgnoreCase) == true)
            {
                await mNext(context);
                return;
            }

            //Header에 access_token가 포함되어 있는지 확인
            var token = GetValueOrNull("access_token", context);
            if (token == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                return;
            }

            //Header에 Uid가 포함되어 있는지 확인
            var uidstr = GetValueOrNull("uid", context);
            if (uidstr == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                return;
            }
            Int32 uid = Convert.ToInt32(uidstr);

            //uid와 token을 통해 정보가 있는지 확인
            var (error, user) = await mMemoryDB.ValidateAndGetUserData(uid);
            if (user == null)
            {

                //갱신 토큰을 이용하여 엑세스 토큰 갱신하기
                (error, var refreshTokenData) = await mMemoryDB.ValidateAndGetRefreshToken(uid);
                if (refreshTokenData == null)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, error);
                    return;
                }

                (error, var accessTokenData) = await authenticationService.RefreshToken(refreshTokenData.user_id, refreshTokenData.refresh_token);
                if (accessTokenData == null || accessTokenData.ExpiresInSeconds == null)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, error);
                    return;
                }

                error = await mMemoryDB.RegisterAuthUserData(uid, refreshTokenData.user_id, accessTokenData.AccessToken, accessTokenData.ExpiresInSeconds.Value);
                if (error != WebErrorCode.None)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, error);
                    return;
                }

                user = new AuthUserDataInfo
                {
                    uid = uidstr,
                    user_id = refreshTokenData.user_id,
                    access_token = accessTokenData.AccessToken,
                };

            }

            //이번 api 호출 끝날 때까지 redis키 잠금
            //만약 잠겨있다면 호출이 중복되므로 에러

            error = await mMemoryDB.RegisterLockAuthUserData(uid);
            if (error != WebErrorCode.None)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status429TooManyRequests, error);
                return;
            }

            context.Items[nameof(AuthUserDataInfo)] = user;

            // Call the next delegate/middleware in the pipeline
            await mNext(context);

            // 트랜잭션 해제(Redis 동기화 해제)
            await mMemoryDB.RemoveLockAuthUserData(uid);
        }

        async Task SendMiddlewareResponse(HttpContext context, int statusCode, WebErrorCode error)
        {
            context.Response.StatusCode = statusCode;
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                error = error
            });
            await context.Response.WriteAsync(errorJsonResponse);
        }

        private string? GetValueOrNull(string key, HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(key, out var token) == false)
            {
                return null;
            }
            return token;
        }

    }
}
