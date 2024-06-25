using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using DataPackageTool.Core.Models;
using DataPackageTool.Core.Enums;

namespace Data_Package_Tool.Core.Utils.Json
{
    public class InviteTypeConverter : JsonConverter<InviteType>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(InviteType) == typeToConvert;
        }
        public override InviteType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (InviteType)reader.GetInt32();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return (InviteType)Enum.Parse(typeof(InviteType),reader.GetString()??string.Empty,true);
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return (InviteType)document.RootElement.Clone().GetInt32();
            }
        }

        public override void Write(Utf8JsonWriter writer, InviteType value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value);
        }
    }
}
