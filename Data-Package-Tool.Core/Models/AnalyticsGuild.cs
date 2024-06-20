using System;
using System.Collections.Generic;

namespace DataPackageTool.Core.Models
{
    public class AnalyticsGuild
    {
        public string Id { get; set; } = "";
        public string JoinType { get; set; } = null!;
        public string JoinMethod { get; set; } = null!;
        public long ApplicationId { get; set; }
        public string Location { get; set; } = null!;
        public List<string> Invites { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
