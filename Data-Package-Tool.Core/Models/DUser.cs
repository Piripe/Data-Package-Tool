using DataPackageTool.Helpers;
using System.Text.Json.Serialization;

namespace DataPackageTool.Classes.Parsing
{
    public class DUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("global_name")]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; } = string.Empty;
        [JsonPropertyName("avatar_hash")]
        public string AvatarHash { get; set; } = string.Empty;
        [JsonPropertyName("avatar")]
        private string AvatarHash2 { set => AvatarHash = value; } // relationship user field
        [JsonPropertyName("relationships")]
        public List<DRelationship> Relationships { get; set; } = null!;
        [JsonPropertyName("notes")]
        public Dictionary<string, string> Notes { get; set; } = null!;

        //public BitmapImage AvatarImage { get; set; }
        public bool IsPomelo
        {
            get => this.Discriminator == "0" || this.Discriminator == "0000";
        }
        public bool IsDeletedUser
        {
            get => this.Id == Constants.DeletedUserId;
        }
        public string Tag
        {
            get => this.IsPomelo ? this.Username : $"{this.Username}#{this.Discriminator}";
        }
        private int DefaultAvatarId
        {
            get
            {
                if(this.IsPomelo)
                {
                    return (int)((Int64.Parse(this.Id) >> 22) % 6);
                } else
                {
                    return Int32.Parse(this.Discriminator) % 5;
                }
            }
        }
        public string AvatarURL
        {
            get
            {
                string avatarHash = this.AvatarHash;
                if (avatarHash != null)
                {
                    return $"https://cdn.discordapp.com/avatars/{this.Id}/{avatarHash}.png?size=64";
                }

                return $"https://cdn.discordapp.com/embed/avatars/{this.DefaultAvatarId}.png?size=64";
            }
        }

        //public Bitmap GetDefaultAvatarBitmap()
        //{
        //    return Properties.Resources.ResourceManager.GetObject($"DefaultAvatar{this.DefaultAvatarId}") as Bitmap;
        //}

        //public BitmapImage GetDefaultAvatarBitmapImage()
        //{
        //    return Discord.DefaultAvatars[DefaultAvatarId];
        //}
    }
}
