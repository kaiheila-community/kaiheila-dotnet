using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Kaiheila.Client.Rest;
using Kaiheila.Events;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClient : RestClient
    {
        #region Constructor

        internal WebHookClient(WebHookClientOptions options) : base(options)
        {
            Options = options;

            _webHost = CreateWebHostBuilder().Build();

            Event = Observable.Create<JObject>(observer =>
                {
                    EventObserver = observer;
                    return Disposable.Empty;
                }).SubscribeOn(Scheduler.Default)
                .Select(ParseEvent)
                .SubscribeOn(Scheduler.Default);
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

        #region Event

        private new IObserver<JObject> EventObserver;

        private KhEventBase ParseEvent(JObject arg)
        {
            System.Diagnostics.Debug.WriteLine(arg.ToString());
            return new KhEventBase();
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
