using System;
using System.Net;
using System.Threading.Tasks;
using Kaiheila.Net;

namespace Kaiheila.Client.Rest
{
    public class RestClient : Bot
    {
        #region Constructor

        public RestClient(
            RestClientOptions options)
        {
            Options = options;
        }

        public static RestClientBuilder CreateRestClient() => new RestClientBuilder();

        #endregion

        #region Lifecycle

        public override void Start()
        {
        }

        #endregion

        #region Options

        public readonly RestClientOptions Options;

        #endregion

        #region Request Utils

        internal HttpWebRequest CreateRequest(
            string endpoint,
            bool post = false) =>
            RequestHelper.CreateWebRequest(Options.BaseUrl + endpoint, post);

        #endregion

        #region Message

        protected internal override Task<Bot> SendTextMessage(
            int type,
            long channel,
            string message,
            string quote = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void Dispose()
        {
        }
    }
}
