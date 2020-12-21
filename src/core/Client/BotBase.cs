using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kaiheila.Events;
using Kaiheila.Events.Combiners;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kaiheila.Client
{
    /// <summary>
    /// Kaiheila机器人。
    /// </summary>
    public abstract class BotBase : IDisposable
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; protected set; }

        protected IObserver<KhEventBase> EventObserver;

        #endregion

        #region Message

        protected internal abstract Task<BotBase> SendTextMessage(
            long channel,
            string message);

        #endregion

        #region Dispose

        public abstract void Dispose();

        #endregion
    }

    public static class BotExtensions
    {
        #region Message

        public static async Task<BotBase> SendEvent(
            this BotBase bot,
            KhEventBase khEvent)
        {
            await khEvent.Send(bot);
            return bot;
        }

        public static async Task<BotBase> SendEvent(
            this Task<BotBase> task,
            KhEventBase khEvent)
        {
            BotBase bot = await task;
            await bot.SendEvent(khEvent);
            return bot;
        }

        public static async Task<BotBase> SendEvents(
            this BotBase bot,
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

            foreach (KhEventBase khEvent in khEvents) await bot.SendEvent(khEvent);

            return bot;
        }

        public static async Task<BotBase> SendEvents(
            this Task<BotBase> task,
            IList<KhEventBase> khEvents,
            KhEventCombinerHost combinerHost = null)
        {
            BotBase bot = await task;
            await bot.SendEvents(khEvents, combinerHost);
            return bot;
        }

        #endregion
    }
}
