using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kaiheila.Events;

namespace Kaiheila.Client
{
    /// <summary>
    /// Kaiheila机器人。
    /// </summary>
    public abstract class Bot : IDisposable
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; protected set; }

        protected IObserver<KhEventBase> EventObserver;

        #endregion

        #region Constructor

        protected Bot()
        {
            Event = Observable.Create<KhEventBase>(observer =>
                {
                    EventObserver = observer;
                    return Disposable.Empty;
                })
                .SubscribeOn(Scheduler.Default);
        }

        #endregion

        #region Lifecycle

        public abstract void Start();

        #endregion

        #region Message

        protected internal abstract Task<(string, long)> SendTextMessage(
            int type,
            long channel,
            string content,
            string quote = null);

        #endregion

        #region Dispose

        public abstract void Dispose();

        #endregion
    }
}
