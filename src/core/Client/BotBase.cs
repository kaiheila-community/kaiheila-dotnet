using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kaiheila.Data;
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

        #region User

        public KhUser Self { get; protected set; }

        #endregion

        #region Message

        public abstract Task SendTextMessage(
            long channel,
            string message);

        public abstract Task SendImageMessage(
            long channel,
            string imageUrl,
            string imageName);

        public async Task SendEvent(
            KhEventBase khEvent,
            KhEventBase target)
        {
            if (target is not null)
            {
                khEvent.ChannelId = target.ChannelId;
                khEvent.ChannelName = target.ChannelName;
                khEvent.Guild = target.Guild;
            }

            await khEvent.Send(this);
        }

        public async Task SendEvents(
            IList<KhEventBase> khEvents,
            KhEventBase target,
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

            foreach (KhEventBase khEvent in khEvents) await SendEvent(khEvent, target);
        }

        #endregion

        #region Upload

        public abstract Task<KhEventImage> UploadImage(
            string name,
            long channel,
            string file);

        public abstract Task<KhEventFile> UploadFile(
            string name,
            long channel,
            string file);

        #endregion

        #region Friend

        public abstract Task<KhUser> GetUserState(long user = 0);

        public abstract Task<List<KhUser>> GetFriends(KhFriendsType type);

        #endregion

        #region Channel

        public abstract Task<KhChannel> GetChannelState(long channelId);

        #endregion

        #region Dispose

        public abstract void Dispose();

        #endregion
    }
}
