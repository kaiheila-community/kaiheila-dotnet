using System;
using Kaiheila.Data;
using Kaiheila.Events;

namespace Kaiheila.Client
{
    /// <summary>
    /// Kaiheila机器人。
    /// </summary>
    public interface IBot
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; set; }

        #endregion

        #region Message



        #endregion

        #region Friend

        public KhUser GetUserState();

        #endregion
    }
}
