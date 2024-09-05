using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using DataPackageTool.Core.Models;
using DataPackageTool.Core.Enums;
using System.Diagnostics;

namespace Data_Package_Tool.Core.Utils.Json
{
    public class ChannelTypeConverter : JsonConverter<ChannelType>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ChannelType) == typeToConvert;
        }
        public override ChannelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (ChannelType)reader.GetInt32();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return (ChannelType)Enum.Parse(typeof(ChannelType),reader.GetString()??string.Empty,true);
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return (ChannelType)document.RootElement.Clone().GetInt32();
            }
        }

        public override void Write(Utf8JsonWriter writer, ChannelType value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value);
        }
    }
}
