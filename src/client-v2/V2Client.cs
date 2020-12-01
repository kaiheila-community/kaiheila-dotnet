﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

            Event = Observable.Create<KhEventBase>(observer =>
            {
                _eventObserver = observer;
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
        }

        #endregion

        #region Config

        private readonly string _auth;

        private const string ApiPrefix = @"https://www.kaiheila.cn/api/v2";

        private static string GetUri(string endpoint) => ApiPrefix + endpoint;

        private HttpWebRequest CreateWebRequest(string uri)
        {
            var request = RequestHelper.CreateWebRequest(uri);
            request.Headers["Cookie"] = "auth=" + _auth;
            return request;
        }

        #endregion

        #region Lifecycle

        public void Dispose()
        {
            _websocketClient?.Dispose();
        }

        #endregion

        #region Core

        private readonly WebsocketClient _websocketClient;

        #endregion

        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; set; }

        private IObserver<KhEventBase> _eventObserver;

        #endregion

        #region Message

        private void SocketOnMessage(ResponseMessage obj)
        {
            JObject payload = JObject.Parse(obj.Text);
            if (payload["cmd"]?.ToObject<string>() != "receiveMessage") return;

            JObject extra = JObject.Parse(payload["args"]?[0]?["content"]?["extra"]?.ToObject<string>()!);

            _eventObserver?.OnNext(new KhEventMessage
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

        public async Task SendMessage(long channel, string message)
        {
            _websocketClient.Send(JObject.FromObject(new
            {
                cmd = "sendMessage",
                type = "1",
                channelId = channel.ToString(),
                content = message
            }).ToString());
        }

        #endregion

        #region Friend

        public async Task<KhUser> GetUserState()
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

        public async Task<List<KhUser>> GetFriends(KhFriendsType type)
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
    }
}
