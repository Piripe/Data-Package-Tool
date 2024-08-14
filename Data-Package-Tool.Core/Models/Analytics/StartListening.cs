using DataPackageTool.Core.Models.Analytics.Abstract;

namespace DataPackageTool.Core.Models.Analytics
{
    public class StartListening : ChannelEvent
    {
        public bool Mute { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (Mute.GetHashCode()<<5);
        }
    }
}
