using System.Text.Json.Serialization;

namespace Data_Package_Tool.Classes.Parsing
{
    public class DPartialGuild
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
