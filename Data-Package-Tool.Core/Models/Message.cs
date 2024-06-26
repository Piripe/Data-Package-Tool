using System;
using System.Collections.Generic;

namespace DataPackageTool.Core.Models
{
    public class Message : DataPackageEntryBase
    {
        public string? Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; } = null!;
        public List<Attachment> Attachments { get; } = new List<Attachment>();
        public Channel Channel { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public string GetMessageLink()
        {
            string guild;
            if (this.Channel.Guild != null)
            {
                guild = Channel.Guild.Id.ToString();
            }
            else if (this.Channel.IsDM() || this.Channel.IsGroupDM())
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
