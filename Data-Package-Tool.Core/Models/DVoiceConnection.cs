using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Package_Tool.Classes.Parsing
{
    public class DVoiceConnection
    {
        public string GuildId { get; set; } = null!;
        public string ChannelId { get; set; } = null!;
        public int? ChannelType { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
