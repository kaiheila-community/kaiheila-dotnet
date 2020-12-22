using System;
using System.Net;
using System.Threading.Tasks;
using Kaiheila.Net;

namespace Kaiheila.Client.Rest
{
    public class RestClient : BotBase
    {
        #region Consts

        internal const string BaseUrl = @"https://www.kaiheila.cn/api/v";

        #endregion

        #region Constructor

        public RestClient(
            AuthorizationType authorizationType,
            string token)
        {
            AuthorizationHeader = authorizationType switch
            {
                AuthorizationType.Bot => $"Bot {token}",
                AuthorizationType.Oauth2 => $"Bearer {token}",
                _ => throw new ArgumentOutOfRangeException(nameof(authorizationType)),
            };
        }

        #endregion

        #region Authorization

        public string AuthorizationHeader;

        #endregion

        #region Request Utils

        internal static HttpWebRequest CreateRequest(
            string endpoint,
            bool post = false) =>
            RequestHelper.CreateWebRequest(BaseUrl + endpoint, post);

        #endregion

        #region Message

        protected internal override Task<BotBase> SendTextMessage(long channel, string message)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void Dispose()
        {
        }
    }

    public enum AuthorizationType
    {
        Bot = 0,
        Oauth2 = 1
    }
}
