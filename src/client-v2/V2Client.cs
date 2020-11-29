using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
    public sealed class V2Client : IBot
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
            
            _websocketClient = new WebsocketClient(uri)
            {
                ReconnectTimeout = TimeSpan.FromSeconds(5)
            };

            _websocketClient.MessageReceived.Subscribe(SocketOnMessage);
            _websocketClient.Start();
        }

        #endregion

        #region Config

        private readonly string _auth;

        private const string ApiPrefix = @"ttps://www.kaiheila.cn/api/v2";

        private static string GetUri(string endpoint) => ApiPrefix + endpoint;

        #endregion

        #region Lifecycle

        public void Dispose()
        {
            _websocketClient?.Dispose();
        }

        #endregion

        #region Core

        private readonly WebsocketClient _websocketClient;

        private void SocketOnMessage(ResponseMessage obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; set; }

        #endregion

        #region Message



        #endregion

        #region Friend

        public async Task<KhUser> GetUserState()
        {
            HttpWebRequest request = RequestHelper.CreateWebRequest(GetUri("/user/user-state"));
            
            JObject response =
                JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                    .ReadToEndAsync());

            return new KhUser
            {
                Id = int.Parse(response["user"]?["id"]?.ToObject<string>()!),
                Username = response["user"]?["username"]?.ToObject<string>()
            };
        }

        public async Task<List<KhUser>> GetFriends(KhFriendsType type)
        {
            HttpWebRequest request = RequestHelper.CreateWebRequest(GetUri($"/friends?type={type.GetTypeString()}"));

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
                    Id = int.Parse(token["friend_info"]?["id"]?.ToObject<string>()!),
                    Username = token["friend_info"]?["username"]?.ToObject<string>()
                });
            }

            return users;
        }

        #endregion
    }
}
