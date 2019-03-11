using System;
using System.Net;
using System.Threading.Tasks;
using IIASA.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IIASA.Api.Global.Middleware
{
    public class HandleErrorMiddleware
    {
        private readonly RequestDelegate _next;

        private ILogger Logger { get; } = GlobalLogging.CreateLogger<HandleErrorMiddleware>();

        public HandleErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception Middleware.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var errorCode = HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new
            {
                error = ex.Message
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorCode;

            return context.Response.WriteAsync(result);
        }
    }
}
