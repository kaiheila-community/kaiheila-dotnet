using Kaiheila.Data;

namespace Kaiheila.Events
{
    public class KhEventMessage : KhEventBase
    {
        public long Guild;

        public long ChannelId;

        public string ChannelName;

        public string Content;

        public KhUser User;
    }
}
