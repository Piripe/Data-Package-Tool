using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace DataPackageTool.Core.Models
{
    public class Guild : DataPackageEntryBase
    {
        public string Id { get; set; } = "";
        public string? Name { get; set; }
        public string? JoinType { get; set; }
        public string? JoinMethod { get; set; }
        public long ApplicationId { get; set; }
        public string? Splash { get; set; }
        public string? Banner { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public List<string> Features { get; set; } = new();
        public List<string> Invites { get; set; } = new();
        public DateTime Timestamp { get; set; }

        private bool _fetchedInviteData;
        private Invite? _inviteData;


        private async Task FetchInviteData()
        {
            if (Invites.Count == 0) return;

            _fetchedInviteData = true;
            foreach (var invite in Invites)
            {
                Invite? inviteData = JsonSerializer.Deserialize<Invite>(await DRequest.GetStringAsync("invites/" + invite, queue: "invite") ?? "{}", Shared.JsonSerializerOptions);
                if (inviteData == null) continue;
                if (inviteData.GuildId != Id) continue;

                _inviteData = inviteData;
                break;
            }
        }

        private IImage? _iconImage;
        public IImage? GetIcon() => _iconImage;
        public async Task<IImage> GetIconAsync()
        {
            if (_iconImage != null) return _iconImage;
            IImage icon = await DownloadIcon();
            if (_inviteData != null) _iconImage = icon;

            return icon;
        }
        public string? GetName()
        {
            if (Name != null) return Name;

            if (_fetchedInviteData) Name = _inviteData?.Guild?.Name;

            return Name;
        }
        public async Task<string> GetNameAsync()
        {
            if (Name != null) return Name;

            if (!_fetchedInviteData)
            {
                await FetchInviteData();
            }
            Name = _inviteData?.Guild?.Name;

            return Name ?? Id;
        }

        async Task<IImage> DownloadIcon()
        {
            if (!_fetchedInviteData)
            {
                await FetchInviteData();
            }
            if (_inviteData != null)
            {
                string? iconHash = _inviteData.Guild?.Icon;
                if (iconHash == null) return User.GetDefaultAvatarBitmap(1);
                Stream? iconStream = await DRequest.GetStreamAsync($"icons/{Id}/{iconHash}.png?size=256", true, queue:"cdn");
                if (iconStream != null)
                {
                    return new Bitmap(iconStream);
                }
                else
                {
                    return DefaultIcon();
                }
            }
            else
            {
                return DefaultIcon();
            }

        }
        public IImage DefaultIcon()
        {
            return User.GetDefaultAvatarBitmap(1);
        }
    }
}
