using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kaiheila.Data;
using Kaiheila.Events;

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

        public abstract Task SendTextMessage(long channel, string message);

        public abstract Task SendImageMessage(long channel, string imageUrl, string imageName);

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
