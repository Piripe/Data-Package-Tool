using DataPackageTool.Core.Models.Analytics.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics
{
    public class AcceptedInstantInvite : AnalyticsEvent, IChannelEvent, IGuildEvent
    {
        [JsonPropertyName("channel")]
        public string? ChannelId { get; set; }
        [JsonPropertyName("channel_type")]
        public string? ChannelType { get; set; }
        [JsonPropertyName("guild")]
        public string? GuildId { get; set; }
        public string? Invite { get; set; }
        [JsonPropertyName("location_channel_id")]
        public string? LocationChannelId { get; set; }
        [JsonPropertyName("location_channel_type")]
        public string? LocationChannelType { get; set; }
        [JsonPropertyName("location_message_id")]
        public string? LocationMessageId { get; set; }
    }
}
