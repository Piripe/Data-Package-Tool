using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Models.UserModels;
using System.Net;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models
{
    public class User : DataPackageEntryBase
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        [JsonPropertyName("global_name")]
        public string? DisplayName { get; set; }
        public string? Discriminator { get; set; }
        [JsonPropertyName("avatar_hash")]
        public string? AvatarHash { get; set; }
        [JsonPropertyName("avatar")]
        private string _avatar { set => AvatarHash = value; } // relationship user field
        public Dictionary<string, string>? Notes { get; set; }
        public UserFlag Flags { get; set; }

        [JsonPropertyName("user_profile_metadata")]
        public UserProfileMetadata? ProfileMetadata { get; set; }
        public List<Relationship>? Relationships { get; set; }
        public UserSettingsCategory? Settings { get; set; }

        public IImage? AvatarImage { get; set; }
        public bool IsPomelo
        {
            get => Discriminator == "0" || Discriminator == "0000" || Discriminator == null;
        }
        public bool IsDeletedUser
        {
            get => Id == Constants.DeletedUserId;
        }
        public string? Tag
        {
            get => IsPomelo ? Username : $"{Username}#{Discriminator}";
        }
        private int DefaultAvatarId
        {
            get
            {
                if(IsPomelo)
                {
                    return (int)((long.Parse(Id??"0") >> 22) % 6);
                } else
                {
                    return int.Parse(Discriminator??"0") % 5;
                }
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
            return GetDefaultAvatarBitmap(DefaultAvatarId);
        }
        public static Bitmap GetDefaultAvatarBitmap(int avatarId)
        {
            return new Bitmap(AssetLoader.Open(new Uri($"avares://DataPackageTool.UI/Assets/Discord/DefaultAvatar{avatarId}.png")));
        }
        async Task<Bitmap> DownloadAvatar()
        {
            Stream? iconStream = await DRequest.GetStreamAsync($"avatars/{Id}/{AvatarHash}.png?size=256", true, queue: "cdn");
            if (iconStream != null)
            {
                return new Bitmap(iconStream);
            }
            else
            {
                return GetDefaultAvatarBitmap();
            }
        }
        public string GetUsername() => DisplayName ?? Tag ?? (Id == Constants.DeletedUserId ? "Deleted User" : (Id == null  ? "Unknown User" : $"<@{Id}>"));
    }
}
