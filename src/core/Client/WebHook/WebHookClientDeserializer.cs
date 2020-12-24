using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientDeserializerMiddleware
    {
        public WebHookClientDeserializerMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Items[WebHookClientExtensions.PayloadKey] =
                JObject.Parse((context.Items[WebHookClientExtensions.PayloadKey] as string)!);

            await _next(context);
        }
    }

    public static class WebHookClientDeserializerExtensions
    {
        internal static IApplicationBuilder UseWebHookClientDeserializer(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<WebHookClientDeserializerMiddleware>();
            return builder;
        }
    }
}
