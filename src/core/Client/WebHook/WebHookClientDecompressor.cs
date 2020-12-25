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
            MemoryStream stream = new MemoryStream();
            await context.Request.Body.CopyToAsync(stream);
            await context.Request.Body.DisposeAsync();

            // Magic headers of zlib:
            // 78 01 - No Compression/low
            // 78 9C - Default Compression
            // 78 DA - Best Compression
            stream.Position = 2;

            DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true);
            MemoryStream resultStream = new MemoryStream();
            await deflateStream.CopyToAsync(resultStream);
            await deflateStream.DisposeAsync();
            await stream.DisposeAsync();

            // Rewind
            resultStream.Position = 0;

            StreamReader reader = new StreamReader(resultStream);
            string result = await reader.ReadToEndAsync();
            await resultStream.DisposeAsync();

            context.Items[WebHookClientExtensions.PayloadKey] = result;
            reader.Dispose();

            await _next(context);
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
