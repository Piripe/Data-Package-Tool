using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models
{
    public partial class Invite
    {
        public class InviteGuild
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string? Splash { get; set; }
            public string? Banner { get; set; }
            public string? Description { get; set; }
            public string? Icon { get; set; }
            public List<string> Features { get; set; } = new();
            // TODO : Add more entries
        }
    }
}
