using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientShorterMiddleware
    {
        public WebHookClientShorterMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;

            System.Diagnostics.Debug.WriteLine(
                (context.Items[WebHookClientExtensions.PayloadKey] as JObject)?.ToString());
        }
    }

    public static class WebHookClientShorterExtensions
    {
        internal static IApplicationBuilder UseWebHookClientShorter(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<WebHookClientEmitterMiddleware>();
            return builder;
        }
    }
}
