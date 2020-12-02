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
    public interface IBot : IDisposable
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; set; }

        #endregion

        #region User

        public KhUser Self { get; }

        #endregion

        #region Message

        public Task SendTextMessage(long channel, string message);

        #endregion

        #region Friend

        public Task<KhUser> GetUserState(long user = 0);

        public Task<List<KhUser>> GetFriends(KhFriendsType type);

        #endregion

        #region Channel

        public Task<KhChannel> GetChannelState(long channelId);

        #endregion
    }
}
