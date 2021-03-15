using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kaiheila.Client.Rest;
using Kaiheila.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Kaiheila.Client.WebHook
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class WebHookClientOptions : RestClientOptions, IEventParserClientOptions
    {
        [Required]
        [JsonProperty("verify_token")]
        public string VerifyToken { get; set; } = "";

        [JsonProperty("encrypt_key")]
        public string EncryptKey { get; set; } = "";

        [Required]
        [JsonProperty("port")]
        public int Port { get; set; }

        [NotMapped]
        public EventParserHost EventParser { get; set; } = new EventParserHost(new NullLogger<EventParserHost>());
    }

    public class WebHookClientBuilder
    {
        internal readonly WebHookClientOptions Options = new WebHookClientOptions();

        public WebHookClientBuilder Configure(
            Action<WebHookClientOptions> configureClient)
        {
            configureClient(Options);
            return this;
        }
    }

    public static class WebHookClientExtensions
    {
        #region Options Builder

        public static T UseVerifyToken<T>(
            this T options,
            string verifyToken) where T : WebHookClientOptions
        {
            options.VerifyToken = verifyToken;
            return options;
        }

        public static T UseEncryptKey<T>(
            this T options,
            string encryptKey) where T : WebHookClientOptions
        {
            options.EncryptKey = encryptKey;
            return options;
        }

        public static T Listen<T>(
            this T options,
            int port) where T : WebHookClientOptions
        {
            options.Port = port;
            return options;
        }

        public static WebHookClient Build(
            this WebHookClientBuilder builder) =>
            new WebHookClient(builder.Options);

        #endregion

        #region Middleware Const Keys

        internal const string PayloadKey = "payload";

        #endregion
    }

    [Serializable]
    public class WebHookClientException : Exception
    {
        public WebHookClientException()
        {
        }

        public WebHookClientException(string message, string middlewareName) : base(message)
        {
            MiddlewareName = middlewareName;
        }

        public WebHookClientException(string message, string middlewareName, Exception inner) : base(message, inner)
        {
            MiddlewareName = middlewareName;
        }

        public string MiddlewareName { get; set; }
    }
}
