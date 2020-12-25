using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientEmitterMiddleware
    {
        public WebHookClientEmitterMiddleware(
            RequestDelegate next,
            IOptions<Func<IObserver<JToken>>> getObserver)
        {
            _next = next;
            _getObserver = getObserver;
        }

        private readonly RequestDelegate _next;

        private readonly IOptions<Func<IObserver<JToken>>> _getObserver;

        public async Task InvokeAsync(HttpContext context)
        {
            if (_getObserver.Value() is null) return;

            JObject payload = context.Items[WebHookClientExtensions.PayloadKey] as JObject;

            if (payload["s"] is not null && payload["s"].ToObject<int>() == 0)
            {
                context.Response.StatusCode = 200;
                _getObserver.Value().OnNext(payload["d"]);
                return;
            }

            await _next(context);
        }
    }

    public static class WebHookClientEmitterExtensions
    {
        internal static IApplicationBuilder UseWebHookClientEmitter(
            this IApplicationBuilder builder,
            Func<IObserver<JToken>> getObserver)
        {
            builder.UseMiddleware<WebHookClientEmitterMiddleware>(
                Options.Create(getObserver));
            return builder;
        }
    }
}
