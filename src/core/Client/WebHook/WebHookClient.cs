using Kaiheila.Client.Rest;

namespace Kaiheila.Client.WebHook
{
    public class WebHookClient : RestClient
    {
        #region Constructor

        internal WebHookClient(WebHookClientOptions options) : base(options)
        {
        }

        public static WebHookClientBuilder CreateWebHookClient() => new WebHookClientBuilder();

        #endregion
    }
}
