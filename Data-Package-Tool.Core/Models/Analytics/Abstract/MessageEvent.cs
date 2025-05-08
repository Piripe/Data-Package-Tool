using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics.Abstract
{
    public class MessageEvent : ChannelEvent
    {
        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }
    }
}
