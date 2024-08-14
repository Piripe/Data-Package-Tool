namespace DataPackageTool.Core.Models.Analytics
{
    public class SessionEnd : AnalyticsEvent
    {
        public string? Session {  get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (Session?.GetHashCode() ?? 0);
        }
    }
}
