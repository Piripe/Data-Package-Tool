using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models.Analytics.Abstract
{
    public class ChannelEvent : GuildEvent, IChannelEvent
    {
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }
        [JsonPropertyName("channel_type")]
        public string? ChannelType { get; set; }
        [JsonPropertyName("parent_channel_id")]
        public string? ParentChannelId { get; set; }
        [JsonPropertyName("parent_channel_type")]
        public string? ParentChannelType { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (ChannelId?.GetHashCode()??0);
        }
    }
}
