using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Utils;
using Avalonia.Controls;

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

        private bool _fetchedData;
        private Invite? _inviteData;


        private async Task FetchData(DataSourceUsability neededUsability = DataSourceUsability.Auto, bool partialData = true)
        {
            if (DataPackage == null) return;
            _fetchedData = true;

            object? DeserializeGuild(string json) => JsonSerializer.Deserialize<Guild>(json, Shared.JsonSerializerOptions);
            object? DeserializeInvite(string json) => JsonSerializer.Deserialize<Invite>(json, Shared.JsonSerializerOptions);

            object? res = await DataPackage.GetObjectFromSources(neededUsability,
                    (partialData ? new List<DRequest>() : new List<DRequest>() {
                        DRequest.Get("guilds/"+Id,context:DRequestContext.Bot),
                        DRequest.Get("guilds/"+Id,context:DRequestContext.User)
                    }).Concat(Invites.Select(x=>DRequest.Get("invites/"+x,context:DRequestContext.Invite,queue:"invite"))).ToList(),
                    Enumerable.Repeat(DeserializeGuild, partialData ? 0 : 2).Concat(Enumerable.Repeat(DeserializeInvite,Invites.Count)).ToList(),
                    (x, _) =>
                    {
                        switch (x)
                        {
                            case Invite invite:
                                return invite.GuildId == Id;
                            case Guild guild:
                                return true;
                            default:
                                return false;
                        }
                    }
                );

            switch (res)
            {
                case Invite invite:
                    _inviteData = invite;
                    Shared.Mapper.Map(invite.Guild, this);
                    break;
                case Guild guild:
                    Shared.Mapper.Map(guild, this);
                    break;
                default:
                    _fetchedData = false;
                    break;
            }


            /*
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
            */
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
        public async Task<string> GetNameAsync()
        {
            if (Name != null) return Name;

            if (!_fetchedData)
            {
                await FetchData();
            }

            return Name ?? Id;
        }

        async Task<IImage> DownloadIcon()
        {
            if (Icon == null && !_fetchedData)
            {
                await FetchData();
            }
            if (Icon != null)
            {
                if (Icon == null) return User.GetDefaultAvatarBitmap(1);
                Stream? iconStream = (await DRequest.client.GetAsync(new Uri(new Uri(Constants.CDNEndpoint), $"icons/{Id}/{Icon}.png?size=256"))).Content.ReadAsStream();
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
