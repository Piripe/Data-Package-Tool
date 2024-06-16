using System;
using System.Collections.Generic;

namespace Data_Package_Tool.Classes.Parsing
{
    public class DAnalyticsGuild
    {
        public string Id { get; set; } = null!;
        public string JoinType { get; set; } = null!;
        public string JoinMethod { get; set; } = null!;
        public string ApplicationId { get; set; } = null!;
        public string Location { get; set; } = null!;
        public List<string> Invites { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
