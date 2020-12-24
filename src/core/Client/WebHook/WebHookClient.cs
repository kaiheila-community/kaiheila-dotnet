using Kaiheila.Client.Rest;
using Microsoft.AspNetCore.Hosting;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClient : RestClient
    {
        #region Constructor

        internal WebHookClient(WebHookClientOptions options) : base(options)
        {
            Options = options;

            _webHost = CreateWebHostBuilder().Build();
        }

        public static WebHookClientBuilder CreateWebHookClient() => new WebHookClientBuilder();

        #endregion

        #region Lifecycle

        public override void Start()
        {
            _webHost.RunAsync();
        }

        public override void Dispose()
        {
            _webHost?.Dispose();
        }

        #endregion

        #region Options

        public new readonly WebHookClientOptions Options;

        #endregion

        #region WebHost

        private readonly IWebHost _webHost;

        private IWebHostBuilder CreateWebHostBuilder() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://0.0.0.0:{Options.Port}")
                .Configure(builder =>
                {
                    builder
                        .UseWebHookClientExceptionHandler()
                        .UseWebHookClientDecompressor()
                        .UseWebHookClientDeserializer()
                        .UseWebHookClientDecryptor(Options)
                        .UseWebHookClientChallengeHandler(Options);
                });

        #endregion
    }
}
