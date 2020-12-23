using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kaiheila.Events;
using Kaiheila.Events.Combiners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Kaiheila.Client
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class BotOptions
    {
        #region Authorization

        [Required]
        [JsonProperty("authorizationType")]
        public AuthorizationType AuthorizationType { get; set; } = AuthorizationType.Bot;

        [Required]
        [JsonProperty("authorizationHeader")]
        public string AuthorizationHeader { get; set; } = "";

        #endregion
    }

    public enum AuthorizationType
    {
        Bot = 0,
        Oauth2 = 1
    }

    public static class BotExtensions
    {
        #region Message

        public static async Task<Bot> Send(
            this Bot bot,
            KhEventBase khEvent)
        {
            await khEvent.Send(bot);
            return bot;
        }

        public static async Task<Bot> Send(
            this Task<Bot> task,
            KhEventBase khEvent)
        {
            Bot bot = await task;
            await bot.Send(khEvent);
            return bot;
        }

        public static async Task<Bot> Send(
            this Bot bot,
            IList<KhEventBase> khEvents,
            KhEventCombinerHost combinerHost = null)
        {
            combinerHost ??= new KhEventCombinerHost(new Logger<KhEventCombinerHost>(new NullLoggerFactory()));

            // Copy
            khEvents = new List<KhEventBase>(khEvents);

            // Combine
            while (true)
            {
                bool flag = false;

                for (int i = 0; i < khEvents.Count - 1; i++)
                {
                    if (khEvents[0].GetType() != khEvents[1].GetType()) continue;

                    KhEventBase newEvent = combinerHost.Combine(khEvents[0], khEvents[1]);
                    khEvents.RemoveAt(0);
                    khEvents.RemoveAt(0);
                    khEvents.Insert(0, newEvent);

                    flag = true;
                    break;
                }

                if (!flag) break;
            }

            foreach (KhEventBase khEvent in khEvents) await bot.Send(khEvent);

            return bot;
        }

        public static async Task<Bot> Send(
            this Task<Bot> task,
            IList<KhEventBase> khEvents,
            KhEventCombinerHost combinerHost = null)
        {
            Bot bot = await task;
            await bot.Send(khEvents, combinerHost);
            return bot;
        }

        #endregion

        #region Options Builder

        public static BotOptions UseBotAuthorization(
            this BotOptions options,
            string token)
        {
            options.AuthorizationType = AuthorizationType.Bot;
            options.AuthorizationHeader = $"Bot {token}";
            return options;
        }

        public static BotOptions UseOauth2Authorization(
            this BotOptions options,
            string token)
        {
            options.AuthorizationType = AuthorizationType.Oauth2;
            options.AuthorizationHeader = $"Bearer {token}";
            return options;
        }

        #endregion
    }
}
