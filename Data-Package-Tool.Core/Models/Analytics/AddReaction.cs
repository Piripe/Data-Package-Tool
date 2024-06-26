using DataPackageTool.Core.Models.Analytics.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics
{
    public class AddReaction : ChannelEvent
    {
        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }
        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; set; }
        [JsonPropertyName("emoji_custom")]
        public bool EmojiCustom { get; set; }
        [JsonPropertyName("is_burst")]
        public bool IsBurst { get; set; }
    }
}
