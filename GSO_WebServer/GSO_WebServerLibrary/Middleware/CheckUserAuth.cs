using GSO_WebServerLibrary.Reposiotry.Interfaces;
using System.Text.Json;
using WebCommonLibrary.Error;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Models.MemoryDB;
using GSO_WebServerLibrary.Servicies.Interfaces;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth.OAuth2.Responses;
using WebCommonLibrary.DTO.Authentication;
using Newtonsoft.Json.Linq;

namespace GSO_WebServerLibrary.Middleware
{
    public class CheckUserAuth
    {
        private readonly IGoogleService mGoogleService;
        private readonly IMemoryDB mMemoryDB;
        private readonly IGameDB mGameDB;
        private readonly RequestDelegate mNext;

        protected List<string> mIgnoreEndPoints = new List<string>();

        public CheckUserAuth(RequestDelegate next, IMemoryDB memoryDb, IGameDB gameDB, IGoogleService googleService)
        {
            mGoogleService = googleService;
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

            var formString = context.Request.Path.Value;
            foreach(string endPoint in mIgnoreEndPoints)
            {
                if (formString.EndsWith(endPoint, StringComparison.OrdinalIgnoreCase) == true)
                {
                    await mNext(context);
                    return;
                }
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
                await SendMiddlewareResponse(context, StatusCodes.Status404NotFound, WebErrorCode.TEMP_ERROR);
                return;
            }

            //uid와 token을 통해 정보가 있는지 확인

            var error = await mGoogleService.ValidateToken(userData.player_id, token);
            AuthUserDataInfo? user = null;
            if (error == WebErrorCode.None)
            {
                (error, user) = await mMemoryDB.ValidateAndGetUserData(uid);
            }

            if (error != WebErrorCode.None || user == null)
            {
                if(userData.refresh_token == null)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status404NotFound, error);
                    return;
                }

                //갱신 토큰을 이용하여 엑세스 토큰 갱신하기
                (error, var accessTokenData) = await mGoogleService.RefreshToken(userData.player_id, userData.refresh_token);
                if (accessTokenData == null || accessTokenData.ExpiresInSeconds == null)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status404NotFound, error);
                    return;
                }

                error = await mMemoryDB.RegisterAuthUserData(uid, userData.player_id, accessTokenData.AccessToken, accessTokenData.ExpiresInSeconds.Value);
                if (error != WebErrorCode.None)
                {
                    await SendMiddlewareResponse(context, StatusCodes.Status404NotFound, error);
                    return;
                }

                await SendRefreshResponse(context, uid, accessTokenData, StatusCodes.Status401Unauthorized, WebErrorCode.AccessTokenIsExpries);
                return;
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

        async Task SendRefreshResponse(HttpContext context, int uid, TokenResponse token, int statusCode, WebErrorCode error)
        {

            if(!token.ExpiresInSeconds.HasValue)
            {
                return;
            }

            context.Response.StatusCode = statusCode;
            var errorJsonResponse = JsonSerializer.Serialize(new RefreshTokenRes
            {
                error_code = error,
                error_description = "토큰을 재 발급하였습니다.",

                uid = uid,
                access_token = token.AccessToken,
                expires_in = token.ExpiresInSeconds.Value,
                scope = token.Scope,
                token_type = token.TokenType,
            });
            await context.Response.WriteAsync(errorJsonResponse);
        }

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
