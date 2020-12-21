using System.Reactive.Linq;
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

        public override Task SendTextMessage(long channel, string message)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
        }
    }
}
