using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Kaiheila.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientDecompressorMiddleware
    {
        public WebHookClientDecompressorMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        private const string CompressKey = "compress";

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Query.ContainsKey(CompressKey) &&
                context.Request.Query[CompressKey] == "0")
            {
                context.Items[WebHookClientExtensions.PayloadKey] = await context.ReadBodyAsStringAsync();

                await _next(context);
                return;
            }

            // Decompress Deflate

            DeflateStream deflateStream = new DeflateStream(context.Request.Body, CompressionMode.Decompress);

            context.Items[WebHookClientExtensions.PayloadKey] =
                await new StreamReader(deflateStream).ReadToEndAsync();

            await deflateStream.DisposeAsync();

            await _next(context);
            return;
        }
    }

    public static class WebHookClientDecompressorExtensions
    {
        internal static IApplicationBuilder UseWebHookClientDecompressor(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<WebHookClientDecompressorMiddleware>();
            return builder;
        }
    }
}
