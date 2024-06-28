using DataPackageTool.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models
{
    public partial class Invite
    {
        public InviteType Type { get; set; }
        public string Code { get; set; } = "";
        public Guild? Guild { get; set; }
        public Channel? Channel { get; set; }
        public User? Inviter { get; set; }
        public DateTime? ExpireAt { get; set; }
        [JsonPropertyName("guild_id")]
        public string? GuildId { get; set; }

    }
}
