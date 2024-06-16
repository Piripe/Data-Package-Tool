using System;
using System.Collections.Generic;

namespace DataPackageTool.Classes.Parsing
{
    public class DMessage
    {
        public string Id { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public string Content { get; set; } = null!;
        public List<DAttachment> Attachments { get; } = new List<DAttachment>();
        public DChannel Channel { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public string GetMessageLink()
        {
            string guild;
            if (this.Channel.Guild != null)
            {
                guild = this.Channel.Guild.Id;
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
