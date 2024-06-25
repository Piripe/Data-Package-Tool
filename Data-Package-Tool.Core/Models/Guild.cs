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

        private bool _fetchedInviteData;
        private Invite? _inviteData;

        public IImage? IconImage { get; set; }

        private async Task FetchInviteData()
        {
            _fetchedInviteData = true;
            foreach (var invite in Invites)
            {
                string inviteReq = await DRequest.GetStringAsync("invites/"+invite) ?? "{}";
                Debug.WriteLine(inviteReq);
                Invite? inviteData = JsonSerializer.Deserialize<Invite>(inviteReq,Shared.JsonSerializerOptions);
                if (inviteData == null) continue;
                if (inviteData.GuildId != Id) continue;

                _inviteData = inviteData;
                break;
            }
        }

        public async Task<IImage> GetIcon()
        {
            if (IconImage != null) return IconImage;
            IconImage = await DownloadIcon();
            
            return IconImage;
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
