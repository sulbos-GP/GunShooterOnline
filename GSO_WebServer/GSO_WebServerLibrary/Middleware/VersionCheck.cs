using GSO_WebServerLibrary.Reposiotry.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebCommonLibrary.DTO.Middleware;
using WebCommonLibrary.Error;
using WebCommonLibrary.Models.MasterDatabase;

namespace GSO_WebServerLibrary.Middleware
{

    public class VersionCheck
    {
        readonly RequestDelegate mNext;
        readonly IMasterDB mMasterDB;

        protected List<string> mUseEndPoints = new List<string>();

        public VersionCheck(RequestDelegate next, IMasterDB masterDb)
        {
            mNext = next;
            mMasterDB = masterDb;
        }

        public async Task Invoke(HttpContext context)
        {
            //경로가 null일 경우
            if (context.Request.Path.Value == null)
            {
                await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                return;
            }

            var formString = context.Request.Path.Value;
            foreach (string endPoint in mUseEndPoints)
            {
                if (formString.EndsWith(endPoint, StringComparison.OrdinalIgnoreCase) == true)
                {
                    var appVersion = context.Request.Headers["AppVersion"].ToString();
                    var masterDataVersion = context.Request.Headers["MasterDataVersion"].ToString();

                    var app = GetValueOrNull("AppVersion", context);
                    if (app == null)
                    {
                        await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                        return;
                    }

                    var data = GetValueOrNull("MasterDataVersion", context);
                    if (data == null)
                    {
                        await SendMiddlewareResponse(context, StatusCodes.Status400BadRequest, WebErrorCode.TEMP_ERROR);
                        return;
                    }

                    FMasterVersionApp masterVersionApp = mMasterDB.Context.MasterVersionApp.FirstOrDefault().Value;
                    string leastVersionAppString = "1.0.0";//masterVersionApp.major.ToString() + "." + masterVersionApp.minor.ToString() + "." + masterVersionApp.patch.ToString();
                    if (!app.Equals(leastVersionAppString))
                    {
                        await SendMiddlewareResponse(context, StatusCodes.Status426UpgradeRequired, WebErrorCode.DiscrepancyAppVersion);
                        return;
                    }

                    FMasterVersionData masterVersionData = mMasterDB.Context.MasterVersionData.FirstOrDefault().Value;
                    string masterVersionDataString = "1.0.0";//masterVersionData.major.ToString() + "." + masterVersionApp.minor.ToString() + "." + masterVersionApp.patch.ToString();
                    if (!data.Equals(masterVersionDataString))
                    {
                        await SendMiddlewareResponse(context, StatusCodes.Status426UpgradeRequired, WebErrorCode.DiscrepancyDataVersion);
                        return;
                    }

                    break;
                }
            }

            await mNext(context);
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

        async Task SendVersionResponse(HttpContext context, int statusCode, WebErrorCode error)
        {
            context.Response.StatusCode = statusCode;
            var errorJsonResponse = JsonSerializer.Serialize(new UpgradeVersionRes
            {
                error_code = error,
                error_description = "새로운 앱 또는 버전이 존재합니다.",

                appVersion = mMasterDB.Context.MasterVersionApp.FirstOrDefault().Value,
                dataVersion = mMasterDB.Context.MasterVersionData.FirstOrDefault().Value,
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
