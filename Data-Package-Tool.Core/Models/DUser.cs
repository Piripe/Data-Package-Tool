using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Net;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models
{
    public class DUser
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        [JsonPropertyName("global_name")]
        public string? DisplayName { get; set; }
        public string? Discriminator { get; set; }
        [JsonPropertyName("avatar_hash")]
        public string? AvatarHash { get; set; }
        [JsonPropertyName("avatar")]
        private string AvatarHash2 { set => AvatarHash = value; } // relationship user field
        [JsonPropertyName("relationships")]
        public List<DRelationship>? Relationships { get; set; }
        public Dictionary<string, string>? Notes { get; set; }

        public IImage? AvatarImage { get; set; }
        public bool IsPomelo
        {
            get => this.Discriminator == "0" || this.Discriminator == "0000";
        }
        public bool IsDeletedUser
        {
            get => this.Id == Constants.DeletedUserId;
        }
        public string? Tag
        {
            get => this.IsPomelo ? this.Username : $"{this.Username}#{this.Discriminator}";
        }
        private int DefaultAvatarId
        {
            get
            {
                if(this.IsPomelo)
                {
                    return (int)((Int64.Parse(this.Id??"0") >> 22) % 6);
                } else
                {
                    return Int32.Parse(this.Discriminator??"0") % 5;
                }
            }
        }
        public string AvatarURL
        {
            get
            {
                if (AvatarHash != null)
                {
                    return $"https://cdn.discordapp.com/avatars/{this.Id}/{AvatarHash}.png?size=64";
                }

                return $"https://cdn.discordapp.com/embed/avatars/{this.DefaultAvatarId}.png?size=64";
            }
        }

        public async Task<IImage> GetAvatar()
        {
            if (AvatarImage != null) return AvatarImage;
            if (AvatarHash == null)
            {
                AvatarImage = GetDefaultAvatarBitmap();
            }
            else
            {
                AvatarImage = await DownloadAvatar();
            }
            return AvatarImage;
        }

        Bitmap GetDefaultAvatarBitmap()
        {
            return new Bitmap(AssetLoader.Open(new Uri($"avares://DataPackageTool.UI/Assets/Discord/DefaultAvatar{DefaultAvatarId}.png")));
        }
        async Task<Bitmap> DownloadAvatar()
        {
            HttpResponseMessage res = await new HttpClient().GetAsync(AvatarURL);
            if (res.IsSuccessStatusCode) {
                return new Bitmap(res.Content.ReadAsStream());
            }
            else
            {
                return GetDefaultAvatarBitmap();
            }
        }
    }
}
