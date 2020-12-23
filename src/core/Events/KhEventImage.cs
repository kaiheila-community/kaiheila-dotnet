using System;
using System.Threading.Tasks;
using Kaiheila.Client;

namespace Kaiheila.Events
{
    [Obsolete]
    public class KhEventImage : KhEventAsset
    {
        public KhEventImage(string path, string name) : base(path, name)
        {
            if (Type != "assets")
                throw new InvalidOperationException($"类型{Type}不是\"assets\"。");
        }

        public override async Task Send(Bot bot) =>
            await bot.SendTextMessage(
                1,
                ChannelId,
                $"（图片： {AssetsPrefix + Path} ）");
    }
}
