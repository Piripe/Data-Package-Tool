using DataPackageTool.Core.Models.Analytics.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics
{
    public class SendMessage : AnalyticsEvent, IChannelEvent
    {
        [JsonPropertyName("message_id")]
        public string? MessageId { get; set; }
        [JsonPropertyName("channel")]
        public string? ChannelId { get; set; }
        [JsonPropertyName("channel_type")]
        public string? ChannelType { get; set; }
        [JsonPropertyName("is_friend")]
        public bool IsFriend { get; set; }
        public bool Private { get; set; }
        [JsonPropertyName("num_attachments")]
        public int NumAttachments { get; set; }
        [JsonPropertyName("max_attachment_size")]
        public int MaxAttachmentSize { get; set; }
        public int Length { get; set; }
        [JsonPropertyName("word_count")]
        public int WordCount { get; set; }
        [JsonPropertyName("mention_everyone")]
        public bool MentionEveryone { get; set; }
        [JsonPropertyName("attachment_ids")]
        public string[]? AttachmentIds { get; set; }

    }
}
