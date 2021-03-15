using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Kaiheila.Client.Event;
using Kaiheila.Events;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClient : EventClient
    {
        #region Constructor

        internal WebHookClient(WebHookClientOptions options) : base(options)
        {
            Options = options;

            Event = _eventObserver
                .SubscribeOn(TaskPoolScheduler.Default)
                .Select(ParseEvent)
                .Where(x => x is not null);

            _webHost = CreateWebHostBuilder().Build();
        }

        public static WebHookClientBuilder CreateWebHookClient() => new WebHookClientBuilder();

        #endregion

        #region Event

        private readonly Subject<JToken> _eventObserver = new();

        public override IObservable<KhEventBase> Event { get; }

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
        
        private KhEventBase ParseEvent(JToken payload)
        {
            try
            {
                Task<KhEventBase> task = Options.EventParser.Parse(payload);
                task.Wait();
                return task.Result;
            }
            catch (EventParseException e)
            {
                _eventObserver.OnError(e);
                return null;
            }
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
                        .UseWebHookClientChallengeHandler(Options)
                        .UseWebHookClientEmitter(() => _eventObserver)
                        .UseWebHookClientShorter();
                });

        #endregion
    }
}
