using System.Threading.Tasks;
using Kaiheila.Client;

namespace Kaiheila.Events
{
    public class KhEventTextMessage : KhEventBase
    {
        public override async Task Send(Bot bot)
        {
            var (msgId, msgTimestamp) = await bot.SendTextMessage(
                1,
                TargetId,
                Content);

            Id = msgId;
            Timestamp = msgTimestamp;
        }
    }
}
