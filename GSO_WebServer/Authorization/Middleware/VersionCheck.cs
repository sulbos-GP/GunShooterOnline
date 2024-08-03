using Authorization.Servicies;
using Authorization.Servicies.Interfaces;
using GSO_WebServerLibrary.DTO.Middleware;
using GSO_WebServerLibrary.Error;
using GSO_WebServerLibrary.Reposiotry.Define.MasterDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using System.Text.Json;

namespace Authorization.Middleware
{
    public class VersionCheck
    {
        readonly RequestDelegate mNext;
        readonly IVersionServicie mVersionService;

        public VersionCheck(RequestDelegate next, IVersionServicie versionService)
        {
            mNext = next;
            mVersionService = versionService;
        }

        public async Task Invoke(HttpContext context)
        {
            
            var appVersion = GetValueOrNull("app_version", context);
            var masterDataVersion = GetValueOrNull("data_version", context);

            if (appVersion == null || masterDataVersion == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.IncorrectHeaderContext);
                return;
            }

            if (!(await VersionCompare(appVersion, masterDataVersion, context)))
            {
                return;
            }

            await mNext(context);
        }

        async Task<bool> VersionCompare(string appVersion, string masterDataVersion, HttpContext context)
        {

            var clientAppVersion = new Version(appVersion);
            var serverAppVersion = await mVersionService.GetLatestAppVersion();
            if (serverAppVersion == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.InvalidVersion);
                return false;
            }

            if(0 != serverAppVersion.CompareTo(clientAppVersion))
            {
                await SendMiddlewareResponse(context, StatusCodes.Status426UpgradeRequired, WebErrorCode.DiscrepancyAppVersion);
                return false;
            }

            var clientDataVersion = new Version(masterDataVersion);
            var serverDataVersion = await mVersionService.GetLatestDataVersion();
            if (serverDataVersion == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.InvalidVersion);
                return false;
            }

            if (0 != serverDataVersion.CompareTo(clientDataVersion))
            {
                await SendMiddlewareResponse(context, StatusCodes.Status426UpgradeRequired, WebErrorCode.DiscrepancyDataVersion);
                return false;
            }

            return true;
        }

        async Task SendMiddlewareResponse(HttpContext context, int statusCode, WebErrorCode error, string description = "")
        {
            context.Response.StatusCode = statusCode;
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                error_code = error,
                error_description = description
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
