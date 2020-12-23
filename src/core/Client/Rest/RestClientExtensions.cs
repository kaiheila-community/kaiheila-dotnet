using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Kaiheila.Client.Rest
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class RestClientOptions : BotOptions
    {
        [Required]
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; } = @"https://www.kaiheila.cn/api/v";

        [Required]
        [JsonProperty("api_version")]
        // ReSharper disable once InconsistentNaming
        public int APIVersion { get; set; }
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

        // ReSharper disable once InconsistentNaming
        public static RestClientOptions UseLatestAPI(
            this RestClientOptions options,
            int apiVersion = 0) =>
            options.UseAPIVersion();

        // ReSharper disable once InconsistentNaming
        public static RestClientOptions UseAPIVersion(
            this RestClientOptions options,
            int apiVersion = 0)
        {
            options.APIVersion = apiVersion;
            return options;
        }

        public static RestClient Build(
            this RestClientBuilder builder) =>
            new RestClient(builder.Options);

        #endregion
    }
}
