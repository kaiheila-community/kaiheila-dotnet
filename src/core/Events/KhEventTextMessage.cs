using System.Threading.Tasks;
using Kaiheila.Client;
using Kaiheila.Data;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Events
{
    public class KhEventTextMessage : KhEventBase, IKhEventUser
    {
        public override async Task Parse(JToken payload)
        {
            User.Id = payload["author"]["id"].ToObject<long>();
            User.Username = payload["author"]["username"].ToObject<string>();
            User.Nickname = payload["author"]["nickname"].ToObject<string>();
        }

        public override async Task Send(Bot bot)
        {
            var (msgId, msgTimestamp) = await bot.SendTextMessage(
                1,
                TargetId,
                Content);

            Id = msgId;
            Timestamp = msgTimestamp;
        }

        public KhUser User { get; set; } = new KhUser();
    }
}
