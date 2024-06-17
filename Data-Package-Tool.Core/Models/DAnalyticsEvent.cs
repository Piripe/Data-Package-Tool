using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models
{
    public class DAnalyticsEvent
    {
        [JsonPropertyName("event_type")]
        public string EventType { get; set; } = null!;

        [JsonPropertyName("guild")]
        public string GuildId { get; set; } = null!; // the guild id on invite events
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = null!; // the channel id on voice events
        [JsonPropertyName("channel_type")]
        public string ChannelType { get; set; } = null!;
        [JsonPropertyName("invite")]
        public string InviteCode { get; set; } = null!;

        [JsonPropertyName("guild_id")]
        private string GuildId2 { set => GuildId = value; }
        [JsonPropertyName("join_type")]
        public string JoinType { get; set; } = null!;
        [JsonPropertyName("join_method")]
        public string JoinMethod { get; set; } = null!;
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; } = null!;
        [JsonPropertyName("location")]
        public string Location { get; set; } = null!;
        [JsonPropertyName("invite_code")]
        private string InviteCode2 { set => InviteCode = value; }
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("timestamp")]
        private string Timestamp2 { set => Timestamp = DateTime.Parse(value.Replace("\"", ""), null, DateTimeStyles.RoundtripKind); }

        [JsonPropertyName("duration")]
        public long Duration { get; set; } // the call duration on voice disconnect events
    }
}
