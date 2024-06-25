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
        public InviteGuild? Guild { get; set; }
        public InviteChannel? Channel { get; set; }
        public InviteInviter? Inviter { get; set; }
        public DateTime? ExpireAt { get; set; }
        [JsonPropertyName("guild_id")]
        public string? GuildId { get; set; }

    }
}
