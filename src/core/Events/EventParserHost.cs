using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
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

            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                x.GetTypes()
                    .Where(type => Attribute.GetCustomAttribute(type, typeof(KhEventAttribute)) is not null)))
            {
                string eventType = (Attribute.GetCustomAttribute(type, typeof(KhEventAttribute)) as KhEventAttribute)?.Type;
                if (eventType == null || _eventBases.ContainsKey(eventType) || type.FullName == null) continue;

                _eventBases.Add(eventType, type);
            }

            _logger.LogInformation($"加载了{_eventBases.Count}个Kaiheila事件类型。");
        }

        public async Task<KhEventBase> Parse(JToken payload)
        {
            try
            {
                // ReSharper disable PossibleNullReferenceException

                string eventType = $"{payload["channel_type"].ToObject<string>()}|{payload["type"].ToObject<int>()}";
                if (!_eventBases.ContainsKey(eventType))
                    throw new EventParseException($"Cannot find event parser for type {eventType}.", payload);

                KhEventBase eventBase = (Activator.CreateInstance(_eventBases[eventType]) as KhEventBase);
                await eventBase.Parse(payload);

                eventBase.Id = payload["msg_id"].ToObject<string>();
                eventBase.TargetId = payload["msg_timestamp"].ToObject<long>();
                eventBase.Content = payload["content"].ToObject<string>();
                eventBase.TargetId = payload["target_id"].ToObject<long>();

                return eventBase;

                // ReSharper restore PossibleNullReferenceException
            }
            catch (EventParseException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new EventParseException("Error when parsing event.", payload, e);
            }
        }

        private readonly Dictionary<string, Type> _eventBases = new Dictionary<string, Type>();

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
