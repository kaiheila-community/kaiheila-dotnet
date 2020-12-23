using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Kaiheila.Net;
using Newtonsoft.Json.Linq;

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

        private async Task<JToken> Post(
            string endpoint,
            JToken payload)
        {
            try
            {
                HttpWebRequest request = CreateRequest(endpoint, true);

                // Write payload
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload.ToString()));
                await stream.CopyToAsync(request.GetRequestStream());

                JObject response =
                    JObject.Parse(await new StreamReader((await request.GetResponseAsync()).GetResponseStream()!)
                        .ReadToEndAsync());

                // ReSharper disable PossibleNullReferenceException

                if (response["code"].ToObject<int>() != 0)
                    throw new RestClientException(response["code"].ToObject<int>(),
                        response["message"].ToObject<string>());

                // ReSharper restore PossibleNullReferenceException

                return response["data"];
            }
            catch (RestClientException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RestClientException(0, string.Empty, e);
            }
        }

        #endregion

        #region Message

        protected internal override async Task<(string, long)> SendTextMessage(
            int type,
            long channel,
            string content,
            string quote = null)
        {
            JToken response =
                await Post(
                    @"/channel/message",
                    JObject.FromObject(
                        new
                        {
                            object_name = type,
                            channel_id = channel,
                            content,
                            quote
                        }));

            try
            {
                // ReSharper disable PossibleNullReferenceException

                string msgId = response["msg_id"].ToObject<string>();
                long msgTimestamp = response["msg_timestamp"].ToObject<long>();

                // ReSharper restore PossibleNullReferenceException

                return (msgId, msgTimestamp);
            }
            catch (Exception e)
            {
                throw new RestClientException(0, string.Empty, e);
            }
        }

        #endregion

        public override void Dispose()
        {
        }
    }

    [Serializable]
    public class RestClientException : Exception
    {
        public RestClientException()
        {
        }

        public RestClientException(int errCode, string message) : base(message)
        {
            ErrCode = errCode;
        }

        public RestClientException(int errCode, string message, Exception inner) : base(message, inner)
        {
            ErrCode = errCode;
        }

        public int ErrCode { get; set; }
    }
}
