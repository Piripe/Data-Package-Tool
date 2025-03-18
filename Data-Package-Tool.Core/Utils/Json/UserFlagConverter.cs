using DataPackageTool.Core.Enums;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Utils.Json
{
    internal class UserFlagConverter : JsonConverter<UserFlag>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(UserFlag) == typeToConvert;
        }
        public override UserFlag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            UserFlag? Read(ref Utf8JsonReader reader)
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    Debug.WriteLine("end");
                    return null;
                }
                if (reader.TokenType == JsonTokenType.Number)
                {
                    Debug.WriteLine("int");
                    return (UserFlag)reader.GetInt32();
                }
                if (reader.TokenType == JsonTokenType.String)
                {
                    Debug.WriteLine("str");
                    return (UserFlag)Enum.Parse(typeof(UserFlag), reader.GetString() ?? string.Empty, true);
                }
                return null;
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                UserFlag result = 0;
                UserFlag? newFlag;
                do
                {
                    reader.Read();
                    newFlag = Read(ref reader);
                    if (newFlag.HasValue) result |= newFlag.Value;
                } while (newFlag.HasValue);
                return result;
            } else
            {
                UserFlag? result = Read(ref reader);
                if (result.HasValue) return result.Value;
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return (UserFlag)document.RootElement.Clone().GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, UserFlag value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((long)value);
        }
    }
}
