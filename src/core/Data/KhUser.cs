using System;
using Newtonsoft.Json;

namespace Kaiheila.Data
{
    [Obsolete]
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class KhUser
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; } = "";
    }
}
