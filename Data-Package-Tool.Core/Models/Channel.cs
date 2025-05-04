using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using DataPackageTool.Core.Enums;
using Microsoft.VisualBasic.FileIO;

namespace DataPackageTool.Core.Models
{
    public class Channel : DataPackageEntryBase
    {
        public string Id { get; set; } = "";
        public ChannelType Type { get; set; }
        public string? Name { get; set; }
        public Guild? Guild { get; set; }
        [JsonPropertyName("recipients")]
        public List<string> RecipientIds { get; set; } = [];

        public HashSet<Message> Messages { get; } = [];
        public string? DMRecipientId { get; set; }
        private User? _DMRecipient;
        public User? DMRecipient { get => DMRecipientId == null ? null : (_DMRecipient ?? ((DataPackage?.UsersMap.TryGetValue(DMRecipientId, out User? usr) ?? false) ? usr : new User() { Id = DMRecipientId})); }
        public bool HasDuplicates { get; set; }

        public void LoadMessagesFromCsv(Stream csv)
        {
            using(TextFieldParser parser = new TextFieldParser(csv))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields()!;

                    string idField = fields[0];
                    string timestampField = fields[1];
                    string contentField = fields[2];
                    string attachmentsField = fields[3];

                    if (idField == "ID") continue; // Header collumns

                    AddMessage(idField, timestampField, contentField, attachmentsField);
                }
            }
        }

        public void LoadMessagesFromJson(Stream json)
        {
            var jsonMsgArray = JsonNode.Parse(json)?.AsArray();
            if (jsonMsgArray == null) return;
            foreach (var jsonMsg in jsonMsgArray)
            {
                if (jsonMsg == null) continue;
                string idField = jsonMsg["ID"]?.ToString()??"";
                string timestampField = jsonMsg["Timestamp"]?.ToString() ?? "";
                string contentField = jsonMsg["Contents"]?.ToString() ?? "";
                string attachmentsField = jsonMsg["Attachments"]?.ToString() ?? "";
                AddMessage(idField, timestampField, contentField, attachmentsField);
            }
        }

        private void AddMessage(string id, string timestamp, string contents, string attachments)
        {
            var msg = new Message
            {
                Id = id,
                Timestamp = DateTime.Parse(timestamp),
                Content = contents,
                Channel = this,
                Author = DataPackage?.User,
            };

            if (attachments != "")
            {
                foreach (var url in attachments.Split(' '))
                {
                    var attachment = new Attachment(url, msg);
                    msg.Attachments.Add(attachment);
                }
            }

            this.Messages.Add(msg);
        }

        public bool IsDM()
        {
            return this.Type == ChannelType.DM;
        }

        public bool IsGroupDM()
        {
            return this.Type == ChannelType.GROUP_DM;
        }

        public bool IsVoice()
        {
            return this.Type == ChannelType.GUILD_VOICE || this.Type == ChannelType.GUILD_STAGE_VOICE;
        }

        public string GetOtherDMRecipient(User user)
        {
            if(!this.IsDM())
            {
                throw new Exception("GetDMRecipient can only be used on dm channels");
            }

            foreach(string id in RecipientIds)
            {
                if(id != user.Id?.ToString())
                {
                    return id;
                }
            }

            throw new Exception("This shouldn't happen");
        }
        public TimeSpan VoiceTimeIn => TimeSpan.FromSeconds(DataPackage?.VoiceDisconnections.Where(x => x.ChannelId == Id).Select(x => x.DurationConnected).Sum() ?? 0);
    }
}
