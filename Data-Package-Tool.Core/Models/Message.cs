using System;
using System.Collections.Generic;

namespace DataPackageTool.Core.Models
{
    public class Message : DataPackageEntryBase
    {
        public string Id { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string? Content { get; set; }
        public List<Attachment> Attachments { get; } = new();
        public Channel? Channel { get; set; }
        public User? Author { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string? GetMessageLink()
        {
            if (Channel == null) return null;
            string guild;
            if (Channel.Guild != null)
            {
                guild = Channel.Guild.Id;
            }
            else if (Channel.IsDM() || Channel.IsGroupDM())
            {
                guild = "@me";
            }
            else
            {
                throw new Exception($"Unable to find the server this message was sent in. This usually happens if you've left the server.");
            }

            return $"{guild}/{this.Channel.Id}/{this.Id}";
        }
    }
}
