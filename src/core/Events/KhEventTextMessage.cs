using System.Threading.Tasks;
using Kaiheila.Client;
using Kaiheila.Data;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Events
{
    [KhEvent("GROUP|1")]
    public class KhEventTextMessage : KhEventBase, IKhEventUser
    {
        public override async Task Parse(JToken payload)
        {
            // ReSharper disable PossibleNullReferenceException

            User.Id = payload["extra"]["author"]["id"].ToObject<long>();
            User.Username = payload["extra"]["author"]["username"].ToObject<string>();
            User.Nickname = payload["extra"]["author"]["nickname"].ToObject<string>();

            // ReSharper restore PossibleNullReferenceException
        }

        public override async Task Send(Bot bot)
        {
            var (msgId, msgTimestamp) = await bot.SendTextMessage(
                1,
                TargetId,
                Content,
                string.IsNullOrEmpty(Quote) ? null : Quote);

            Id = msgId;
            Timestamp = msgTimestamp;
        }

        public KhUser User { get; set; } = new KhUser();

        public string Quote { get; set; } = "";
    }
}
