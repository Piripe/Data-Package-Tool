using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageTool.Core.Models
{
    public class VoiceCall(DateTime startedAt, TimeSpan duration, Channel? channel)
    {
        public DateTime StartedAt { get; set; } = startedAt;
        public TimeSpan Duration { get; set; } = duration;
        public Channel? Channel { get; set; } = channel;

        public VoiceCall? SplitDay()
        {
            if (StartedAt.Day == (StartedAt + Duration).Day) return null;
            TimeSpan fullDuration = Duration;
            DateTime nextDay = new(StartedAt.Year,StartedAt.Month,StartedAt.Day);
            Duration = nextDay - StartedAt;
            return new(nextDay, fullDuration - Duration, Channel);
        }

        public bool Intersect(VoiceCall other)
        {
            DateTime thisEndAt = StartedAt + Duration;
            if (other.StartedAt >= thisEndAt) return false;
            if (other.StartedAt >= StartedAt)
            {
                other.Duration -= other.StartedAt - StartedAt;
                other.StartedAt = StartedAt + Duration;
                return true;
            }
            DateTime otherEndAt = other.StartedAt + other.Duration;
            if (otherEndAt <= StartedAt) return false;

            other.Duration = StartedAt - other.StartedAt;
            return true;
        }
    }
}
