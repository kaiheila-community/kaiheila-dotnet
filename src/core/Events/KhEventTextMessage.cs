using System.Threading.Tasks;
using Kaiheila.Client;

namespace Kaiheila.Events
{
    public class KhEventTextMessage : KhEventMessage
    {
        public override async Task Send(Bot bot) =>
            await bot.SendTextMessage(
                1,
                TargetId,
                Content);
    }
}
