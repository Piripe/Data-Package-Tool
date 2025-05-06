using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics.Abstract
{
    public class GuildEvent : AnalyticsEvent, IGuildEvent
    {
        [JsonPropertyName("guild_id")]
        public string? GuildId { get; set; }
    }
}
