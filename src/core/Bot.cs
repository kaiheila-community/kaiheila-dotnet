using System;
using Kaiheila.Events;

namespace Kaiheila
{
    /// <summary>
    /// Kaiheila机器人。
    /// </summary>
    public class Bot
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event;

        #endregion
    }
}
