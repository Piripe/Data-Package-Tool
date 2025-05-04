using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics.Abstract
{
    public class DurationChannelEvent : ChannelEvent
    {
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Duration;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
