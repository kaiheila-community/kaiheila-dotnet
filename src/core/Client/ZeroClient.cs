using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kaiheila.Data;
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

        public override Task SendImageMessage(long channel, string imageUrl, string imageName)
        {
            throw new System.NotImplementedException();
        }

        public override Task<KhEventImage> UploadImage(string name, long channel, string file)
        {
            throw new System.NotImplementedException();
        }

        public override Task<KhEventFile> UploadFile(string name, long channel, string file)
        {
            throw new System.NotImplementedException();
        }

        public override Task<KhUser> GetUserState(long user = 0)
        {
            throw new System.NotImplementedException();
        }

        public override Task<List<KhUser>> GetFriends(KhFriendsType type)
        {
            throw new System.NotImplementedException();
        }

        public override Task<KhChannel> GetChannelState(long channelId)
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
        }
    }
}
