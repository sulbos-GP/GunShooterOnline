using GSO_WebServerLibrary.Error;
using GsoWebServer.DTO.Middleware;
using GSO_WebServerLibrary.Models.MemoryDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Reposiotry.Define.MemoryDB;
using GsoWebServer.Servicies.Interfaces;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using GsoWebServer.DTO.Authentication;
using GSO_WebServerLibrary.DTO.Match;
using System.Text;

namespace GsoWebServer.Middleware
{
    public class CheckUserAuth
    {
        readonly IAuthenticationService authenticationService;
        readonly IMemoryDB mMemoryDB;
        readonly IGameDB mGameDB;
        readonly RequestDelegate mNext;

        public CheckUserAuth(RequestDelegate next, IMemoryDB memoryDb, IGameDB gameDB, IAuthenticationService authcationService)
        {
            authenticationService = authcationService;
            mMemoryDB = memoryDb;
            mGameDB = gameDB;
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
            if (formString.EndsWith("/Authentication", StringComparison.OrdinalIgnoreCase) == true ||
                formString.EndsWith("/SignIn", StringComparison.OrdinalIgnoreCase) == true)
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

            var userData = await mGameDB.GetUserByUid(uid);
            if (userData == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, WebErrorCode.TEMP_ERROR);
                return;
            }

            //uid와 token을 통해 정보가 있는지 확인
            var (error, user) = await mMemoryDB.ValidateAndGetUserData(uid);
            if (user == null)
            {

                //갱신 토큰을 이용하여 엑세스 토큰 갱신하기
                //(error, var refreshTokenData) = await mMemoryDB.ValidateAndGetRefreshToken(uid);
                (error, var accessTokenData) = await authenticationService.RefreshToken(userData.player_id, userData.refresh_token);
                if (accessTokenData == null || accessTokenData.ExpiresInSeconds == null)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, error);
                    return;
                }

                error = await mMemoryDB.RegisterAuthUserData(uid, userData.player_id, accessTokenData.AccessToken, accessTokenData.ExpiresInSeconds.Value);
                if (error != WebErrorCode.None)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status401Unauthorized, error);
                    return;
                }

                user = new AuthUserDataInfo
                {
                    uid = uidstr,
                    user_id = userData.player_id,
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
                error_code = error
            });
            await context.Response.WriteAsync(errorJsonResponse);
        }

        //async Task SendRefreshTokenResponse(HttpContext context, int uid, WebErrorCode error)
        //{

        //    HttpClient client = mHttpClientFactory.CreateClient("Authorization");

        //    RefreshTokenReq body = new RefreshTokenReq
        //    {
        //        uid = uid
        //    };
        //    var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        //    var response = await client.PostAsync("api/Authorize/RefreshToken", content);
        //    response.EnsureSuccessStatusCode();

        //    var newToken = await response.Content.ReadFromJsonAsync<RefreshTokenRes>();
        //    if (newToken == null)
        //    {
        //        await SendMiddlewareResponse(context, StatusCodes.Status403Forbidden, WebErrorCode.FailedRefreshToken);
        //    }else
        //    {
        //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        var errorJsonResponse = JsonSerializer.Serialize(newToken);
        //        await context.Response.WriteAsync(errorJsonResponse);
        //    }
        //}

        private string? GetValueOrNull(string key, HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(key, out var value) == false)
            {
                return null;
            }
            return value;
        }

    }
}
