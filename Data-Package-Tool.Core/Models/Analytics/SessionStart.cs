using System.Threading;

namespace DataPackageTool.Core.Models.Analytics
{
    public class SessionStart : AnalyticsEvent
    {
        public string? Session {  get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (Session?.GetHashCode() ?? 0);
        }
    }
}
