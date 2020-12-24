using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClientDecryptorMiddleware
    {
        public WebHookClientDecryptorMiddleware(
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
            
            bool configEnabled = !string.IsNullOrEmpty(_options.Value.EncryptKey);
            bool contextEnabled = payload["encrypt"] is not null;

            if (configEnabled != contextEnabled)
                throw new WebHookClientException(
                    $"Encrypt config error. Config: {configEnabled}, Context: {contextEnabled}",
                    nameof(WebHookClientDecryptorMiddleware));

            if (!contextEnabled)
            {
                await _next(context);
                return;
            }

            // Decrypt

            string eData = Encoding.UTF8.GetString(Convert.FromBase64String(payload["encrypt"].ToObject<string>()!));

            byte[] iv = Encoding.UTF8.GetBytes(eData.Substring(0, 16));
            eData = eData.Substring(16);

            byte[] key = Encoding.UTF8.GetBytes(_options.Value.EncryptKey +
                                                new string('\0', 32 - _options.Value.EncryptKey.Length));

            try
            {
                using var rijndaelManaged =
                    new RijndaelManaged {Key = key, IV = iv, Mode = CipherMode.CBC};
                await using var memoryStream =
                    new MemoryStream(Convert.FromBase64String(eData));
                await using var cryptoStream =
                    new CryptoStream(memoryStream,
                        rijndaelManaged.CreateDecryptor(key, iv),
                        CryptoStreamMode.Read);

                context.Items[WebHookClientExtensions.PayloadKey] =
                    JObject.Parse(await new StreamReader(cryptoStream).ReadToEndAsync());
            }
            catch (CryptographicException e)
            {
                throw new WebHookClientException(
                    "RijndaelManaged decrypt error.",
                    nameof(WebHookClientDecryptorMiddleware),
                    e);
            }
            catch (Exception e)
            {
                throw new WebHookClientException(
                    "Decrypt error.",
                    nameof(WebHookClientDecryptorMiddleware),
                    e);
            }

            await _next(context);
        }
    }

    public static class WebHookClientDecryptorExtensions
    {
        internal static IApplicationBuilder UseWebHookClientDecryptor(
            this IApplicationBuilder builder,
            WebHookClientOptions options)
        {
            builder.UseMiddleware<WebHookClientDecryptorMiddleware>(
                Options.Create(options));
            return builder;
        }
    }
}
