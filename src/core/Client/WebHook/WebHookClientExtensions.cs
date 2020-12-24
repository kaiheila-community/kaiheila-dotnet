using System;
using System.ComponentModel.DataAnnotations;
using Kaiheila.Client.Rest;
using Newtonsoft.Json;

namespace Kaiheila.Client.WebHook
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class WebHookClientOptions : RestClientOptions
    {
        [Required]
        [JsonProperty("verify_token")]
        public string VerifyToken { get; set; } = "";

        [JsonProperty("encrypt_key")]
        public string EncryptKey { get; set; } = "";

        [Required]
        [JsonProperty("port")]
        public int Port { get; set; }
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

        public static WebHookClientOptions UseVerifyToken(
            this WebHookClientOptions options,
            string verifyToken)
        {
            options.VerifyToken = verifyToken;
            return options;
        }

        public static WebHookClientOptions UseEncryptKey(
            this WebHookClientOptions options,
            string encryptKey)
        {
            options.EncryptKey = encryptKey;
            return options;
        }

        public static WebHookClientOptions Listen(
            this WebHookClientOptions options,
            int port)
        {
            options.Port = port;
            return options;
        }

        #endregion

        #region Middleware Const Keys

        internal const string PayloadKey = "payload";

        #endregion
    }
}
