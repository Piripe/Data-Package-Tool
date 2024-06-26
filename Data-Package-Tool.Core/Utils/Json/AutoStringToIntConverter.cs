using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data_Package_Tool.Core.Utils.Json
{
    // Copied from https://stackoverflow.com/a/59099589/20959896
    public class AutoStringToIntConverter : JsonConverter<int>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(int) == typeToConvert;
        }
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt32(out int l) ?
                    l :
                    (int)reader.GetDouble();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return int.TryParse(reader.GetString(), out int l) ? l : 0;
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return int.TryParse(document.RootElement.Clone().ToString(), out int l) ? l : 0;
            }
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
