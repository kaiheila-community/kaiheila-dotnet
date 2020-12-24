using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    class WebHookClientEmitterMiddleware
    {
        public WebHookClientEmitterMiddleware(
            RequestDelegate next,
            IOptions<IObserver<JObject>> observer)
        {
            _next = next;
            _observer = observer;
        }

        private readonly RequestDelegate _next;

        private readonly IOptions<IObserver<JObject>> _observer;

        public async Task InvokeAsync(HttpContext context)
        {
            _observer.Value.OnNext(context.Items[WebHookClientExtensions.PayloadKey] as JObject);

            context.Response.StatusCode = 200;
        }
    }

    public static class WebHookClientEmitterExtensions
    {
        internal static IApplicationBuilder UseWebHookClientEmitter(
            this IApplicationBuilder builder,
            IObserver<JObject> observer)
        {
            builder.UseMiddleware<WebHookClientEmitterMiddleware>(
                Options.Create(observer));
            return builder;
        }
    }
}
