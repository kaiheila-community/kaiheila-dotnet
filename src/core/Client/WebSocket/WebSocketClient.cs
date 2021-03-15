using System;
using System.IO;
using System.IO.Compression;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Kaiheila.Client.Event;
using Kaiheila.Events;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace Kaiheila.Client.WebSocket
{
    public class WebSocketClient : EventClient
    {
        #region Constructor

        internal WebSocketClient(WebSocketClientOptions options) : base(options)
        {
            Options = options;

            Event = _eventObserver
                .Where(x => x is not null)
                .Select(ParseEvent)
                .Where(x => x is not null);
        }

        public static WebSocketClientBuilder CreateWebSocketClient() => new();

        #endregion

        #region Event

        private readonly Subject<JToken> _eventObserver = new();

        public override IObservable<KhEventBase> Event { get; }

        #endregion

        #region LifeCycle

        public override void Start()
        {
            Task<string> getGatewayTask = GetGateway();
            getGatewayTask.Wait();

            _websocketClient = new WebsocketClient(new Uri(getGatewayTask.Result))
            {
                ReconnectTimeout = TimeSpan.FromMinutes(1),
                ErrorReconnectTimeout = TimeSpan.FromMinutes(3)
            };

            _websocketClient.MessageReceived
                .ObserveOn(TaskPoolScheduler.Default)
                .Where(x => x is not null)
                .Select(ParsePacket)
                .Subscribe(_eventObserver);

            _websocketClient.Start();
        }

        public override void Dispose()
        {
            _websocketClient?.Dispose();
        }

        #endregion

        #region WebSocket Client

        private WebsocketClient _websocketClient;

        private JToken ParsePacket(ResponseMessage responseMessage)
        {
            try
            {
                // Decompress Deflate
                MemoryStream stream = new MemoryStream();
                stream.Write(responseMessage.Binary);

                // Magic headers of zlib:
                // 78 01 - No Compression/low
                // 78 9C - Default Compression
                // 78 DA - Best Compression
                stream.Position = 2;

                DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true);
                MemoryStream resultStream = new MemoryStream();
                deflateStream.CopyTo(resultStream);
                deflateStream.Dispose();
                stream.Dispose();

                // Rewind
                resultStream.Position = 0;

                StreamReader reader = new StreamReader(resultStream);
                string raw = reader.ReadToEnd();
                resultStream.Dispose();

                JObject response = JObject.Parse(raw);

                int s = response["s"].ToObject<int>();
                if (s != 0) return null;

                return response["d"];
            }
            catch (Exception e)
            {
                _eventObserver.OnError(e);
                return null;
            }
        }

        #endregion

        #region Event Parser

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

        public new readonly WebSocketClientOptions Options;

        #endregion
    }
}
