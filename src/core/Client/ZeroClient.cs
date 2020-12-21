﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Kaiheila.Events;

namespace Kaiheila.Client
{
    public class ZeroClient : BotBase
    {
        public ZeroClient()
        {
            Event = Observable.Empty<KhEventBase>();
        }

        protected internal override Task<BotBase> SendTextMessage(long channel, string message) =>
            throw new System.NotImplementedException();

        public override void Dispose()
        {
        }
    }
}
