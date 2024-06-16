using System.Text.Json.Serialization;

namespace Data_Package_Tool.Classes.Parsing
{
    public class DRelationship
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        // Data packages from before 04.2024 have `type` set to a number,
        // while the current ones have it set to a string.
        // The enum resolves both automatically.
        [JsonPropertyName("type")]
        public RelationshipType Type { get; set; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = null!;
        [JsonPropertyName("user")]
        public DUser User { get; set; } = null!;
    }

    public enum RelationshipType
    {
        MONE = 0,
        FRIEND = 1,
        BLOCKED = 2,
        PENDING_INCOMING = 3,
        PENDING_OUTGOING = 4,
        IMPLICIT = 5
    }
}
