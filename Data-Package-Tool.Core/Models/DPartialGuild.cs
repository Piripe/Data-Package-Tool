﻿using System.Text.Json.Serialization;

namespace DataPackageTool.Classes.Parsing
{
    public class DPartialGuild
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
