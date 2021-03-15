using System;
using Kaiheila.Client.Rest;
using Kaiheila.Events;

namespace Kaiheila.Client.Event
{
    public abstract class EventClient : RestClient
    {
        internal EventClient(RestClientOptions options) : base(options)
        {
        }

        #region Event

        /// <summary>
        /// 机器人事件的 Observable。
        /// </summary>
        public abstract IObservable<KhEventBase> Event { get; }

        #endregion
    }
}
