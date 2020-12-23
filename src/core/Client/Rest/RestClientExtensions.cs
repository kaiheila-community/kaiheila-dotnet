using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Kaiheila.Client.Rest
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class RestClientOptions : BotOptions
    {
        #region Connection

        [Required]
        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; } = @"https://www.kaiheila.cn/api/v";

        #endregion
    }

    public class RestClientBuilder
    {
        internal readonly RestClientOptions Options = new RestClientOptions();

        public RestClientBuilder Configure(
            Action<RestClientOptions> configureClient)
        {
            configureClient(Options);
            return this;
        }
    }

    public static class RestClientExtensions
    {
        #region Options Builder

        public static RestClientOptions UseBaseUrl(
            this RestClientOptions options,
            string baseUrl)
        {
            options.BaseUrl = baseUrl;
            return options;
        }

        public static RestClient Build(
            this RestClientBuilder builder) =>
            new RestClient(builder.Options);

        #endregion
    }
}
