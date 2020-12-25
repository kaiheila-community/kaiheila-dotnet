using System;
using System.Composition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Events
{
    /// <summary>
    /// 开黑啦事件解析主机。
    /// </summary>
    [Export]
    public class EventParserHost
    {
        public EventParserHost(
            ILogger<EventParserHost> logger)
        {
            _logger = logger ?? new NullLogger<EventParserHost>();
        }

        public KhEventBase Parse(JToken payload)
        {
            throw new NotImplementedException();
        }

        private ILogger<EventParserHost> _logger;
    }

    public interface IEventParserClientOptions
    {
        public EventParserHost EventParser { get; set; }
    }

    public static class EventParserClientExtensions
    {
        public static T UseEventParserHost<T>(
            this T options,
            EventParserHost host) where T : IEventParserClientOptions
        {
            options.EventParser = host;
            return options;
        }
    }

    [Serializable]
    public class EventParseException : Exception
    {
        public EventParseException(string message, JToken eventPayload = null, Exception inner = null) : base(message, inner)
        {
            EventPayload = eventPayload;
        }

        public JToken EventPayload;
    }
}
