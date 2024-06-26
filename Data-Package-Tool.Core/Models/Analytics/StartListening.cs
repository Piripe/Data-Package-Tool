using DataPackageTool.Core.Models.Analytics.Abstract;

namespace DataPackageTool.Core.Models.Analytics
{
    public class StartListening : ChannelEvent
    {
        public bool Mute { get; set; }

    }
}
