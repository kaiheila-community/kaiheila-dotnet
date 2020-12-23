using System;
using System.Threading.Tasks;
using Kaiheila.Client;

namespace Kaiheila.Events
{
    public class KhEventBase
    {
        #region Send

        public virtual async Task Send(Bot bot)
        {
            // Abstract
            throw new NotImplementedException($"{GetType().Name}中无法调用Send()。");
        }

        #endregion
    }
}
