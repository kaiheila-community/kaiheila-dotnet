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
            IOptions<IObserver<JToken>> observer)
        {
            _next = next;
            _observer = observer;
        }

        private readonly RequestDelegate _next;

        private readonly IOptions<IObserver<JToken>> _observer;

        public async Task InvokeAsync(HttpContext context)
        {
            if (_observer.Value is null) return;

            JObject payload = context.Items[WebHookClientExtensions.PayloadKey] as JObject;

            if (payload["s"] is not null && payload["s"].ToObject<int>() == 0)
            {
                context.Response.StatusCode = 200;
                _observer.Value.OnNext(payload["d"]);
                return;
            }

            await _next(context);
        }
    }

    public static class WebHookClientEmitterExtensions
    {
        internal static IApplicationBuilder UseWebHookClientEmitter(
            this IApplicationBuilder builder,
            IObserver<JToken> observer)
        {
            builder.UseMiddleware<WebHookClientEmitterMiddleware>(
                Options.Create(observer));
            return builder;
        }
    }
}
