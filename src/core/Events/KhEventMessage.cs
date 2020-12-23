using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Kaiheila.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class KhEventMessage : KhEventBase
    {
        [Required]
        [JsonProperty("id")]
        public string Id;

        [Required]
        [JsonProperty("timestamp")]
        public long Timestamp;

        [Required]
        [JsonProperty("target_id")]
        public long TargetId;

        [Required]
        [JsonProperty("content")]
        public string Content;
    }
}
