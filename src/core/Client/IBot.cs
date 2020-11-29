﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kaiheila.Data;
using Kaiheila.Events;

namespace Kaiheila.Client
{
    /// <summary>
    /// Kaiheila机器人。
    /// </summary>
    public interface IBot : IDisposable
    {
        #region Event

        /// <summary>
        /// 机器人事件Observable。
        /// </summary>
        public IObservable<KhEventBase> Event { get; set; }

        #endregion

        #region Message



        #endregion

        #region Friend

        public Task<KhUser> GetUserState();

        public Task<List<KhUser>> GetFriends(KhFriendsType type);

        #endregion
    }
}
