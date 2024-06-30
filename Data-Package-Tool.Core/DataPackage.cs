using AutoMapper;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Data_Package_Tool.Core.Utils.Json;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Models;
using DataPackageTool.Core.Models.Analytics;
using DataPackageTool.Core.Models.UserModels;
using DataPackageTool.Core.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataPackageTool.Core
{
    public struct LoadStatus
    {
        public float Progress;
        public string Status;
        public bool Finished;
    }
    public class DataPackage
    {
        public User User { get; private set; } = new User()
#if DEBUG
        // Dummy user for debugging/previewing
        { DisplayName = "Dummy user", Flags = (UserFlag)0x400048, ProfileMetadata = new UserProfileMetadata() { LegacyUsername = "Dummy user#1564", NitroStartedAt = new DateTime(2023, 10, 5), BoosingStartedAt = new DateTime(2023, 10, 7) } };
#else
;
#endif
        public List<Channel> Channels { get; } = new();
        public Dictionary<string, Channel> ChannelsMap { get; } = new();

        public List<Guild> Guilds { get; private set; } = new()
#if DEBUG
        { new Guild() {
            Id = "603970300668805120",
            Invites = ["discord-603970300668805120"]
        } };
#else
            ;
#endif
        public Dictionary<string, Guild> GuildsMap { get; } = new();

        public Dictionary<string, Message> MessagesMap { get; } = new();

        public Dictionary<string, User> UsersMap { get; } = new();

        public List<Attachment> ImageAttachments { get; private set; } = new();
        public List<AnalyticsEvent> AnalyticsEvents { get; private set; } = new List<AnalyticsEvent>();
        public List<VoiceDisconnect> VoiceDisconnections { get; private set; } = new List<VoiceDisconnect>();

        public DateTime CreationTime { get; private set; } = DateTime.Now;

        public DataSourceUsability InviteUsability { get; set; } = DataSourceUsability.Manual;
        public DataSourceUsability SelfbotUsability { get; set; } = DataSourceUsability.None;
        public DataSourceUsability BotUsability { get; set; } = DataSourceUsability.None;

        public bool UsesUnsignedCDNLinks
        {
            get => ImageAttachments.Count > 0 && !ImageAttachments[0].Url.Contains("?ex=");
        }

        internal struct ZipEntryStreamAndMatches
        {
            public MemoryStream? stream;
            public MemoryStream? channelStream;
            public Match messageMatch;
            public bool avatarMatch;
            public bool analyticsMatch;
            public string? FullName;
        }
        public static Task<DataPackage> LoadAsync(string fileName, Action<LoadStatus> statusCallback)
        {
            string currentStatus = "Loading user infos";
            void UpdateStatus(float progress, string? status = null, bool finished = false)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    if (status != null) currentStatus = status;
                    statusCallback.Invoke(new LoadStatus { Status = currentStatus, Progress = progress, Finished = finished });
                });
            }
            return Task.Run(() =>
            {
                var startTime = DateTime.Now;

                UpdateStatus(0f);


                Shared.JsonSerializerOptions = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                };
                Shared.JsonSerializerOptions.Converters.Add(new RelationshipTypeConverter());
                Shared.JsonSerializerOptions.Converters.Add(new InviteTypeConverter());
                Shared.JsonSerializerOptions.Converters.Add(new AutoNumberToStringConverter());
                Shared.JsonSerializerOptions.Converters.Add(new AutoStringToIntConverter());

                DataPackage dp = new DataPackage();

                var file = File.OpenRead(fileName);
                var zip = new ZipArchive(file, ZipArchiveMode.Read);

                var userFile = zip.GetEntry("account/user.json");
                if (userFile == null)
                {
                    throw new Exception("Invalid data package: missing user.json");
                }

                dp.CreationTime = userFile.LastWriteTime.DateTime;



                dp.User = JsonSerializer.Deserialize<User>(userFile.Open(), Shared.JsonSerializerOptions) ?? dp.User;

                if (dp.User == null)
                {
                    throw new Exception("Invalid data package: wrong content in user.json");
                }

                if (dp.User.Relationships != null)
                {
                    foreach (var relationship in dp.User.Relationships)
                    {
                        if (relationship.User.Id != null) dp.UsersMap.TryAdd(relationship.User.Id, relationship.User);
                    }
                }
                else
                {
                    // TODO: Add a warning for missing relationships
                }

                var channelNamesMap = new Dictionary<string, string>();
                var messagesIndexFile = zip.GetEntry("messages/index.json");
                if (messagesIndexFile != null)
                {
                    channelNamesMap = JsonSerializer.Deserialize<Dictionary<string, string>>(messagesIndexFile.Open(), Shared.JsonSerializerOptions) ?? channelNamesMap;
                }
                else
                {
                    // TODO: Add a warning for missing messages/index.json
                }


                var serversIndexFile = zip.GetEntry("servers/index.json");
                if (serversIndexFile != null)
                {
                    var guildNamesMap = JsonSerializer.Deserialize<Dictionary<string, string>>(serversIndexFile.Open(), Shared.JsonSerializerOptions);
                    if (guildNamesMap != null)
                    {
                        dp.Guilds = guildNamesMap.Select(x => new Guild() { Id = x.Key, Name = x.Value }).ToList();
                    }
                }
                else
                {
                    // TODO: Add a warning for missing servers/index.json
                }

                var messagesRegex = new Regex(@"messages/(c?(\d+))/messages\.(csv|json)", RegexOptions.Compiled);
                var avatarRegex = new Regex(@"account/avatar\.[a-z]+", RegexOptions.Compiled);
                var nameRegex = new Regex(@"^Direct Message with (.+)#(\d{1,4})$", RegexOptions.Compiled);
                var activityRegex = new Regex(@"activity/(analytics|reporting)/events.+\.json", RegexOptions.Compiled);

                List<ZipArchiveEntry> analyticsFiles = new List<ZipArchiveEntry>();


                UpdateStatus(0.05f, "Reading messages...");
                int entriesCount = zip.Entries.Count;
                IEnumerable<ZipEntryStreamAndMatches> readedStreams = zip.Entries.Select((entry, i) =>
                {
                    if (i % 100 == 0) UpdateStatus(((float)i / entriesCount) * 0.45f + 0.05f);
                    var match = messagesRegex.Match(entry.FullName);
                    bool avatarMatch = false;
                    bool activityMatch = false;
                    MemoryStream? channelStream = null;
                    if (match.Success)
                    {
                        var channelFile = zip.GetEntry($"messages/{match.Groups[1].Value}/channel.json");
                        if (channelFile != null)
                        {
                            channelStream = new MemoryStream((int)entry.Length);
                            channelFile.Open().CopyTo(channelStream);
                        }
                        else
                        {
                            // TODO: Add a warning for missing messages/{folderName}/channel.json
                        }
                    }
                    else
                    {
                        avatarMatch = avatarRegex.IsMatch(entry.FullName);
                        if (!avatarMatch) activityMatch = activityRegex.IsMatch(entry.FullName);

                        if (activityMatch)
                        {
                            Debug.WriteLine($"{entry.FullName} is an analytics file");
                            analyticsFiles.Add(entry);
                        }
                    }
                    if (match.Success || avatarMatch)
                    {
                        var ms = new MemoryStream((int)entry.Length);
                        entry.Open().CopyTo(ms);
                        ms.Position = 0;
                        if (channelStream != null) channelStream.Position = 0;
                        return new ZipEntryStreamAndMatches() { stream = ms, messageMatch = match, avatarMatch = avatarMatch, channelStream = channelStream };
                    }
                    return new ZipEntryStreamAndMatches() { stream = null };
                }).Where((ZipEntryStreamAndMatches x) => x.stream != null).ToList();

                Task analyticsTask = Task.Run(() =>
                {
                    long analyticsLength = analyticsFiles.Sum(f => f.Length);
                    long bytesRead = 0;
                    int fileIndex = 0;

                    foreach (ZipArchiveEntry entry in analyticsFiles)
                    {
                        using (var reader = new StreamReader(entry.Open()))
                        {
                            int updateCounter = 1000;
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine() ?? "";
                                Task.Run(() =>
                                {
                                    bytesRead += line.Length;

                                    if (Constants.ParsedEvents.Any(x => line.StartsWith("{\"event_type\":\"" + x + "\"")))
                                    {
                                        AnalyticsEvent? e = JsonSerializer.Deserialize<AnalyticsEvent>(line, Shared.JsonSerializerOptions);
                                        if (e != null) dp.AnalyticsEvents.Add(e);
                                    }

                                    updateCounter++;
                                    if (updateCounter >= 1000)
                                    {
                                        updateCounter = 0;
                                        UpdateStatus(((float)bytesRead / analyticsLength) * 0.40f + 0.55f, $"Loading analytics ({fileIndex}/{analyticsFiles.Count})");
                                    }
                                });
                            }
                        }
                        fileIndex++;
                    }
                });

                List<object?> parsedEntries = readedStreams.AsParallel().WithDegreeOfParallelism(16).Select<ZipEntryStreamAndMatches, object?>((entry, i) =>
                {
                    if (entry.messageMatch.Success)
                    {
                        var channelId = long.Parse(entry.messageMatch.Groups[2].Value);
                        var folderName = entry.messageMatch.Groups[1].Value; // folder name might not start with "c" in older versions
                        var fileExtension = entry.messageMatch.Groups[3].Value;

                        Channel? channel = null;
                        if (entry.channelStream != null)
                        {
                            channel = JsonSerializer.Deserialize<Channel>(entry.channelStream);
                            entry.channelStream.Dispose();
                        }

                        if (channel == null)
                        {
                            // TODO: Add a warning for wrong data in messages/{folderName}/channel.json

                            channel = new Channel() { Id = channelId.ToString() }; // A partial channel should be enough
                        }

                        switch (fileExtension)
                        {
                            case "csv":
                                channel.LoadMessagesFromCsv(entry.stream!);
                                break;
                            case "json":
                                channel.LoadMessagesFromJson(entry.stream!);
                                break;
                        }

                        entry.stream!.Dispose();

                        return channel;
                    }
                    else if (entry.avatarMatch)
                    {
                        var avatar = new Bitmap(entry.stream!);

                        entry.stream!.Dispose();

                        return avatar;
                    }
                    return null;
                }).ToList();

                int i = 0;
                foreach (object? entry in parsedEntries)
                {
                    if (entry is Channel channel)
                    {
                        if (channel.IsDM())
                        {
                            var recipientId = channel.GetOtherDMRecipient(dp.User);
                            channel.DMRecipientId = recipientId;
                            if (!dp.UsersMap.ContainsKey(recipientId) && recipientId != Constants.DeletedUserId && channelNamesMap.TryGetValue(channel.Id ?? "", out var channelName))
                            {
                                var nameMatch = nameRegex.Match(channelName);
                                if (nameMatch.Success)
                                {
                                    dp.UsersMap.Add(recipientId, new User
                                    {
                                        Id = recipientId,
                                        Username = nameMatch.Groups[1].Value,
                                        Discriminator = nameMatch.Groups[2].Value
                                    });
                                }
                            }
                        }

                        foreach (var msg in channel.Messages)
                        {
                            dp.MessagesMap.Add(msg.Id, msg);
                            foreach (var attachment in msg.Attachments)
                            {
                                if (attachment.IsImage)
                                {
                                    dp.ImageAttachments.Add(attachment);
                                }
                            }
                        }
                        dp.Channels.Add(channel);
                        dp.ChannelsMap[channel.Id] = channel;
                    }
                    else if (entry is Bitmap avatar)
                    {
                        dp.User.AvatarImage = avatar;
                    }
                    else if (entry is ZipArchiveEntry zipEntry)
                    {
                        analyticsFiles.Add(zipEntry);
                    }
                    i++;
                    //if (i % 100 == 0) UpdateStatus((i / parsedEntries.Count) * 0.05f + 0.5f, $"Loading data ({i}/{parsedEntries.Count})");
                }

                analyticsTask.Wait();

                zip.Dispose();
                file.Dispose();

                void MergeGuild(Guild guild)
                {
                    Guild? alreadyIncludedGuild = dp.Guilds.FirstOrDefault(x => x.Id == guild.Id);
                    if (alreadyIncludedGuild == null)
                    {
                        dp.Guilds.Add(guild);
                        return;
                    }
                    Shared.Mapper.Map(guild,alreadyIncludedGuild);
                }

                var analyticsGroups = dp.AnalyticsEvents.GroupBy(x => x.GetType());

                foreach (var group in analyticsGroups)
                {
                    switch (group.First())
                    {
                        case GuildJoined:
                            IEnumerable<Guild> partialJoinedGuilds = group.Cast<GuildJoined>()
                            .Select(x => x.GuildId != null ? new Guild()
                            {
                                Id = x.GuildId,
                                JoinMethod = x.JoinMethod,
                                JoinType = x.JoinType
                            } : null).Where(x => x != null).Cast<Guild>();

                            foreach (Guild partialGuild in partialJoinedGuilds)
                            {
                                MergeGuild(partialGuild);
                            }
                            break;
                        case AcceptedInstantInvite:
                            IEnumerable<Guild> partialAcceptedInstantInviteGuilds = group.Cast<AcceptedInstantInvite>()
                            .Select(x => x.GuildId != null ? new Guild()
                            {
                                Id = x.GuildId,
                                Invites = [x.Invite]
                            } : null).Where(x => x != null).Cast<Guild>();

                            foreach (Guild partialGuild in partialAcceptedInstantInviteGuilds)
                            {
                                MergeGuild(partialGuild);
                            }
                            break;
                    }
                }

                dp.ImageAttachments = dp.ImageAttachments.OrderByDescending(o => o.Message.Id).ToList();
                List<GuildFolder>? folders = dp.User.Settings?.Settings?.GuildFolders?.Folders;
                if (folders != null)
                {
                    List<string> guildsOrder = folders.SelectMany(x => x.GuildIds ?? new List<string>()).ToList();
                    dp.Guilds.Sort((x,y) => {
                        int xPos = guildsOrder.IndexOf(x.Id);
                        int yPos = guildsOrder.IndexOf(y.Id);
                        if (xPos == -1)
                        {
                            xPos = int.TryParse(x.Id.Remove(8), out int xId) ? xId : int.MaxValue;
                        }
                        if (yPos == -1)
                        {
                            yPos = int.TryParse(y.Id.Remove(8), out int yId) ? yId : int.MaxValue;
                        }
                        return (xPos).CompareTo(yPos);
                        });
                }

                dp.Guilds.ForEach((x) => x.DataPackage = dp);

                UpdateStatus(1f, $"Finished! Parsed {dp.MessagesMap.Count.ToString("N0", new NumberFormatInfo { NumberGroupSeparator = " " })} messages in {Math.Floor((DateTime.Now - startTime).TotalSeconds)}s\nPackage created at: {dp.CreationTime.ToShortDateString()}", true);

                return dp;
            });
        }

        private bool _partialGuildsFetched = false;
        public async Task GetPartialGuilds()
        {
            try
            {
                if (_partialGuildsFetched) return;
                _partialGuildsFetched = true;

                List<Guild>? partialGuilds = await this.GetObjectFromSources(DataSourceUsability.Auto, [DRequest.Get("users/@me/guilds", context: DRequestContext.User)], [(HttpResponseMessage res) => JsonSerializer.Deserialize<List<Guild>>(res.Content.ReadAsStream(),Shared.JsonSerializerOptions)]);

                Debug.WriteLine("Partial guilds are null : "+(partialGuilds == null).ToString());
                if (partialGuilds == null) return;

                foreach (Guild guild in partialGuilds)
                {
                    Guild? packageGuild = Guilds.Find((x) => x.Id == guild.Id);
                    if (packageGuild == null) continue;

                    Shared.Mapper.Map(guild, packageGuild);
                    Debug.WriteLine($"Guild {guild.Name}({guild.Id}) now has icon : {guild.Icon}");
                }
            }
            catch (Exception ex) {
            Debug.WriteLine(ex); 
            }
        }


        public void LoadGuilds(string fileName)
        {
            throw new NotImplementedException();
            //var compiledRegex = new Regex(@"activity/reporting/events.+\.json", RegexOptions.Compiled);

            //using (var file = File.OpenRead(fileName))
            //using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            //{
            //    foreach (var entry in zip.Entries)
            //    {
            //        if (compiledRegex.IsMatch(entry.FullName))
            //        {
            //            using (var data = new StreamReader(entry.Open()))
            //            {
            //                long bytesRead = 0;
            //                while (!data.EndOfStream)
            //                {
            //                    var line = data.ReadLine();
            //                    bytesRead += line.Length;

            //                    this.GuildsLoadStatus.Progress = (int)((double)bytesRead / (long)entry.Length * 100);
            //                    ProcessAnalyticsLine(line);
            //                }
            //            }
            //        }
            //    }
            //}

            //this.AcceptedInvites = this.AcceptedInvites.OrderBy(o => o.Timestamp.Ticks).ToList();

            //foreach (var eventData in this.AcceptedInvites)
            //{
            //    var guild = this.JoinedGuilds.Find(x => x.Id == eventData.GuildId);
            //    if (guild == null)
            //    {
            //        this.JoinedGuilds.Add(new DAnalyticsGuild
            //        {
            //            Id = eventData.GuildId,
            //            JoinType = "invite",
            //            Invites = new List<string> { eventData.InviteCode },
            //            Timestamp = eventData.Timestamp
            //        });
            //    }
            //    else
            //    {
            //        if (!guild.Invites.Contains(eventData.InviteCode))
            //        {
            //            guild.Invites.Add(eventData.InviteCode);
            //        }

            //        // Handle the case where the original join didn't create a guild_join event, but did create accepted_instant_invite, and then a rejoin created a newer guild_join
            //        // (i.e. use older date from accepted_instant_invite if there is one)
            //        var joinDate = eventData.Timestamp;
            //        if (joinDate.Ticks < guild.Timestamp.Ticks)
            //        {
            //            guild.Timestamp = joinDate;
            //        }
            //    }
            //}

            //this.JoinedGuilds = this.JoinedGuilds.OrderByDescending(o => o.Timestamp.Ticks).ToList();
            //this.GuildsLoadStatus.Finished = true;

            //GC.Collect();
        }
        private static string[] AnalyticsLines = { "guild_joined", "create_guild", "accepted_instant_invite", "voice_disconnect" };
        private void ProcessAnalyticsLine(string line)
        {
            throw new NotImplementedException();

            //// Pro optimization
            //if (!AnalyticsLines.Any(x=>line.StartsWith("{\"event_type\":\""+x)))
            //{
            //    return;
            //}

            //var eventData = Newtonsoft.Json.JsonConvert.DeserializeObject<DAnalyticsEvent>(line);

            //switch (eventData.EventType)
            //{
            //    case "guild_joined":
            //    case "guild_joined_pending":
            //        var idx = this.JoinedGuilds.FindIndex(x => x.Id == eventData.GuildId);
            //        if (idx > -1)
            //        {
            //            var guild = this.JoinedGuilds[idx];
            //            if (eventData.InviteCode != null && !guild.Invites.Contains(eventData.InviteCode))
            //            {
            //                guild.Invites.Add(eventData.InviteCode);
            //            }

            //            // Get the earliest join date
            //            var timestamp = eventData.Timestamp;
            //            if (timestamp < guild.Timestamp)
            //            {
            //                guild.Timestamp = timestamp;
            //            }
            //        }
            //        else
            //        {
            //            this.JoinedGuilds.Add(new DAnalyticsGuild
            //            {
            //                Id = eventData.GuildId,
            //                JoinType = eventData.JoinType,
            //                JoinMethod = eventData.JoinMethod,
            //                ApplicationId = eventData.ApplicationId,
            //                Location = eventData.Location,
            //                Invites = (eventData.InviteCode != null ? new List<string> { eventData.InviteCode } : new List<string>()),
            //                Timestamp = eventData.Timestamp
            //            });
            //        }
            //        break;
            //    case "create_guild":
            //        this.JoinedGuilds.Add(new DAnalyticsGuild
            //        {
            //            Id = eventData.GuildId,
            //            JoinType = "created by you",
            //            Invites = new List<string>(),
            //            Timestamp = eventData.Timestamp
            //        });
            //        break;
            //    case "accepted_instant_invite":
            //        if (eventData.GuildId != null) this.AcceptedInvites.Add(eventData);
            //        break;
            //    case "voice_disconnect":
            //        this.VoiceDisconnections.Add(new DVoiceConnection()
            //        {
            //            ChannelId = eventData.ChannelId,
            //            ChannelType = eventData.ChannelType != null ? int.Parse(eventData.ChannelType) : null,
            //            GuildId = eventData.GuildId,
            //            Timestamp = eventData.Timestamp,
            //            Duration = TimeSpan.FromMilliseconds(eventData.Duration)
            //        });
            //        break;
            //}
        }
    }
}
