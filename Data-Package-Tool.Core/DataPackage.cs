using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Data_Package_Tool.Core.Utils.Json;
using DataPackageTool.Core.Enums;
using DataPackageTool.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;

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
{ DisplayName = "Dummy user", Flags = (UserFlag)0x400048, ProfileMetadata = new UserProfileMetadata() { LegacyUsername = "Dummy user#1564", NitroStartedAt = new DateTime(2023,10,5), BoosingStartedAt = new DateTime(2023, 10, 7) } };
#else
;
#endif
        public readonly MemoryStream Avatar = new();
        public readonly List<DChannel> Channels = new();
        public readonly Dictionary<string, DChannel> ChannelsMap = new();
        public readonly Dictionary<string, Message> MessagesMap = new();
        public readonly Dictionary<string, User> UsersMap = new();

        public List<Attachment> ImageAttachments { get; private set; } = new List<Attachment>();
        public List<Guild> JoinedGuilds { get; private set; } = new List<Guild>()
#if DEBUG
        { new Guild() { 
            Id = "603970300668805120",
            Invites = ["discord-603970300668805120"]
        } };
#else
            ;
#endif
        public List<AnalyticsEvent> AcceptedInvites { get; private set; } = new List<AnalyticsEvent>();
        public List<VoiceConnection> VoiceDisconnections { get; private set; } = new List<VoiceConnection>();
        public Dictionary<string, string> GuildNamesMap { get; private set; } = new();

        public DateTime CreationTime { get; private set; } = DateTime.Now;
        public int TotalMessages { get; private set; } = 0;

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

                DataPackage dp = new DataPackage();

                var file = File.OpenRead(fileName);
                var zip = new ZipArchive(file, ZipArchiveMode.Read);

                var userFile = zip.GetEntry("account/user.json");
                if (userFile == null)
                {
                    throw new Exception("Invalid data package: missing user.json");
                }

                dp.CreationTime = userFile.LastWriteTime.DateTime;



                dp.User = JsonSerializer.Deserialize<User>(userFile.Open(), Shared.JsonSerializerOptions) ??dp.User;

                if (dp.User == null)
                {
                    throw new Exception("Invalid data package: wrong content in user.json");
                }

                if (dp.User.Relationships != null)
                {
                    foreach (var relationship in dp.User.Relationships)
                    {
                        if(relationship.User.Id!=null) dp.UsersMap.TryAdd(relationship.User.Id, relationship.User);
                    }
                } else
                {
                    // TODO: Add a warning for missing relationships
                }

                var channelNamesMap = new Dictionary<long, string>();
                var messagesIndexFile = zip.GetEntry("messages/index.json");
                if (messagesIndexFile != null)
                {
                    channelNamesMap = JsonSerializer.Deserialize<Dictionary<long, string>>(messagesIndexFile.Open(), Shared.JsonSerializerOptions) ?? channelNamesMap;
                }
                else
                {
                    // TODO: Add a warning for missing messages/index.json
                }


                var serversIndexFile = zip.GetEntry("servers/index.json");
                if (serversIndexFile != null)
                {
                    dp.GuildNamesMap = JsonSerializer.Deserialize<Dictionary<string, string>>(serversIndexFile.Open(), Shared.JsonSerializerOptions) ?? dp.GuildNamesMap;
                }
                else
                {
                    // TODO: Add a warning for missing servers/index.json
                }

                    var messagesRegex = new Regex(@"messages/(c?(\d+))/messages\.(csv|json)", RegexOptions.Compiled);
                    var avatarRegex = new Regex(@"account/avatar\.[a-z]+", RegexOptions.Compiled);
                    var nameRegex = new Regex(@"^Direct Message with (.+)#(\d{1,4})$", RegexOptions.Compiled);


                UpdateStatus(0.05f, "Reading messages...");
                int entriesCount = zip.Entries.Count;
                List<object?> parsedEntries = zip.Entries.Select((entry, i) =>
                {
                    if (i % 100 == 0) UpdateStatus(((float)i / entriesCount) * 0.75f + 0.05f);
                    var match = messagesRegex.Match(entry.FullName);
                    bool avatarMatch = false;
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
                }).Where((ZipEntryStreamAndMatches x) => x.stream != null).AsParallel().WithDegreeOfParallelism(16).Select<ZipEntryStreamAndMatches, object?>((entry,i) =>
                {
                    if (entry.messageMatch.Success)
                        {
                            var channelId = long.Parse(entry.messageMatch.Groups[2].Value);
                            var folderName = entry.messageMatch.Groups[1].Value; // folder name might not start with "c" in older versions
                            var fileExtension = entry.messageMatch.Groups[3].Value;

                            DChannel? channel = null;
                            if (entry.channelStream != null)
                            {
                                channel = JsonSerializer.Deserialize<DChannel>(entry.channelStream);
                                entry.channelStream.Dispose();
                            }
                            
                            if (channel == null)
                            {
                                // TODO: Add a warning for wrong data in messages/{folderName}/channel.json

                                channel = new DChannel() { Id = channelId.ToString() }; // A partial channel should be enough
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
                        if (entry is DChannel channel)
                        {
                            if (channel.IsDM())
                            {
                                var recipientId = channel.GetOtherDMRecipient(dp.User);
                                channel.DMRecipientId = recipientId;
                                if (!dp.UsersMap.ContainsKey(recipientId) && recipientId != Constants.DeletedUserId && channelNamesMap.TryGetValue(long.Parse(channel.Id??"0"), out var channelName))
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
                            dp.TotalMessages += channel.Messages.Count;
                            dp.Channels.Add(channel);
                            dp.ChannelsMap[channel.Id] = channel;
                        }
                        else if (entry is Bitmap avatar)
                        {
                            dp.User.AvatarImage = avatar;
                    }
                    i++;
                    if (i % 100 == 0) UpdateStatus((i / parsedEntries.Count)*0.05f+0.95f, $"Loading data ({i}/{parsedEntries.Count})");
                }
                    
                    
                    zip.Dispose();
                    file.Dispose();

                    dp.ImageAttachments = dp.ImageAttachments.OrderByDescending(o => o.Message.Id).ToList();
                    UpdateStatus(1f,$"Finished! Parsed {dp.TotalMessages.ToString("N0", new NumberFormatInfo { NumberGroupSeparator = " " })} messages in {Math.Floor((DateTime.Now - startTime).TotalSeconds)}s\nPackage created at: {dp.CreationTime.ToShortDateString()}",true);

                return dp;
            });
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
