using System;
using Kaiheila.Data;
using Kaiheila.Events;

namespace Kaiheila.Client.V2
{
    /// <summary>
    /// Kaiheila V2机器人。
    /// </summary>
    public sealed class V2Client : IBot
    {
        #region Constructor

        public V2Client(
            string auth,
            string host,
            string port)
        {
            _auth = auth;
            _host = host;
            _port = port;
        }

        #endregion

        #region Config

        private readonly string _auth;
        private readonly string _host;
        private readonly string _port;

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

        public KhUser GetUserState()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
