using System.Threading.Tasks;

namespace Kaiheila.Client
{
    public class ZeroClient : Bot
    {
        public override void Start() => throw new System.NotImplementedException();

        protected internal override Task<Bot> SendTextMessage(long channel, string message) =>
            throw new System.NotImplementedException();

        public override void Dispose()
        {
        }
    }
}
