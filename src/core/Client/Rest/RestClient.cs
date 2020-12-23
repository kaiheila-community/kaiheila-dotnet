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

        private HttpWebRequest CreateRequest(
            string endpoint,
            bool post = false)
        {
            HttpWebRequest request = RequestHelper.CreateWebRequest(
                Options.BaseUrl + (Options.APIVersion > 0 ? Options.APIVersion.ToString() : "") + endpoint, post);

            request.Headers.Add("Authorization", Options.AuthorizationHeader);

            if (post) request.ContentType = "application/json";

            return request;
        }

        #endregion

        #region Message

        protected internal override Task SendTextMessage(
            int type,
            long channel,
            string message,
            string quote = null)
        {
            HttpWebRequest request = CreateRequest(@"/channel/message", true);
        }

        #endregion

        public override void Dispose()
        {
        }
    }
}
