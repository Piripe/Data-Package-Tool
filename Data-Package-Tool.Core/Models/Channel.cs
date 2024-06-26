using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.FileIO;

namespace DataPackageTool.Core.Models
{
    public class Channel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("guild")]
        public PartialGuild Guild { get; set; } = null!;
        [JsonPropertyName("recipients")]
        public List<string> RecipientIds { get; set; } = null!;

        public List<Message> Messages { get; } = new List<Message>();
        public string? DMRecipientId { get; set; }
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
                Channel = this
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
            return this.Type == 1;
        }

        public bool IsGroupDM()
        {
            return this.Type == 3;
        }

        public bool IsVoice()
        {
            return this.Type == 2 || this.Type == 13;
        }

        public string GetOtherDMRecipient(User user)
        {
            if(!this.IsDM())
            {
                throw new Exception("GetDMRecipient can only be used on dm channels");
            }

            foreach(string id in RecipientIds)
            {
                if(id != user.Id.ToString())
                {
                    return id;
                }
            }

            throw new Exception("This shouldn't happen");
        }
    }
}
