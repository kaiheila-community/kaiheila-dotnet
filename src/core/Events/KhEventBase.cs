using System;
using System.Threading.Tasks;
using Kaiheila.Client;
using Kaiheila.Data;

namespace Kaiheila.Events
{
    public class KhEventBase
    {
        public string Raw;

        public long Guild;

        public long ChannelId;

        public string ChannelName;

        public KhUser User;

        #region Send

        public virtual async Task Send(Bot bot)
        {
            // Abstract
            throw new NotImplementedException($"{GetType().Name}中无法调用Send()。");
        }

        #endregion
    }
}
