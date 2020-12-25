using System;
using Newtonsoft.Json;

namespace Kaiheila.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class KhUser
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; } = "";

        [JsonProperty("nickname")]
        public string Nickname { get; set; } = "";
    }

    public interface IKhEventUser
    {
        public KhUser User { get; set; }
    }
}
