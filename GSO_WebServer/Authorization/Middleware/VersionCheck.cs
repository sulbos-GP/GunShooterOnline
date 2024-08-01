using GSO_WebServerLibrary.DTO.Middleware;
using GSO_WebServerLibrary.Error;
using System.Text.Json;

namespace Authorization.Middleware
{
    public class VersionCheck
    {
        readonly RequestDelegate _next;
        readonly ILogger<VersionCheck> _logger;
        readonly IMasterDb _masterDb;

        public VersionCheck(RequestDelegate next, ILogger<VersionCheck> logger, IMasterDb masterDb)
        {
            _next = next;
            _logger = logger;
            _masterDb = masterDb;
        }

        public async Task Invoke(HttpContext context)
        {
            
            var appVersion = GetValueOrNull("AppVersion", context);
            var masterDataVersion = GetValueOrNull("MasterDataVersion", context);

            if (appVersion == null || masterDataVersion == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.IncorrectHeaderContext);
                return;
            }

            if (!(await VersionCompare(appVersion, masterDataVersion, context)))
            {
                return;
            }

            await _next(context);
        }

        async Task<bool> VersionCompare(string appVersion, string masterDataVersion, HttpContext context)
        {
            if (!appVersion.Equals(_masterDb._version!.app_version))
            {
                await SendMiddlewareResponse(context, StatusCodes.Status426UpgradeRequired, WebErrorCode.DiscrepancyAppVersion);
                return false;
            }

            if (!masterDataVersion.Equals(_masterDb._version!.master_data_version))
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
