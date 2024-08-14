using DataPackageTool.Core.Models.Analytics.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics
{
    public class VoiceDisconnect : ChannelEvent
    {
        [JsonPropertyName("duration_connected")]
        public int DurationConnected { get; set; }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ DurationConnected;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
