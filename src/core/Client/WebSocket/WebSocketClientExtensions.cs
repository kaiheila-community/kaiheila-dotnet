using System;
using Kaiheila.Client.Rest;
using Kaiheila.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Kaiheila.Client.WebSocket
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class WebSocketClientOptions : RestClientOptions, IEventParserClientOptions
    {
        public EventParserHost EventParser { get; set; } = new(new NullLogger<EventParserHost>());
    }

    public class WebSocketClientBuilder
    {
        internal readonly WebSocketClientOptions Options = new();

        public WebSocketClientBuilder Configure(
            Action<WebSocketClientOptions> configureClient)
        {
            configureClient(Options);
            return this;
        }
    }

    public static class WebSocketClientExtensions
    {
        #region Options Builder

        public static WebSocketClient Build(
            this WebSocketClientBuilder builder) =>
            new(builder.Options);

        #endregion
    }

    [Serializable]
    public class WebSocketClientException : Exception
    {
        public WebSocketClientException()
        {
        }

        public WebSocketClientException(string message) : base(message)
        {
        }

        public WebSocketClientException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
