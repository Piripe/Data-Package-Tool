using DataPackageTool.Core.Models.Analytics.Abstract;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models.Analytics
{
    public class CreateGuild : AnalyticsEvent, IGuildEvent
    {
        [JsonPropertyName("guild_id")]
        public string? GuildId { get; set; }
    }
}
