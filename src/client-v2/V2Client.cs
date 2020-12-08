using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Kaiheila.Assets;
using Kaiheila.Data;
using Kaiheila.Events;
using Kaiheila.Net;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace Kaiheila.Client.V2
{
    /// <summary>
    /// Kaiheila V2机器人。
    /// </summary>
    public sealed class V2Client : BotBase
    {
        #region Constructor

        /// <summary>
        /// 初始化Kaiheila V2机器人。
        /// </summary>
        /// <param name="auth">Kaiheila鉴权使用的Cookie中的auth字段。</param>
        /// <param name="uri">WebSocket连接地址。</param>
        public V2Client(
            string auth,
            Uri uri)
        {
            _auth = auth;

            Event = Observable.Create<KhEventBase>(observer =>
            {
                EventObserver = observer;
                return Disposable.Empty;
            });

            _websocketClient = new WebsocketClient(uri)
            {
                ReconnectTimeout = TimeSpan.FromSeconds(5)
            };
            _websocketClient.ReconnectTimeout = TimeSpan.MaxValue;

            _websocketClient.MessageReceived
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(SocketOnMessage);

            _websocketClient.Start();

            Task<KhUser> getUserStateAsync = GetUserState();
            getUserStateAsync.Wait();
            Self = getUserStateAsync.Result;
        }

        #endregion

        #region Config

        private readonly string _auth;

        private const string ApiPrefix = @"https://www.kaiheila.cn/api/v2";

        private static string GetUri(string endpoint) => ApiPrefix + endpoint;

        private HttpWebRequest CreateWebRequest(string uri, bool post = false)
        {
            var request = RequestHelper.CreateWebRequest(uri, post);
            request.Headers["Cookie"] = "auth=" + _auth;
            return request;
        }

        #endregion

        #region Lifecycle

        public override void Dispose()
        {
            _websocketClient?.Dispose();
        }

        #endregion

        #region Core

        private readonly WebsocketClient _websocketClient;

        #endregion

        #region Message

        private void SocketOnMessage(ResponseMessage obj)
        {
            JObject payload = JObject.Parse(obj.Text);
            if (payload["cmd"]?.ToObject<string>() != "receiveMessage") return;

            JObject extra;

            try
            {
                extra = JObject.Parse(payload["args"]?[0]?["content"]?["extra"]?.ToObject<string>()!);
            }
            catch (Exception)
            {
                return;
            }

            EventObserver?.OnNext(new KhEventTextMessage
            {
                Content = payload["args"]?[0]?["content"]?["content"]?.ToObject<string>(),
                ChannelId = long.Parse(payload["args"]?[0]?["targetId"]?.ToObject<string>()!),
                ChannelName = extra["channel_name"]?.ToObject<string>(),
                Guild = long.Parse(extra["guild_id"]?.ToObject<string>()!),
                Raw = obj.Text,
                User = new KhUser
                {
                    Id = long.Parse(extra["author"]?["id"]?.ToObject<string>()!),
                    Username = extra["author"]?["username"]?.ToObject<string>()
                }
            });
        }

        public override async Task SendTextMessage(long channel, string message)
        {
            _websocketClient.Send(JObject.FromObject(new
            {
                cmd = "sendGroupMessage",
                channelId = channel.ToString(),
                content = message
            }).ToString());
        }

        public override async Task SendImageMessage(long channel, string imageUrl, string imageName)
        {
            _websocketClient.Send(JObject.FromObject(new
            {
                cmd = "sendGroupImage",
                channelId = channel.ToString(),
                content = imageUrl,
                image_name = imageName
            }).ToString());
        }

        #endregion

        #region Upload

        private const string Boundary = "----WebKitFormBoundaryheEi6UpgP9In8jez";

        public override async Task<KhEventImage> UploadImage(
            string name,
            long channel,
            string file)
        {
            // Construct Request
            HttpWebRequest request = CreateWebRequest(GetUri("/assets"), true);
            request.ContentType = "multipart/form-data; boundary=" + Boundary;

            // Initialize Content
            using MultipartFormDataContent content = new MultipartFormDataContent(Boundary)
            {
                {new StreamContent(await AssetsHelper.GetAssetFile(file)), "\"image\"", $"\"{name}\""},
                {new StringContent("image"), "\"type\""},
                {new StringContent(channel.ToString()), "\"channel_id\""}
            };

            await content.CopyToAsync(request.GetRequestStream());

            // Get Response
            JObject response =
                JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                    .ReadToEndAsync());

            return new KhEventImage(
                response["url"]?.ToObject<string>()?.Replace(KhEventAsset.AssetsPrefix, ""),
                name)
            {
                ChannelId = channel,
                User = Self
            };
        }

        public override async Task<KhEventFile> UploadFile(
            string name,
            long channel,
            string file)
        {
            // Construct Request
            HttpWebRequest request = CreateWebRequest(GetUri("/assets/file"), true);
            request.ContentType = "multipart/form-data; boundary=" + Boundary;

            // Initialize Content
            using MultipartFormDataContent content = new MultipartFormDataContent(Boundary)
            {
                {new StreamContent(await AssetsHelper.GetAssetFile(file)), "\"file\"", $"\"{name}\""},
                {new StringContent(channel.ToString()), "\"channel_id\""},
                {new StringContent(name), "\"filename\""}
            };

            await content.CopyToAsync(request.GetRequestStream());

            // Get Response
            JObject response =
                JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                    .ReadToEndAsync());

            return new KhEventFile(
                response["url"]?.ToObject<string>(),
                name,
                response["extension"]?.ToObject<string>())
            {
                ChannelId = channel,
                User = Self
            };
        }

        #endregion

        #region Friend

        public override async Task<KhUser> GetUserState(long user = 0)
        {
            if (user == 0)
            {
                HttpWebRequest request = CreateWebRequest(GetUri("/user/user-state"));

                JObject response =
                    JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                        .ReadToEndAsync());

                return new KhUser
                {
                    Id = long.Parse(response["user"]?["id"]?.ToObject<string>()!),
                    Username = response["user"]?["username"]?.ToObject<string>()
                };
            }
            else
            {
                HttpWebRequest request = CreateWebRequest(GetUri("/users/" + user));

                JObject response =
                    JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                        .ReadToEndAsync());

                return new KhUser
                {
                    Id = long.Parse(response["id"]?.ToObject<string>()!),
                    Username = response["username"]?.ToObject<string>()
                };
            }
        }

        public override async Task<List<KhUser>> GetFriends(KhFriendsType type)
        {
            HttpWebRequest request = CreateWebRequest(GetUri($"/friends?type={type.GetTypeString()}"));

            string raw = await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                .ReadToEndAsync();

            if (string.IsNullOrEmpty(raw))
                return new List<KhUser>();

            JArray response = JArray.Parse(raw);

            List<KhUser> users = new List<KhUser>();

            foreach (JToken token in response)
            {
                users.Add(new KhUser
                {
                    Id = long.Parse(token["friend_info"]?["id"]?.ToObject<string>()!),
                    Username = token["friend_info"]?["username"]?.ToObject<string>()
                });
            }

            return users;
        }

        #endregion

        #region Channel

        public override async Task<KhChannel> GetChannelState(long channelId)
        {
            HttpWebRequest request = CreateWebRequest(GetUri("/channels/" + channelId));

            JObject response =
                JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                    .ReadToEndAsync());

            return new KhChannel
            {
                ChannelId = long.Parse(response["id"]?.ToObject<string>()!),
                ChannelName = response["name"]?.ToObject<string>(),
                Guild = long.Parse(response["guild_id"]?.ToObject<string>()!)
            };
        }

        #endregion
    }
}
