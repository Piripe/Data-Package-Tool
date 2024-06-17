using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models
{
    public class DRelationship
    {
        public string Id { get; set; } = null!;

        // Data packages from before 04.2024 have `type` set to a number,
        // while the current ones have it set to a string.
        // The enum resolves both automatically.
        public RelationshipType Type { get; set; }
        public string Nickname { get; set; } = null!;
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
