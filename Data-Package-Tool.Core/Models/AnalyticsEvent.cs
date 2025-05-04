using DataPackageTool.Core.Models.Analytics;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DataPackageTool.Core.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "event_type")]
    [JsonDerivedType(typeof(AcceptedInstantInvite), "accepted_instant_invite")]
    [JsonDerivedType(typeof(AddReaction), "add_reaction")]
    [JsonDerivedType(typeof(ChannelOpened), "channel_opened")]
    [JsonDerivedType(typeof(CreateGuild), "create_guild")]
    [JsonDerivedType(typeof(GuildJoined), "guild_joined")]
    [JsonDerivedType(typeof(SendMessage), "send_message")]
    [JsonDerivedType(typeof(SessionEnd), "session_end")]
    [JsonDerivedType(typeof(SessionStart), "session_start")]
    [JsonDerivedType(typeof(StartListening), "start_listening")]
    [JsonDerivedType(typeof(VoiceDisconnect), "voice_disconnect")]
    [JsonDerivedType(typeof(LeaveVoiceChannel), "leave_voice_channel")]

    public class AnalyticsEvent
    {
        [JsonPropertyName("event_source")]
        public string? EventSource { get; set; }
        public int Day { get; set; }
        [JsonPropertyName("")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("timestamp")]
        public string _timestamp { set => Timestamp = DateTime.Parse(value.Replace("\"", ""), null, DateTimeStyles.RoundtripKind); }

        public override int GetHashCode()
        {
            return (EventSource?.GetHashCode() ?? Day) + Timestamp.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            //Debug.WriteLine($"{ToString()} != {obj?.ToString()}");
            if (obj is AnalyticsEvent e)
            {
                if ((GetHashCode() == e?.GetHashCode()) && (Timestamp != e.Timestamp))
                {
                    Debug.WriteLine($"{ToString()} != {e.ToString()}");
                }
                return (GetHashCode() == e?.GetHashCode());
            }
            return false;
        }
    }
}
