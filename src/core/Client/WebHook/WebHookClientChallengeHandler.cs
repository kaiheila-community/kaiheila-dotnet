using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    class WebHookClientChallengeHandlerMiddleware
    {
        public WebHookClientChallengeHandlerMiddleware(
            RequestDelegate next,
            IOptions<WebHookClientOptions> options)
        {
            _next = next;
            _options = options;
        }

        private readonly RequestDelegate _next;

        private readonly IOptions<WebHookClientOptions> _options;

        public async Task InvokeAsync(HttpContext context)
        {
            JObject payload = context.Items[WebHookClientExtensions.PayloadKey] as JObject;

            if (payload is null)
                throw new WebHookClientException(
                    "Payload (string) is null.",
                    nameof(WebHookClientDecryptorMiddleware));

            // Skip other events
            if (payload["s"] is null ||
                payload["s"].ToObject<int>() != 0 ||
                payload["d"]?["type"] is null ||
                payload["d"]["type"].ToObject<int>() != 255 ||
                payload["d"]["channel_type"] is null ||
                payload["d"]["channel_type"].ToObject<string>() != "WEBHOOK_CHALLENGE")
            {
                await _next(context);
                return;
            }

            // Eject wrong requests
            if (payload["d"]["challenge"] is null ||
                payload["d"]["verify_token"] is null ||
                payload["d"]["verify_token"].ToObject<string>() != _options.Value.VerifyToken)
            {
                context.Response.StatusCode = 403;
                return;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JObject.FromObject(new
            {
                challenge = payload["d"]["challenge"].ToObject<string>()
            }).ToString());
        }
    }

    public static class WebHookClientChallengeHandlerExtensions
    {
        internal static IApplicationBuilder UseWebHookClientChallengeHandler(
            this IApplicationBuilder builder,
            WebHookClientOptions options)
        {
            builder.UseMiddleware<WebHookClientChallengeHandlerMiddleware>(
                Options.Create(options));
            return builder;
        }
    }
}
