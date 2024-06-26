using DataPackageTool.Core.Models.Analytics.Abstract;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models.Analytics
{
    public class GuildJoined : AnalyticsEvent, IGuildEvent
    {
        [JsonPropertyName("guild_id")]
        public string? GuildId { get; set; }
        [JsonPropertyName("join_method")]
        public string? JoinMethod { get; set; }
        [JsonPropertyName("join_type")]
        public string? JoinType { get; set; }
    }
}
