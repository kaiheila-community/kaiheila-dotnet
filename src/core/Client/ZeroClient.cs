using System;
using System.Threading.Tasks;

namespace Kaiheila.Client
{
    [Obsolete]
    public class ZeroClient : Bot
    {
        public override void Start() => throw new System.NotImplementedException();

        protected internal override Task SendTextMessage(
            int type,
            long channel,
            string message,
            string quote = null) =>
            throw new NotImplementedException();

        public override void Dispose()
        {
        }
    }
}
