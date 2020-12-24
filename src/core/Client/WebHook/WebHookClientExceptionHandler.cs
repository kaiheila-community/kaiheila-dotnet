using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientExceptionHandlerMiddleware
    {
        public WebHookClientExceptionHandlerMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        private RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";

            IExceptionHandlerPathFeature exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature.Error is not null)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Error:");
                builder.AppendLine(exceptionHandlerPathFeature.Error.GetType().Name);
                builder.AppendLine("Message:");
                builder.AppendLine(exceptionHandlerPathFeature.Error.Message);

                string e = builder.ToString();

                System.Diagnostics.Debug.WriteLine(e);
                await context.Response.WriteAsync(e);
            }
        }
    }

    public static class WebHookClientExceptionHandlerExtensions
    {
        internal static IApplicationBuilder UseWebHookClientExceptionHandler(
            this IApplicationBuilder builder)
        {
            builder.UseExceptionHandler(errorApp =>
            {
                errorApp.UseMiddleware<WebHookClientExceptionHandlerMiddleware>();
            });

            return builder;
        }
    }
}
