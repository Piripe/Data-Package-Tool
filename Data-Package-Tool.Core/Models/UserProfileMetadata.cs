using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models
{
    public class UserProfileMetadata
    {
        [JsonPropertyName("user_id")]
        public string? Id { get; set; }
        [JsonPropertyName("boosting_started_at")]
        public DateTime? BoosingStartedAt { get; set; }
        [JsonPropertyName("premium_started_at")]
        public DateTime? NitroStartedAt { get; set; }
        [JsonPropertyName("legacy_username")]
        public string? LegacyUsername { get; set; }

        public int GetBoostingLevel(DateTime packageDate) {
            if (!BoosingStartedAt.HasValue) return 0;
            int months = packageDate.Subtract(BoosingStartedAt.Value).Days / 30; // Approximative calculation of the months count
            return Constants.BoostLevels.IndexOf(Constants.BoostLevels.LastOrDefault(x => x <= months,-1)) + 1;
        }
    }
}
