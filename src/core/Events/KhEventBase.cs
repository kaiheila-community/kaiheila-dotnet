using Kaiheila.Data;

namespace Kaiheila.Events
{
    public class KhEventBase
    {
        public string Raw;

        public long Guild;

        public long ChannelId;

        public string ChannelName;

        public KhUser User;
    }
}
