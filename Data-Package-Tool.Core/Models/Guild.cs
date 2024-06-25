using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace DataPackageTool.Core.Models
{
    public class Guild
    {
        public string Id { get; set; } = "";
        public string JoinType { get; set; } = null!;
        public string JoinMethod { get; set; } = null!;
        public long ApplicationId { get; set; }
        public string Location { get; set; } = null!;
        public List<string> Invites { get; set; } = new();
        public DateTime Timestamp { get; set; }
        public DataPackage? DataPackage { get; set; }

        private bool _fetchedInviteData;
        private Invite? _inviteData;


        private async Task FetchInviteData()
        {
            _fetchedInviteData = true;
            foreach (var invite in Invites)
            {
                Invite? inviteData = JsonSerializer.Deserialize<Invite>(await DRequest.GetStringAsync("invites/" + invite) ?? "{}", Shared.JsonSerializerOptions);
                if (inviteData == null) continue;
                if (inviteData.GuildId != Id) continue;

                _inviteData = inviteData;
                break;
            }
        }

        private IImage? _iconImage;
        public async Task<IImage> GetIcon()
        {
            if (_iconImage != null) return _iconImage;
            _iconImage = await DownloadIcon();

            return _iconImage;
        }
        private string? _name;
        public async Task<string> GetName()
        {
            if (_name != null) return _name;

            if (!_fetchedInviteData)
            {
                await FetchInviteData();
            }
            _name = _inviteData?.Guild?.Name ?? ((DataPackage?.GuildNamesMap.TryGetValue(Id,out string? name)??false) ? name : null);

            return _name ?? Id;
        }

        async Task<Bitmap> DownloadIcon()
        {
            if (!_fetchedInviteData)
            {
                await FetchInviteData();
            }
            if (_inviteData != null)
            {
                string? iconHash = _inviteData.Guild?.Icon;
                if (iconHash == null) return User.GetDefaultAvatarBitmap(1);
                Stream? iconStream = await DRequest.GetStreamAsync($"icons/{Id}/{iconHash}.png?size=256",true);
                if (iconStream != null)
                {
                    return new Bitmap(iconStream);
                }
                else
                {
                    return User.GetDefaultAvatarBitmap(1);
                }
            }
            else
            {
                return User.GetDefaultAvatarBitmap(1);
            }
            
        }
    }
}
