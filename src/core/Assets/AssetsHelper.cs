using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Kaiheila.Net;

namespace Kaiheila.Assets
{
    public static class AssetsHelper
    {
        public static string GetAssetType(string path)
        {
            string[] split = path.Split('/');

            if (split.Length < 3 || string.IsNullOrEmpty(split[1]))
                throw new ArgumentOutOfRangeException(nameof(path), "Path不正确。");

            return split[1];
        }

        #region Prefix

        private const string FilePrefix = @"file://";

        private const string HttpPrefix = @"http://";

        private const string Base64Prefix = @"base64://";

        #endregion

        public static async Task<Stream> GetAssetFile(string url)
        {
            if (url.StartsWith(FilePrefix))
            {
                url = url.Replace(FilePrefix, "");
                return File.OpenRead(url);
            }

            if (url.StartsWith(HttpPrefix))
            {
                HttpWebRequest request = RequestHelper.CreateWebRequest(url);
                return (await request.GetResponseAsync()).GetResponseStream();
            }

            if (url.StartsWith(Base64Prefix))
            {
                url = url.Replace(Base64Prefix, "");
                byte[] bytes = Convert.FromBase64String(url);
                return new MemoryStream(bytes, false);
            }

            throw new InvalidOperationException($"不支持的Url：{url}");
        }
    }
}
