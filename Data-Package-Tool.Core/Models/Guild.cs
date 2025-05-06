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
        public DateTime? JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public DateTime CreationDate => SnowflakeUtils.FromSnowflake(ulong.TryParse(Id, out ulong v) ? v : 0);
        public long ApplicationId { get; set; }
        public string? Splash { get; set; }
        public string? Banner { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public List<string> Features { get; set; } = new();
        public List<string> Invites { get; set; } = new();
        public List<Channel> Channels { get; set; } = new();

        internal bool fetchedData;
        internal DataSourceUsability triedfetchedData = DataSourceUsability.NotUsable;
        private Invite? _inviteData;


        private async Task FetchData(DataSourceUsability neededUsability = DataSourceUsability.Auto)
        {
            if (DataPackage == null || fetchedData ||triedfetchedData <= neededUsability) return;
            fetchedData = true;
            triedfetchedData = neededUsability;

            object? DeserializeGuild(string json) => JsonSerializer.Deserialize<Guild>(json, Shared.JsonSerializerOptions);
            object? DeserializeInvite(string json) => JsonSerializer.Deserialize<Invite>(json, Shared.JsonSerializerOptions);

            object? res = await DataPackage.GetObjectFromSources(neededUsability,
                    [DRequest.Get("guilds/"+Id,context:DRequestContext.Bot),DRequest.Get("guilds/"+Id+"/preview",context:DRequestContext.User),
                    ..Invites.Select(x=>DRequest.Get("invites/"+x,context:DRequestContext.Invite,queue:"invite"))],
                    [..Enumerable.Repeat(DeserializeGuild, 2),..Enumerable.Repeat(DeserializeInvite,Invites.Count)],
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
                    Debug.WriteLine(JsonSerializer.Serialize(guild, new JsonSerializerOptions() { WriteIndented = true }));
                    Shared.Mapper.Map(guild, this);
                    break;
                default:
                    fetchedData = false;
                    break;
            }
        }

        private IImage? _iconImage;
        public IImage? GetIcon() => _iconImage;
        public async Task<IImage> GetIconAsync(DataSourceUsability neededUsability = DataSourceUsability.Auto)
        {
            if (_iconImage != null) return _iconImage;
            IImage icon = await DownloadIcon(neededUsability);
            if (_inviteData != null) _iconImage = icon;

            return icon;
        }
        public async Task<string> GetNameAsync(DataSourceUsability neededUsability = DataSourceUsability.Auto)
        {
            if (Name != null) return Name;

            await FetchData(neededUsability);

            return Name ?? Id;
        }

        async Task<IImage> DownloadIcon(DataSourceUsability neededUsability = DataSourceUsability.Auto)
        {
            if (Icon == null)
            {
                await FetchData(neededUsability);
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
