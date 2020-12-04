using System.Threading.Tasks;
using Kaiheila.Client;

namespace Kaiheila.Events
{
    public class KhEventTextMessage : KhEventBase
    {
        public string Content;

        public override async Task Send(BotBase bot) => await bot.SendTextMessage(ChannelId, Content);
    }
}
