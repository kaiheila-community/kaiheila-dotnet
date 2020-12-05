using System.Net;

namespace Kaiheila.Net
{
    public static class RequestHelper
    {
        public const string UserAgent =
            @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) kaiheila/0.0.27 Chrome/80.0.3987.158 Electron/8.2.0 Safari/537.36";

        public const string Accept =
            @"*/*";

        public static HttpWebRequest CreateWebRequest(string uri, bool post = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = UserAgent;
            request.Accept = Accept;

            if (post) request.Method = "POST";

            return request;
        }
    }
}
