using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace IdentityControl.API.Asp
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            var queryString = context.Request.QueryString.Value.Length > 0
                ? $"QueryString: {context.Request.QueryString}{Environment.NewLine}"
                : string.Empty;
            Log.Information($"Incoming {context.Request.Method} Request{Environment.NewLine}" +
                            $"{context.Request.Scheme.ToUpper()} Endpoint: {context.Request.Path}{Environment.NewLine}" +
                            queryString);
            return Task.CompletedTask;
        }

        private async Task LogResponse(HttpContext context)
        {
            await _next(context);
            Log.Information($"Outgoing {context.Request.Method} Response{Environment.NewLine}" +
                            $"{context.Request.Scheme.ToUpper()} Endpoint: {context.Request.Path}{Environment.NewLine}" +
                            $"Status Code: {context.Response.StatusCode}");
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}