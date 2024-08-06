using GSO_WebServerLibrary.DTO;
using GSO_WebServerLibrary.Error;
using Matchmaker.DTO;
using System.Text.Json;

namespace Matchmaker.Middleware
{
    public class GoogleAuthCheck
    {

        readonly RequestDelegate mNext;

        public GoogleAuthCheck(RequestDelegate next)
        {
            mNext = next;
        }

        public async Task Invoke(HttpContext context)
        {

            //경로가 null일 경우
            if (context.Request.Path.Value == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
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

            //



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

        private string? GetValueOrNull(string key, HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(key, out var token) == false)
            {
                return null;
            }
            return token;
        }

        public class MiddlewareResponse : ErrorCodeDTO
        {

        }
    }
}
