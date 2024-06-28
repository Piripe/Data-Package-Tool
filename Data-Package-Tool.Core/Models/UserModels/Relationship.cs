using DataPackageTool.Core.Enums;

namespace DataPackageTool.Core.Models.UserModels
{
    public class Relationship
    {
        public string? Id { get; set; }

        // Data packages from before 04.2024 have `type` set to a number,
        // while the current ones have it set to a string.
        // The enum resolves both automatically.
        public RelationshipType Type { get; set; }
        public string? Nickname { get; set; }
        public User? User { get; set; }
    }
}
