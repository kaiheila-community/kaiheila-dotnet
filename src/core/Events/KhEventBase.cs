using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kaiheila.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kaiheila.Events
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class KhEventBase
    {
        #region Message Fields

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

        #endregion

        #region Parse

        public virtual async Task Parse(JToken payload)
        {
            // Abstract
            throw new NotImplementedException($"{GetType().Name}中无法调用Parse()。");
        }

        #endregion

        #region Send

        public virtual async Task Send(Bot bot)
        {
            // Abstract
            throw new NotImplementedException($"{GetType().Name}中无法调用Send()。");
        }

        #endregion
    }
}
